﻿using Microsoft.Extensions.DependencyInjection;
using SmtpPilot.Server.Conversation;
using SmtpPilot.Server.Internal;
using SmtpPilot.Server.IO;
using System;

namespace SmtpPilot.Server.States
{
    internal class AcceptMailConversationState : MinimalConversationState
    {
        public override ConversationStateKey ThisKey => ConversationStates.Accept;

        public override void EnterState(SmtpStateContext context)
        {
            context.ContextBuilder.ResetState();
        }

        public override ConversationStateKey Advance(SmtpStateContext context)
        {
            var temp = context.ContextBuilder.GetTemporaryBuffer();

            if (!context.Client.PeekUntil(Markers.Space, temp, out var spaceIx))
            {
                return ThisKey;
            }

            var command = IOHelper.GetCommand(temp.Slice(0, spaceIx));
            var buffer = context.ContextBuilder.GetBuffer(1024);

            if (context.Client.ReadUntil(Markers.CarriageReturnLineFeed, buffer, spaceIx, out var count))
            {
                switch (command)
                {
                    case SmtpCommand.MAIL:

                        // Look for the ':' in MAIL FROM:
                        var start = buffer.IndexOf(':') + 1;
                        context.ContextBuilder.StartMessage(start, count - start);
                        context.Reply(SmtpReply.OK);

                        return ConversationStates.Recipient;
                    case SmtpCommand.VRFY:

                        context.Reply(new SmtpReply(SmtpReplyCode.Code250, String.Format("{0} <{0}@{1}>", "sample", context.Configuration.HostName)));

                        return ConversationStates.Accept;
                    default:
                        return ProcessBaseCommands(command, buffer.Slice(0, count), context);
                }

            }

            return ConversationStates.Accept;
        }

        internal override string HandleHelp()
        {
            return Constants.HelpTextAcceptState;
        }
    }
}
