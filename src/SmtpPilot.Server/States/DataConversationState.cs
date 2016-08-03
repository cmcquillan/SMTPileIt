﻿using System;
using SmtpPilot.Server.Conversation;
using SmtpPilot.Server.Internal;

namespace SmtpPilot.Server.States
{
    public class DataConversationState : IConversationState
    {
        private bool _headersAreOver;

        public SmtpCommand AllowedCommands
        {
            get
            {
                return SmtpCommand.NonCommand;
            }
        }

        public void EnterState(ISmtpStateContext context)
        {
            context.Reply(SmtpReply.BeginData);
            context.Conversation.AddElement(new SmtpData());
            _headersAreOver = false;
        }

        public void LeaveState(ISmtpStateContext context)
        {
            context.Reply(SmtpReply.OK);
        }

        public IConversationState ProcessData(ISmtpStateContext context, string line)
        {
            if (line.Equals(Constants.CarriageReturnLineFeed))
                return this;

            string choppedLine = line.Replace(Environment.NewLine, String.Empty);

            if (!_headersAreOver)
            {
                if (IO.IOHelper.LooksLikeHeader(line))
                {
                    string[] header = choppedLine.Split(new char[] { ':' }, 2);
                    context.AddHeader(new SmtpHeader(header[0], header[1]));
                }
                else
                {
                    _headersAreOver = true;
                }
            }
            else if (line.Equals(Constants.EndOfDataElement))
            {
                context.CompleteMessage();
                return new AcceptMailConversationState();
            }

            context.Conversation.CurrentMessage.AppendLine(choppedLine);

            return this;
        }

        public IConversationState ProcessNewCommand(ISmtpStateContext context, SmtpCmd cmd, string line)
        {
            throw new NotSupportedException();
        }
    }
}