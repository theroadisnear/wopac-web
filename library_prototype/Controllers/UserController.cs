using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using library_prototype.Models;
using library_prototype.DAL;
using library_prototype.CustomClass;

namespace library_prototype.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private LibraryDbContext db = new LibraryDbContext();

        [Authorize(Roles ="student, administrator")]
        public ActionResult UserProfile()
        {
            var model = new MultipleModel.UserProfileVM();
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            model.User = db.Users.Where(u => u.Id == new Guid(getUserId) && u.Deleted == false).SingleOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult SetProfilePicture(HttpPostedFileBase img, MultipleModel.ProfileVM prof)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Books()
        {
            var model = new MultipleModel.BooksVM();
            model.Books = db.Books.Where(b => b.Delete == false).OrderByDescending(b => b.CreatedAt).Take(12).ToList();
            return View(model);
        }
        
        public ActionResult GetBooksbySubject(int classNumber)
        {
            var getBooks = db.Books.Where(b => b.Subject.CallNo >= classNumber && b.Subject.CallNo <= classNumber + 99).ToList();
            return PartialView("_Books", getBooks);
        }

        public JsonResult SearchAutoComplete(string term)
        {
            using (var db = new LibraryDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var booksList = db.Books.Where(s => s.Title.ToLower().Contains(term.ToLower())).ToList();
                return Json(booksList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchBook(string term)
        {
            var getBooks = db.Books.Where(s => s.Title.ToLower().Contains(term.ToLower())).ToList();
            return PartialView("_Books", getBooks);
        }

        public ActionResult ShowBook(string id)
        {
            var model = new MultipleModel.BooksVM();
            model.Book = db.Books.Where(b => b.Id == new Guid(id)).SingleOrDefault();
            return PartialView("_ShowBook", model);
        }

        public ActionResult ViewBookComment(string id)
        {
            var model = new MultipleModel.CommentsVM();
            model.Book = db.Books.Where(b => b.Id == new Guid(id) && b.Delete == false).SingleOrDefault();
            return PartialView("_ViewBookComment", model);
        }

        public ActionResult ViewReservation(string id)
        {
            var getReservation = db.Reservations.Where(r => r.UserId == new Guid(id) && r.Delete == false).ToList();
            return PartialView("_ViewReservation", getReservation);
        }
        
        public ActionResult ViewOnHand()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var getUser = db.Users.Where(u => u.Id == new Guid(getUserId) && u.Deleted == false).SingleOrDefault();

            var getBook = db.BookLogs.Where(b => b.UserId == getUser.Id && b.Return == false).ToList();
            return PartialView("_ViewOnHand", getBook);
        }

        [HttpPost]
        public ActionResult CancelReservation(string id)
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var getUser = db.Users.Where(u => u.Id == new Guid(getUserId) && u.Deleted == false).SingleOrDefault();

            var delReservation = db.Reservations.Where(r => r.Id == new Guid(id) && r.Delete == false).SingleOrDefault();
            if (delReservation != null)
            {
                delReservation.Delete = true;
                delReservation.UpdatedAt = DateTime.UtcNow;
                db.Entry(delReservation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            var getReservation = db.Reservations.Where(r => r.UserId == getUser.Id && r.Delete == false).ToList();
            return PartialView("_ViewReservation", getReservation);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles ="student, administrator")]
        public ActionResult AddNewComment(MultipleModel.CommentsVM request)
        {
            var errorList = new List<string>();
            if (ModelState.IsValid)
            {
                var getUser = db.Users.Where(u => u.Id == request.NewComment.UserId && u.Deleted == false).SingleOrDefault();
                if(getUser != null)
                {
                    var getBook = db.Books.Where(b=>b.Id == request.NewComment.BookId && b.Delete == false).SingleOrDefault();
                    if(getBook != null)
                    {
                        if(!(db.Comments.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Where(c => c.Comment.ToLower() == request.NewComment.Comment.ToLower()).Any()))
                        {
                            var newComment = db.Comments.Create();
                            newComment.Book = getBook;
                            newComment.User = getUser;
                            newComment.Comment = request.NewComment.Comment;
                            newComment.CreatedAt = DateTime.UtcNow;

                            db.Comments.Add(newComment);
                            db.SaveChanges();
                            ModelState.Clear();
                            request.Error = false;
                            string message = "You have successfully add a new comment";
                            errorList.Add(message);
                            request.Message = errorList;
                        }
                        else if(db.Comments.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Where(c => c.Comment.ToLower() == request.NewComment.Comment.ToLower()).Any())
                        {
                            request.Error = true;
                            string message = "Duplicate comments are not allowed!!";
                            errorList.Add(message);
                            request.Message = errorList;
                        }
                    }
                    else if(getBook == null)
                    {
                        request.Error = true;
                        string message = "The selected book is not existing!!";
                        errorList.Add(message);
                        request.Message = errorList;
                    }
                }
                else if(getUser == null)
                {
                    request.Error = true;
                    string message = "Invalid user!! Kindly sign in again";
                    errorList.Add(message);
                    request.Message = errorList;
                }
            }
            else if(!(ModelState.IsValid))
            {
                request.Error = true;
                request.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            request.Book = db.Books.Where(b => b.Id == request.NewComment.BookId && b.Delete == false).SingleOrDefault();
            return PartialView("_ViewBookComment", request);
        }

        [HttpPost]
        [Authorize(Roles = "student, administrator")]
        public ActionResult RateBook(string id, int rate)
        {
            var errorList = new List<string>();
            var request = new MultipleModel.BooksVM();
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var getUser = db.Users.Where(u => u.Id == new Guid(getUserId) && u.Deleted == false).SingleOrDefault();

            var getBook = db.Books.Where(b => b.Id == new Guid(id) && b.Delete == false).SingleOrDefault();
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
                            ModelState.Clear();
                            request.Error = false;
                            string message = "You have successfully rated this book";
                            errorList.Add(message);
                            request.Message = errorList;
                        }
                        else if (db.Ratings.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id).Any())
                        {
                            request.Error = true;
                            string message = "You have already rated this book";
                            errorList.Add(message);
                            request.Message = errorList;
                        }
                    }
                    else if (getBook == null)
                    {
                        request.Error = true;
                        string message = "The selected book is not existing!!";
                        errorList.Add(message);
                        request.Message = errorList;
                    }
                }
                else if (getUser == null)
                {
                    request.Error = true;
                    string message = "Invalid user!! Kindly sign in again";
                    errorList.Add(message);
                    request.Message = errorList;
                }
            }
            else if (rate > 3)
            {
                request.Error = true;
                string message = "Error";
                errorList.Add(message);
                request.Message = errorList;
            }
            request.Book = db.Books.Where(b => b.Id == getBook.Id && b.Delete == false).SingleOrDefault();
            return PartialView("_RateBook", request);
        }

        [HttpPost]
        [Authorize(Roles = "student, administrator")]
        public ActionResult ReserveBook(string id)
        {
            var errorList = new List<string>();
            var request = new MultipleModel.BooksVM();
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var getUser = db.Users.Where(u => u.Id == new Guid(getUserId) && u.Deleted == false).SingleOrDefault();

            var getBook = db.Books.Where(b => b.Id == new Guid(id) && b.Delete == false).SingleOrDefault();
            if (getUser != null)
            {
                if (getBook != null)
                {
                    if (!(db.Reservations.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id && c.Delete == false).Any()))
                    {
                        var checkUserReservation = db.Reservations.Where(r => r.UserId == getUser.Id && r.Delete == false).ToList();
                        if(checkUserReservation.Count <= 3)
                        {
                            var newReservation = db.Reservations.Create();
                            newReservation.User = getUser;
                            newReservation.Book = getBook;
                            newReservation.CreatedAt = DateTime.UtcNow;

                            db.Reservations.Add(newReservation);
                            db.SaveChanges();
                            ModelState.Clear();
                            request.Error = false;
                            string message = "You have successfully made a reservation";
                            errorList.Add(message);
                            request.Message = errorList;
                        }
                        else if(checkUserReservation.Count > 3)
                        {
                            request.Error = true;
                            string message = "You have already exceeded the maximumum number of reservation";
                            errorList.Add(message);
                            request.Message = errorList;
                        }
                    }
                    else if (db.Reservations.Where(c => c.UserId == getUser.Id && c.BookId == getBook.Id && c.Delete == false).Any())
                    {
                        request.Error = true;
                        string message = "You have already made a reservation for this book";
                        errorList.Add(message);
                        request.Message = errorList;
                    }
                }
                else if (getBook == null)
                {
                    request.Error = true;
                    string message = "The selected book is not existing!!";
                    errorList.Add(message);
                    request.Message = errorList;
                }
            }
            else if (getUser == null)
            {
                request.Error = true;
                string message = "Invalid user!! Kindly sign in again";
                errorList.Add(message);
                request.Message = errorList;
            }
            request.Book = db.Books.Where(b => b.Id == getBook.Id && b.Delete == false).SingleOrDefault();
            return PartialView("_RateBook", request);
        }

        [HttpGet]
        public ActionResult RecommendBookForm (Guid id)
        {
            var model = new MultipleModel.RecommendBookVM();
            model.Book = db.Books.Where(b => b.Id == id).Where(b => b.Delete == false).SingleOrDefault();
            return PartialView("_RecommendBookForm", model);
        }

        [HttpPost]
        public ActionResult RecommendBook(MultipleModel.RecommendBookVM req)
        {
            var model = new MultipleModel.RecommendBookVM();
            var messageList = new List<string>();
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            model.Error = true;
            if (ModelState.IsValid)
            {
                var getUser = db.Users.Where(u => u.Email == req.Recommend.Email).Where(u => u.Deleted == false).SingleOrDefault();
                if(getUser != null)
                {
                    if(!(db.BookRecommendations.Where(b => b.FromUserId == new Guid(getUserId)).Where(b => b.ToUserId == getUser.Id).Where(b => b.BookId == req.Recommend.BookId).Where(b => b.Deleted == false).Any()))
                    {
                        var newRecommend = db.BookRecommendations.Create();
                        newRecommend.BookId = req.Recommend.BookId;
                        newRecommend.ToUserId = getUser.Id;
                        newRecommend.FromUserId = new Guid(getUserId);
                        newRecommend.CreatedAt = DateTime.UtcNow;
                        newRecommend.Deleted = false;
                        db.BookRecommendations.Add(newRecommend);
                        db.SaveChanges();

                        string message = "You have successfully recommend this book to " + getUser.Email;
                        model.Error = false;
                        messageList.Add(message);
                    }
                    else
                    {
                        string message = "You have already recommended this book to "+ req.Recommend.Email;
                        messageList.Add(message);
                    }
                }
                else
                {
                    string message = "The user is not existing!!";
                    messageList.Add(message);
                }
            }
            else
            {
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            model.Message = messageList;
            model.Book = db.Books.Where(b => b.Id == req.Recommend.BookId).Where(b => b.Delete == false).SingleOrDefault();
            return PartialView("_RecommendBookForm", model);
        }
        public JsonResult EmailAutoComplete(string term)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var email = db.Users.Where(s => s.Email.ToLower().Contains(term.ToLower())).ToList();
            return Json(email, JsonRequestBehavior.AllowGet);
        }
    }
}
