using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace EmailTestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AWSMailEngine awsMail = new AWSMailEngine();
            O365MailEngine o365Mail = new O365MailEngine();

            Console.WriteLine("Sending Message....");

            #region AWS SES
            //Console.WriteLine("Using AWS SES...");
            //awsMail.SendMail();
            #endregion

            #region O365 graph mail
            //Console.WriteLine("Using O365 Graph Mail");
            //string res = o365Mail.SendMessage().Result;
            //Console.WriteLine(res);

            Console.WriteLine("Getting Inbox O365 Graph Mail");
            var inbox = o365Mail.GetInbox().Result;
            var messages = inbox.CurrentPage;
            Console.WriteLine("Inbox count:" + messages.Count);
            var messageIdList = new List<string>();
            foreach (var mail in messages)
            {
                Console.WriteLine("==========================");
                Console.WriteLine($"ID:{mail.Id}"); messageIdList.Add(mail.Id);
                Console.WriteLine();
                Console.WriteLine($"Subject:{mail.Subject}");
                Console.WriteLine();
                Console.WriteLine($"From:{mail.From.EmailAddress.Address}");
                Console.WriteLine();
                Console.WriteLine($"Body:{mail.BodyPreview}");
                Console.WriteLine("==========================");
                Console.WriteLine();
            }

            foreach (var mailId in messageIdList)
            {
                Console.WriteLine(o365Mail.DeleteMessage(mailId).Result);
            }
            #endregion

            Console.WriteLine("Ending Program...");
        }
    }
}
