﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTPileIt.Server.Conversation
{
    public interface IAppendable
    {
        void AppendLine(string l);
        void Append(string l);
    }
}
