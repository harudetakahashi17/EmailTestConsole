using System;
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
            Console.WriteLine("Using O365 Graph Mail");
            string res = o365Mail.SendMessage().Result;
            Console.WriteLine(res);

            Console.WriteLine("Getting Inbox O365 Graph Mail");
            var inbox = o365Mail.GetInbox().Result;
            var messages = inbox.CurrentPage;
            Console.WriteLine("Inbox count:" + messages.Count);
            foreach (var mail in messages)
            {
                Console.WriteLine("==========================");
                Console.WriteLine($"ID:{mail.Subject}");
                Console.WriteLine($"Subject:{mail.Subject}");
                Console.WriteLine($"From:{mail.From.EmailAddress.Address}");
                Console.WriteLine($"Body:{mail.BodyPreview}");
                Console.WriteLine("==========================");
                Console.WriteLine();
            }
            #endregion

            Console.WriteLine("Ending Program...");
        }
    }
}
