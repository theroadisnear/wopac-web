using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using System.Net;
using System.Net.Mail;
using WebJob2.AzureWebService;

namespace WebJob2
{
    class SMTP
    {
        public static void SendNotification()
        {
            var webService = new AzureWebService.SampleWebServiceSoapClient();
            try
            {
                var getNotifyUsers = webService.NotifyBookDeadline();
                //foreach(var user in getNotifyUsers)
                //{
                //    var getUser = webService.GetUser(user.UserId.ToString());
                //    var getBook = webService.GetBook(user.BookId.ToString());
                //    using (MailMessage mm = new MailMessage("stvpslibrary@gmail.com", getUser.Email))
                //    {
                //        mm.Subject = "Library Notification";
                //        mm.Body = "Hi! " + getUser.Student.FirstName + " " + getUser.Student.LastName + 
                //            ", You are required to return the book ("+getBook.Title+") on or before "+
                //            user.Deadline.AddHours(8).ToShortDateString()+". Thank you!";
                //        mm.IsBodyHtml = false;
                //        SmtpClient smtp = new SmtpClient();
                //        smtp.Host = "smtp.sendgrid.net";
                //        smtp.EnableSsl = true;
                //        NetworkCredential nc = new NetworkCredential("azure_324d8597ef63a3693116719bfa792c04@azure.com", "bg5PSAAPof9L2TW");
                //        smtp.UseDefaultCredentials = false;
                //        smtp.Credentials = nc;
                //        smtp.Port = 587;
                //        smtp.Send(mm);
                //    }
                //}
                Console.WriteLine("Web service success: ");
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Web service error: "+e);
            }
            
        }
    }
}
