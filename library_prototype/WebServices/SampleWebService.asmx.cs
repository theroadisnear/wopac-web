using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using library_prototype.DAL;
using library_prototype.Models;
using library_prototype.CustomClass;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data.Entity;
using Newtonsoft.Json;

namespace library_prototype.WebServices
{
    /// <summary>
    /// Summary description for SampleWebService
    /// </summary>
    [WebService(Namespace = "http://microsoft.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SampleWebService : System.Web.Services.WebService
    {
        LibraryDbContext db = new LibraryDbContext();
        
        [WebMethod]
        public LibraryDbContext.UserModel VerifyEmail(string email)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getUser = db.Users.Include(u=>u.Student).Where(u=>u.Email == email && u.Deleted == false).FirstOrDefault();
            
            return getUser;
        }

        [WebMethod]
        public List<LibraryDbContext.SubjectModel> DownloadSubject()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getSubjects = db.Subjects.ToList();
            return getSubjects;
        }

        [WebMethod]
        public List<LibraryDbContext.BookModel> DownloadBook()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getBooks = db.Books.Where(b => b.Delete == false).ToList();
            return getBooks;
        }

        [WebMethod]
        public List<LibraryDbContext.AuthorModel> DownloadAuthor()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getAuthors = db.Authors.Where(b => b.Delete == false).ToList();
            return getAuthors;
        }

        [WebMethod]
        public List<LibraryDbContext.BookAuthorModel> DownloadBookAuthors()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getBookAuthors = db.BooksAuthors.ToList();
            return getBookAuthors;
        }

        [WebMethod]
        public List<LibraryDbContext.PublisherModel> DownloadPublisher()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getPublishers = db.Publishers.Where(p=>p.Delete == false).ToList();
            return getPublishers;
        }

        [WebMethod]
        public List<LibraryDbContext.ReservationModel> ViewReservation(string userId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getReservation = db.Reservations.Where(r => r.UserId == new Guid(userId) && r.Delete == false).ToList();
            return getReservation;
        }

        [WebMethod]
        public string ReserveBook(string userId, string bookId)
        {
            var getUser = db.Users.Where(u => u.Id == new Guid(userId) && u.Deleted == false).SingleOrDefault();
            var getBook = db.Books.Where(b => b.Id == new Guid(bookId) && b.Delete == false).SingleOrDefault();
            string message="";
            if (getUser != null)
            {
                if (getBook != null)
                {
                    if(getBook.Borrow == true)
                    {
                        if (!(db.Reservations.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id && c.Delete == false).Any()))
                        {
                            var checkUserReservation = db.Reservations.Where(r => r.UserId == getUser.Id && r.Delete == false).ToList();
                            if (checkUserReservation.Count <= 3)
                            {
                                var newReservation = db.Reservations.Create();
                                newReservation.User = getUser;
                                newReservation.Book = getBook;
                                newReservation.CreatedAt = DateTime.UtcNow;

                                db.Reservations.Add(newReservation);
                                db.SaveChanges();
                                message = "You have successfully made a reservation";
                            }
                            else if (checkUserReservation.Count > 3)
                            {
                                message = "You have already exceeded the maximumum number of reservation";
                            }
                        }
                        else if (db.Reservations.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id && c.Delete == false).Any())
                        {
                            message = "You have already made a reservation for this book";
                        }
                    }
                    else if(getBook.Borrow == false)
                    {
                        message = "The selected book is can't be borrowed";
                    }
                    
                }
                else if (getBook == null)
                {
                    message = "The selected book is not existing!!";
                }
            }
            else if (getUser == null)
            {
                message = "Invalid user!! Kindly sign in again";
            }
            return message;
        }

        [WebMethod]
        public string CancelReservation(string reservationId)
        {
            string message = "";
            var delReservation = db.Reservations.Where(r => r.Id == new Guid(reservationId) && r.Delete == false).SingleOrDefault();
            if (delReservation != null)
            {
                delReservation.Delete = true;
                delReservation.UpdatedAt = DateTime.UtcNow;
                db.Entry(delReservation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                message = "You have successfully canceled a reservation";
            }
            else if(delReservation == null)
            {
                message = "The reservation is not existing";
            }
            return message;
        }

        [WebMethod]
        public List<LibraryDbContext.CommentModel> ViewComment(string bookId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getComment = db.Comments.Where(r => r.BookId == new Guid(bookId) && r.Delete == false).OrderByDescending(c=>c.CreatedAt).ToList();
            return getComment;
        }

        [WebMethod]
        public LibraryDbContext.UserModel GetUser(string userId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getUser = db.Users.Include(u=>u.Student).Where(u => u.Id == new Guid(userId) && u.Deleted == false).SingleOrDefault();
            return getUser;
        }

        [WebMethod]
        public LibraryDbContext.BookModel GetBook(string bookId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var getBook = db.Books.Where(u => u.Id == new Guid(bookId) && u.Delete == false).SingleOrDefault();
            return getBook;
        }

        [WebMethod]
        public string AddNewComment(string userId, string bookId, string comment)
        {
            string message = "";
            var getUser = db.Users.Where(u => u.Id == new Guid(userId) && u.Deleted == false).SingleOrDefault();
            if (getUser != null)
            {
                var getBook = db.Books.Where(b => b.Id == new Guid(bookId) && b.Delete == false).SingleOrDefault();
                if (getBook != null)
                {
                    if (!(db.Comments.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Where(c => c.Comment.ToLower() == comment.ToLower()).Any()))
                    {
                        var newComment = db.Comments.Create();
                        newComment.Book = getBook;
                        newComment.User = getUser;
                        newComment.Comment = comment;
                        newComment.CreatedAt = DateTime.UtcNow;

                        db.Comments.Add(newComment);
                        db.SaveChanges();
                        message = "You have successfully add a new comment";
                    }
                    else if (db.Comments.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Where(c => c.Comment.ToLower() == comment.ToLower()).Any())
                    {
                        message = "Duplicate comments are not allowed!!";
                    }
                }
                else if (getBook == null)
                {
                    message = "The selected book is not existing!!";
                }
            }
            else if (getUser == null)
            {
                message = "Invalid user!! Kindly sign in again";
            }
            return message;
        }

        [WebMethod]
        public string RateBook(string userId, string bookId, int rate)
        {
            var getUser = db.Users.Where(u => u.Id == new Guid(userId) && u.Deleted == false).SingleOrDefault();
            var getBook = db.Books.Where(b => b.Id == new Guid(bookId) && b.Delete == false).SingleOrDefault();
            string message = "";
            if (rate == 1 || rate == 2 || rate == 3)
            {
                if (getUser != null)
                {
                    if (getBook != null)
                    {
                        if (!(db.Ratings.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Any()))
                        {
                            var newRate = db.Ratings.Create();
                            newRate.Rate = rate;
                            newRate.BookId = getBook.Id;
                            newRate.UserId = getUser.Id;
                            newRate.CreatedAt = DateTime.UtcNow;

                            db.Ratings.Add(newRate);
                            db.SaveChanges();
                            message = "You have successfully rated this book";
                        }
                        else if (db.Ratings.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Any())
                        {
                            message = "You have already rated this book";
                        }
                    }
                    else if (getBook == null)
                    {
                        message = "The selected book is not existing!!";
                    }
                }
                else if (getUser == null)
                {
                    message = "Invalid user!! Kindly sign in again";
                }
            }
            else if (rate > 3)
            {
                message = "Error";
            }
            return message;
        }

        [WebMethod]
        public string NotifyBookDeadline()
        {
            string message = "";
            db.Configuration.ProxyCreationEnabled = false;
            var dateTime = DateTime.UtcNow.AddDays(-1);
            var getBookLogs = db.BookLogs.Where(b => b.Return == false).Where(b=>b.Deadline >= dateTime && b.Deadline <= DateTime.UtcNow).ToList();
            foreach (var log in getBookLogs)
            {
                message = message + SMTP.SendNotification(log.Id);
            }
            return message;
        }
    }
}

