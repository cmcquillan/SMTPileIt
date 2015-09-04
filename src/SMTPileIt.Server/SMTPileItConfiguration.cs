﻿using SMTPileIt.Server.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTPileIt.Server
{
    public class SMTPileItConfiguration
    {
        public const string Localhost = "127.0.0.1";
        public const int DefaultSmtpPort = 25;

        public SMTPileItConfiguration()
            : this(Localhost, DefaultSmtpPort)
        {

        }

        public SMTPileItConfiguration(string listenUri, int portNumber)
        {
            Listeners.Add(new TcpClientListener(listenUri, portNumber));
        }

        public IList<IMailClientListener> Listeners { get; } = new List<IMailClientListener>();
    }
}
