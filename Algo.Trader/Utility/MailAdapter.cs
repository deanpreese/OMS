using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Algo.Trader.Utility
{
    public class MailAdapter
    {
        private string fromAddress = "contact@blackwavetechnologies.com";
        private string smtpAddress = "smtpout.secureserver.net";
        private string networkUser = "contact@blackwavetechnologies.com";
        private string networkPassword = "P@ssw0rd";

        public void SendEmailMessage(string toAddress, string subject, string body)
        {
            // New message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(fromAddress);
            mail.To.Add(toAddress);
            mail.IsBodyHtml = true;

            //set the content
            mail.Subject = subject;
            mail.Body = body;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpAddress);
            try
            {
                //send the message
                smtp.Credentials = new System.Net.NetworkCredential(networkUser, networkPassword);
                smtp.Send(mail);

                //smtp_local.Send(mail);

            }
            catch (Exception ex)
            {
                string err = "Error Sending Email Message: " + ex.ToString();
                Console.WriteLine(err);
            }


        }


    }
}