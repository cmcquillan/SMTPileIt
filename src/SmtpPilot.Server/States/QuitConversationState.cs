﻿using SmtpPilot.Server.Conversation;
using System;

namespace SmtpPilot.Server.States
{
    internal class QuitConversationState : IConversationState
    {
        public bool ShouldDisconnect => true;

        public ConversationStateKey Advance(SmtpStateContext context)
        {
            throw new NotImplementedException();
        }

        public void EnterState(SmtpStateContext context)
        {
            context.Reply(SmtpReply.ServerClosing);
            context.Configuration.ServerEvents.OnClientDisconnected(this, new MailClientDisconnectedEventArgs(context.Client, DisconnectReason.TransactionCompleted));
        }
    }
}