﻿using SMTPileIt.Server.Conversation;
using SMTPileIt.Server.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMTPileIt.Server.States
{
    public interface ISmtpStateContext
    {
        IMailClient Client { get; }
        SmtpConversation Conversation { get; }
        SmtpCommand Command { get; }
    }
}
