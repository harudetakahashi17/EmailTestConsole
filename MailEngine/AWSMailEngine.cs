using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EmailTestConsole
{
    public class AWSMailEngine
    {
        public AWSMailEngine()
        {
        }

        public void SendMail()
        {
            string AWSSESMailServer = "email-smtp.ap-southeast-1.amazonaws.com";
            string AWSSESSMTPPort = "587";
            string AWSSESUsername = "Your Username";
            string ASWSESPassword = "Your Password";

            string subject = "Email Test";
            string emaiFrom = "example@domain.com";
            string emailFromName = "Example User";
            string emailTo = "yourmail@yopmail.com";
            string emailToName = "Your Name";
            try
            {
                #region AWS SES
                using (var client = new SmtpClient(AWSSESMailServer, Convert.ToInt32(AWSSESSMTPPort)))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(AWSSESUsername, ASWSESPassword);

                    MailMessage message = new MailMessage()
                    {
                        From = new MailAddress(emaiFrom, emailFromName),
                        Body = "<p>Email test, please ignore it</p><br><p>server : email-smtp.ap-southeast-1.amazonaws.com</p>",
                        Subject = subject,
                        IsBodyHtml = true
                    };
                    message.To.Add(new MailAddress(emailTo, emailToName));

                    client.Send(message);
                    Console.WriteLine("Email Sent...");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex);
            }
        }
    }
}
