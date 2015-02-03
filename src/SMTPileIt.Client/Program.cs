﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMTPileIt.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);

            using (var client = new SmtpClient("localhost", 25))
            {
                var message = new MailMessage("casey.mcquillan@gmail.com", "cmcquillan@restaurant.org");
                message.Subject = "Hello, World";
                message.Body = "This is my message";

                client.Send(message);
            }
        }
    }
}
