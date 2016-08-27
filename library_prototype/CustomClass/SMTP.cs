using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using SendGrid;
using library_prototype.DAL;
using library_prototype.CustomClass;

namespace library_prototype.CustomClass
{
    public class SMTP
    {
        public void SendEmailPinCode(string emailReceiver, string pincode)
        {
            using (var db = new LibraryDbContext())
            {
                try
                {
                    var getMessage = db.EmailMessages.Where(e=>e.Deleted == false).Where(e=>e.Type == "accountpincode").SingleOrDefault();
                    using (MailMessage mm = new MailMessage("stvpslibrary@gmail.com", emailReceiver))
                    {
                        mm.Subject = getMessage.Subject;
                        mm.Body = "<html><head></head><body><p>" + getMessage.Body + "</p><p>"+
                            "To activate your account kindly use the pincode provided <b><u>"+pincode+"</u></b></p></body></html>";
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = getMessage.EmailCredential.Host;
                        smtp.EnableSsl = true;
                        NetworkCredential nc = new NetworkCredential(getMessage.EmailCredential.Username, CustomDecrypt.Decrypt(getMessage.EmailCredential.Password));
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = nc;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error SMTP: " + e);
                }
                
            }
        }

        public static string SendNotification(Guid id)
        {
            using (var db = new LibraryDbContext())
            {
                try
                {
                    var getMessage = db.EmailMessages.Where(e => e.Deleted == false).Where(e => e.Type == "notification").SingleOrDefault();
                    var getBookLog = db.BookLogs.Where(b => b.Id == id).SingleOrDefault();
                    using (MailMessage mm = new MailMessage("stvpslibrary@gmail.com", getBookLog.User.Email))
                    {
                        mm.Subject = getMessage.Subject;
                        mm.Body = "<html><head></head><body><p>"+ getMessage.Body + "</p> <ul>"+
                            "<li> Borrower's Name: " + getBookLog.User.Student.LastName + ", " + getBookLog.User.Student.FirstName + "</li>" +
                            "<li> Book's Title: " + getBookLog.Book.Title + "</li>" +
                            "<li> Deadline: " + getBookLog.Deadline.AddHours(8).ToShortDateString() + "</li>" +
                            "</ul></body></html>";
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = getMessage.EmailCredential.Host;
                        smtp.EnableSsl = true;
                        NetworkCredential nc = new NetworkCredential(getMessage.EmailCredential.Username, CustomDecrypt.Decrypt(getMessage.EmailCredential.Password));
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = nc;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                    return "";
                }
                catch (Exception e)
                {
                    return "SMTP Error:" + e;
                }

            }
        }

        //public static void SendNotification()
        //{
        //    using (MailMessage mm = new MailMessage("stvpslibrary@gmail.com", "rodnerraymundo@gmail.com"))
        //    {
        //        mm.Subject = "WOPAC Account Activation";
        //        mm.Body = "Hi, here's your pincode ";
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
    }
}