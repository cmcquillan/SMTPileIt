﻿using SMTPileIt.Server.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTPileIt.Server.Conversation
{
    public class SmtpConversation
    {
        private readonly List<ConversationElement> _elements = new List<ConversationElement>();
        private string _fromAddress;
        private string[] _toAddresses;

        public SmtpConversation() {  }

        public IReadOnlyList<ConversationElement> Elements
        {
            get
            {
                return _elements.AsReadOnly();
            }
        }

        public string FromAddress
        {
            get { return _fromAddress; }
            set { _fromAddress = value; }
        }

        public ConversationElement LastElement
        {
            get 
            {
                if (_elements.Count == 0)
                    return null;

                return _elements.Last();
            }
        }

        public bool HasError
        {
            get
            {
                //CONSIDER: Linq performance vs loop performance.
                return _elements.Select(p => p as SmtpReply)
                  .Where(p => p != null && p.IsError)
                  .Any();
            }
        }

        public SmtpCmd LastCommand
        {
            get
            {
                var q = _elements.Where(p => p is SmtpCmd);
                if (!q.Any())
                    return null;
                return q.Last() as SmtpCmd;
            }
        }

        public SmtpReply LastReply
        {
            get
            {
                var q = _elements.Where(p => p is SmtpReply);
                if (q.Any())
                    return null;
                return q.Last() as SmtpReply;
            }
        }

        public string[] ToAddresses
        {
            get { return _toAddresses; }
            set { _toAddresses = value; }
        }

        public void AddElement(ConversationElement element)
        {
            _elements.Add(element);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach(var e in _elements)
            {
                sb.Append(e.ToString());
            }

            return sb.ToString();
        }
    }
}
