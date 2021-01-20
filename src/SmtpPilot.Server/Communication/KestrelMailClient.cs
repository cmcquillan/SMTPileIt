﻿using Microsoft.AspNetCore.Connections;
using SmtpPilot.Server.IO;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmtpPilot.Server.Communication
{
    public class KestrelMailClient : IMailClient
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Func<bool> _closedFunc;
        private readonly PipeReader _reader;
        private readonly PipeWriter _writer;

        public KestrelMailClient(ConnectionContext context)
        {
            context.ConnectionClosed.Register(OnClosed);
            _reader = context.Transport.Input;
            _writer = context.Transport.Output;

            object isClosed = false;
            _closedFunc = () => (bool)isClosed;
            void OnClosed()
            {
                isClosed = true;
            }
        }

        public KestrelMailClient(TcpClient client)
        {
            var stream = client.GetStream();
            _reader = PipeReader.Create(stream);
            _writer = PipeWriter.Create(stream);
            _closedFunc = () => !client.Connected;
        }

        public Guid ClientId { get; } = Guid.NewGuid();
        
        public bool Disconnected => _closedFunc();
        
        public int SecondsClientHasBeenSilent => 1;

        public void Disconnect()
        {
            // Maybe?
            _writer.Complete();
        }

        private bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            var position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            var slicePosition = buffer.GetPosition(1, position.Value);

            line = buffer.Slice(0, slicePosition);
            buffer = buffer.Slice(slicePosition);
            return true;
        }

        public void Write(string message)
        {
            var newlines = Encoding.ASCII.GetBytes(Environment.NewLine);
            _writer.WriteAsync(Encoding.ASCII.GetBytes(message)).GetAwaiter().GetResult();
            _writer.WriteAsync(newlines).GetAwaiter().GetResult();
        }

        public int ReadLine(Span<char> outBuffer)
        {
            if (_reader.TryRead(out var read))
            {
                var inBuffer = read.Buffer;
                if (TryReadLine(ref inBuffer, out var line))
                {
                    FillLine(line, outBuffer, out var processed, out var written);
                    _reader.AdvanceTo(inBuffer.Start);
                    return written;
                }
            }

            return 0;
        }

        private void FillLine(ReadOnlySequence<byte> line, Span<char> buffer, out int processed, out int written)
        {
            var decoder = Encoding.ASCII.GetDecoder();
            var length = buffer.Length;
            processed = 0;
            written = 0;

            foreach(var bytes in line)
            {
                processed += bytes.Length;
                var last = processed == length;
                var span = bytes.Span;
                var chars = decoder.GetChars(span, buffer, last);
                buffer = buffer.Slice(0, chars);
                written += chars;
            }

        }
    }
}
