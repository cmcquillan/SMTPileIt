﻿using SMTPileIt.Server.Conversation;
using SMTPileIt.Server.IO;
using SMTPileIt.Server.States;
using System.Collections.Generic;
using System.Threading;

namespace SMTPileIt.Server
{
    public class SMTPileIt
    {
        public const int DefaultSmtpPort = 25;

        private readonly IMailClientListener _listener;
        private readonly List<IMailClient> _clients;
        private readonly Dictionary<int, SmtpStateMachine> _conversations;


        public SMTPileIt(string ipString, int ipPort)
        {
            _listener = new TcpClientListener(ipString, ipPort);
            _clients = new List<IMailClient>();
            _conversations = new Dictionary<int, SmtpStateMachine>();
        }

        public void Run()
        {
            while(true)
            {
                if (_listener.ClientPending)
                {
                    var c = _listener.AcceptClient();
                    _clients.Add(c);
                    _conversations[c.ClientId] = new SmtpStateMachine(c, new SmtpConversation());
                    //c.Write(new SmtpReply(SmtpReplyCode.Code220).ToString());
                }

                foreach (var client in _clients)
                {
                    if (_conversations[client.ClientId].IsInQuitState)
                    {
                        client.Disconnect();
                        continue;
                    }

                    _conversations[client.ClientId].ProcessLine();
                    Thread.Sleep(5);
                }

                _clients.RemoveAll(p => p.Disconnected);
            }
        }
    }
}
