using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace SummaryAPI2.Helpers
{
    public class Mail
    {
        public void LicenseMail(string MailId, string HtmlContent)
        {
            try
            {
                MailMessage message = new MailMessage();
                //string Mail = "ajaybharath.rapelli@ideabytes.com";
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("services@iotsystem.online", "IoT Services");
                message.To.Add(new MailAddress(MailId));
                //foreach (string m in mail)
                //{
                //    if (!string.IsNullOrEmpty(m))
                //    {
                //        try
                //        {
                //            message.To.Add(new MailAddress(m));
                //        }
                //        catch (Exception ex)
                //        {
                //            ex = null;
                //            continue;
                //        }

                //    }
                //}
                //  message.To.Add(new MailAddress("chandana.akoju@ideabytes.com"));
                //message.To.Add(new MailAddress("ajaybharath.rapelli@ideabytes.com"));

                message.IsBodyHtml = true;
                message.Subject = "License Subscription Details";

                message.Body = HtmlContent;

                //byte[] bytes = generate();
                //message.Attachments.Add(new Attachment(new MemoryStream(bytes), "Invoice.pdf"));

                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new NetworkCredential("services@iotsystem.online", "Ide@#321");

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);

            }
            catch (Exception ex)
            {
                ex = null;
            }
        }
    }
}