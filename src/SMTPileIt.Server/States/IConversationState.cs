﻿using SMTPileIt.Server.Conversation;
using SMTPileIt.Server.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTPileIt.Server.States
{
    public interface IConversationState
    {
        void EnterState(ISmtpStateContext context);
        void LeaveState(ISmtpStateContext context);
        IConversationState ProcessNewCommand(ISmtpStateContext context, SmtpCmd cmd, string line);
        IConversationState ProcessData(ISmtpStateContext context, string line);
        SmtpCommand AllowedCommands { get; }
    }
}
