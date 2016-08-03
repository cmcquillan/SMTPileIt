﻿using SmtpPilot.Server.Conversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmtpPilot.Server.Data
{
    [DataContract]
    internal class XmlMailMessage
    {
        internal const string Namespace = "https://github.com/cmcquillan/SmtpPilot/08/2016/XmlMail";

        [DataMember(Order = 0)]
        internal string From { get; set; }

        [DataMember(Order = 1)]
        internal XmlAddressList To { get; set; }

        [DataMember(Order = 2)]
        internal XmlAddressList Cc { get; set; }

        [DataMember(Order = 3)]
        internal XmlAddressList Bcc { get; set; }

        [DataMember(Order = 4)]
        internal XmlMailHeader[] Headers { get; set; }

        [DataMember(Order = 5)]
        internal string Message { get; set; }

        internal static XmlMailMessage FromMessage(IMessage message)
        {
            return new XmlMailMessage()
            {
                From = message.FromAddress.ToString(),
                To = new XmlAddressList(message.ToAddresses.Where(p => p.Type == AddressType.To).Select(p => p.ToString()).ToList()),
                Cc = new XmlAddressList(message.ToAddresses.Where(p => p.Type == AddressType.Cc).Select(p => p.ToString()).ToList()),
                Bcc = new XmlAddressList(message.ToAddresses.Where(p => p.Type == AddressType.Bcc).Select(p => p.ToString()).ToList()),
                Headers = message.Headers.Select(p => new XmlMailHeader() { Name = p.Name, Value = p.Value }).ToArray(),
                Message = message.Data,
            };
        }
    }
}
