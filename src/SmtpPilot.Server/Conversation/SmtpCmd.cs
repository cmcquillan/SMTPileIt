﻿using System;
using System.Text;

namespace SmtpPilot.Server.Conversation
{
    public class SmtpCmd : ConversationElement, IAppendable
    {
        private readonly SmtpCommand _cmd;
        private readonly StringBuilder _lines;

        public SmtpCmd(SmtpCommand cmd, string text)
        {
            _cmd = cmd;
            _lines = new StringBuilder();

            if (!String.IsNullOrEmpty(text))
                _lines.AppendLine(text);
        }

        public override string Preamble
        {
            get { return _cmd.ToString(); }
        }

        public bool IsCommand
        {
            get
            {
                return Enum.TryParse(Preamble, out SmtpCommand _);
            }
        }

        public SmtpCommand Command
        {
            get
            {
                return (SmtpCommand)Enum.Parse(typeof(SmtpCommand), Preamble);
            }
        }

        public override string FullText
        {
            get { return ToString(); }
        }

        public string Args
        {
            get
            {
                var sb = new StringBuilder(FullText);
                sb = sb.Replace(Preamble, String.Empty, 0, Preamble.Length);
                return sb.ToString().Trim();
            }
        }

        public void AppendLine(string l)
        {
            _lines.AppendLine(l);
        }

        public override string ToString()
        {
            return String.Format("{0}", _lines.ToString());
        }

        public void Append(string l)
        {
            _lines.Append(l);
        }
    }
}
