using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using library_prototype.Models;
using library_prototype.DAL;
using SimpleCrypto;
using System.IO;
using library_prototype.CustomClass;
using System.Threading.Tasks;

namespace library_prototype.Controllers
{
    [Authorize(Roles = "administrator")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private LibraryDbContext db = new LibraryDbContext();

        public ActionResult Index()
        {
            var students = db.Students.ToList();
            return View(students);
        }

        [HttpPost]
        public ActionResult Index(string asd)
        {
            return View();
        }
        
        public ActionResult UpdateImage(Guid? id)
        {
            if (Request.Files.Count > 0)
            {
                var img = Request.Files[0];
                if (img == null)
                {
                    ModelState.AddModelError("ImageErrorMessage", "Image field is empty");
                }
                else if (img != null)
                {
                    string name = Guid.NewGuid().ToString() + "_" + Path.GetFileName(img.FileName);
                    string path = Path.Combine(Server.MapPath("~/Image/group_image"), name);
                    img.SaveAs(path);

                    var newImage = db.Images.Find(id);
                    newImage.Name = name;
                    newImage.Path = "~/Image/group_image/" + name;
                    newImage.UpdatedAt = DateTime.UtcNow;
                    
                    db.Entry(newImage).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View();
        }
        
        public ActionResult AddBook()
        {
            return View();
        }

        public ActionResult BookStatus()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserRegister()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AccountIndex()
        {
            return View();
        }

        public ActionResult ShowUser()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserInformation(Guid? id)
        {
            MultipleModel.UserInformationVM users = new MultipleModel.UserInformationVM();
            var userTempData = TempData["UserInformationTD"] as MultipleModel.UserInformationVM;
            
            if (userTempData != null)
            {
                users.Error = userTempData.Error;
                users.Message = userTempData.Message;
            }

            if (id != null)
            {
                var getUser = db.Users.SingleOrDefault(u => u.Id == id);
                
                if(getUser != null && getUser.Deleted != true)
                {
                    users.User = getUser;
                }
                else if(getUser != null && getUser.Deleted == true)
                {
                    return RedirectToAction("UserIndex", new { id = getUser.Student.SectionId});
                }
            }
            else
            {
                return RedirectToAction("GradesIndex", "Admin");
            }
            return View(users);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateUser(MultipleModel.UserInformationVM vm)
        {
            var errorList = new List<string>();
            string message = null;
            var user = db.Users.SingleOrDefault(u=>u.Id == vm.User.Id);
            if(user != null)
            {
                if(ModelState.IsValid)
                {
                    if((!db.Users.Where(u => u.Email == vm.NewUserInfo.EmailAddress).Any()) || user.Email == vm.NewUserInfo.EmailAddress)
                    {
                        user.Student.FirstName = vm.NewUserInfo.FirstName;
                        user.Student.MiddleInitial = vm.NewUserInfo.MiddleInitial;
                        user.Student.LastName = vm.NewUserInfo.LastName;
                        user.Student.Gender = vm.NewUserInfo.Gender;
                        user.Email = vm.NewUserInfo.EmailAddress;

                        user.UpdatedAt = DateTime.UtcNow;
                        user.Student.UpdatedAt = DateTime.UtcNow;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        vm.Error = false;
                        message = "You have successfully updated " + user.Student.LastName + " s information";
                        errorList.Add(message);
                    }
                    else if ((db.Users.Where(u => u.Email == vm.NewUserInfo.EmailAddress).Any()) || user.Email == vm.NewUserInfo.EmailAddress)
                    {
                        vm.Error = true;
                        message = vm.NewUserInfo.EmailAddress + " is already existing!!";
                        errorList.Add(message);
                    }
                }
                else
                {
                    vm.Error = true;
                    errorList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
                }
            }
            else if( user == null)
            {
                vm.Error = true;
                message = "The selected user doesn't exist";
                errorList.Add(message);
            }
            
            vm.Message = errorList;
            vm.User = db.Users.SingleOrDefault(u => u.Id == vm.User.Id);
            return PartialView("_UserInformation", vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteUser(MultipleModel.UserInformationVM vm)
        {
            var errorList = new List<string>();
            string message = null;
            var user = db.Users.SingleOrDefault(u => u.Id == vm.User.Id);
            if (user != null)
            {
                user.Email = user.Email + user.Id.ToString();
                user.Deleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                vm.Error = false;
                message = "You have successfully deleted " + user.Student.LastName + "'s account";
                errorList.Add(message);
                vm.Message = errorList;
                TempData["UserInformationTD"] = vm;
                
                return RedirectToAction("UserIndex", new { id = user.Student.SectionId });
            }
            vm.Error = true;
            message = "The selected user doesn't exist";
            errorList.Add(message);
            vm.Message = errorList;
            TempData["UserInformationTD"] = vm;

            return RedirectToAction("UserInformation", new { id = vm.User.Id });
        }

        [HttpGet]
        public ActionResult UserIndex(Guid? id)
        {
            var userVM = new MultipleModel.UserIndexVM();
            userVM.Error = null;
            var section = db.Sections.Find(id);
            userVM.GroupID = section.GradeId;
            userVM.SectionID = id;
            userVM.SectionName = section.Section;
            var getUsers = db.Users.Where(u=>u.Student.SectionId == id).Where(u => u.Deleted == false).OrderBy(u=>u.Student.Gender)
                .ThenBy(u=>u.Student.LastName).ToList();
            userVM.Users = getUsers;
            var tempData = TempData["UserIndexTD"] as MultipleModel.UserIndexVM;
            var userInformation = TempData["UserInformationTD"] as MultipleModel.UserInformationVM;

            if (tempData != null)
            {
                userVM.Error = tempData.Error;
                userVM.Message = tempData.Message;
                if(tempData.Error == false)
                {
                    userVM.Register = null;
                }
                else if (tempData.Error == true)
                {
                    userVM.Register = tempData.Register;
                }
            }
            else if(userInformation != null)
            {
                userVM.Error = userInformation.Error;
                userVM.Message = userInformation.Message;
            }
            return View(userVM);
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateUser(MultipleModel.UserIndexVM reg)
         {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(u => u.Email == reg.Register.EmailAddress).Any())
                {
                    reg.Error = true;
                    var errorList = new List<string>();
                    string message = "Email " + reg.Register.EmailAddress + " is already existing";
                    errorList.Add(message);
                    reg.Message = errorList;
                }
                else
                {
                    using (var db = new LibraryDbContext())
                    {
                        var newUser = db.Users.Create();
                        string pin = RandomPassword.Generate(6, PasswordGroup.Lowercase, PasswordGroup.Lowercase, PasswordGroup.Numeric);
                        var crypto = new PBKDF2();
                        var encrypPin = crypto.Compute(pin);

                        newUser.Pincode = encrypPin;
                        newUser.PincodeSalt = crypto.Salt;

                        newUser.Email = reg.Register.EmailAddress;
                        newUser.Role = "student";
                        newUser.CreatedAt = DateTime.UtcNow;
                        newUser.UpdatedAt = DateTime.UtcNow;
                        db.Users.Add(newUser);
                        var newStudent = db.Students.Create();
                        var section = db.Sections.FirstOrDefault(s => s.Id == reg.SectionID);
                        newStudent.SectionId = section.Id;
                        newStudent.FirstName = reg.Register.FirstName;
                        newStudent.MiddleInitial = reg.Register.MiddleInitial;
                        newStudent.LastName = reg.Register.LastName;
                        newStudent.Gender = reg.Register.Gender;
                        newStudent.CreatedAt = DateTime.UtcNow;
                        newStudent.UpdatedAt = DateTime.UtcNow;

                        db.Students.Add(newStudent);
                        db.SaveChanges();

                        SMTP smtp = new SMTP();
                        smtp.SendEmailPinCode(newUser.Email, pin);

                        var errorList = new List<string>();
                        string message = "You have successfully added a user " + newUser.Email;
                        errorList.Add(message);
                        reg.Error = false;
                        reg.Message = errorList;
                    }
                }

            }
            else
            {
                reg.Error = true;
                reg.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            
            reg.Users = db.Users.Include(u => u.Student).Where(u => u.Student.SectionId == reg.SectionID).Where(u => u.Deleted == false).OrderBy(u => u.Student.Gender)
                .ThenBy(u => u.Student.LastName).ToList();
            return PartialView("_UserList", reg);
        }

        [HttpGet]
        public ActionResult GradesIndex()
        {
            MultipleModel.CreateGradeVM grades = new MultipleModel.CreateGradeVM();
            grades.Grades = db.Grades.OrderBy(g => g.Grade).Where(g => g.Delete == false).ToList();

            var groupCreate = TempData["AddGroup"] as MultipleModel.CreateGradeVM;

            if (groupCreate != null)
            {
                grades.CreateGrade = groupCreate.CreateGrade;
                grades.Error = groupCreate.Error;
                grades.Message = groupCreate.Message;
            }

            return View(grades);
        }

        [HttpPost]
        public ActionResult CreateGroup(MultipleModel.CreateGradeVM vm)
        {
            if (ModelState.IsValid)
            {
                using (var db = new LibraryDbContext())
                {
                    if (db.Grades.Where(u => u.Grade == vm.CreateGrade.Grade).Any())
                    {
                        vm.Error = true;
                        var errorList = new List<string>();
                        string message = "Grade " + vm.CreateGrade.Grade + " is already existing";
                        errorList.Add(message);
                        vm.Message = errorList;

                        TempData["AddGroup"] = vm;
                        return RedirectToAction("GradesIndex", "Admin");
                    }
                    else
                    {
                        var newGroup = db.Grades.Create();
                        newGroup.Grade = vm.CreateGrade.Grade;
                        newGroup.CreatedAt = DateTime.UtcNow;
                        if (Request.Files.Count > 0)
                        {
                            var img = Request.Files[0];
                            if ((img != null) && (img.FileName == null))
                            {
                                string name = Guid.NewGuid().ToString() + "_" + Path.GetFileName(img.FileName);
                                string path = Path.Combine(Server.MapPath("~/Image/group_image"), name);
                                img.SaveAs(path);

                                var image = newGroup.Image;
                                image.Name = name;
                                image.Path = "~/Image/group_image/" + name;
                                image.CreatedAt = DateTime.UtcNow;

                            }
                        }

                        db.Grades.Add(newGroup);
                        db.SaveChanges();

                        vm.Error = false;
                        var errorList = new List<string>();
                        string message = "You have successfully added a group(" + newGroup.Grade + ")";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["AddGroup"] = vm;

                        return RedirectToAction("GradesIndex", "Admin");
                    }
                }
            }
            vm.Error = true;
            vm.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["AddGroup"] = vm;
            return RedirectToAction("GradesIndex", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGroup(MultipleModel.CreateGradeVM vm)
        {
            if(ModelState.IsValid)
            {
                var getGroup = db.Grades.Find(vm.DeleteGrade.GroupId);
                if(getGroup !=null)
                {
                    getGroup.Delete = true;
                    getGroup.UpdatedAt = DateTime.UtcNow;
                    db.Entry(getGroup).State = EntityState.Modified;
                    db.SaveChanges();

                    vm.Error = false;
                    var errorList = new List<string>();
                    string message = "You have successfully deleted (" + getGroup.Grade + ")";
                    errorList.Add(message);
                    vm.Message = errorList;
                    TempData["AddGroup"] = vm;
                    return RedirectToAction("GradesIndex", "Admin");
                }
                else if(getGroup == null)
                {
                    vm.Error = true;
                    var errorList = new List<string>();
                    string message = "Deletion of selected group failed. The selected group does not exist!!";
                    errorList.Add(message);
                    vm.Message = errorList;
                    TempData["AddGroup"] = vm;
                    return RedirectToAction("GradesIndex", "Admin");
                }
            }
            vm.Error = true;
            vm.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["AddGroup"] = vm;
            return RedirectToAction("GradesIndex", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateGroup(MultipleModel.CreateGradeVM vm)
        {
            if(ModelState.IsValid)
            {
                var getGroup = db.Grades.Find(vm.UpdateGroup.GroupId);
                if (getGroup != null)
                {
                    if(!(db.Grades.Where(g => g.Grade == vm.UpdateGroup.GroupName).Any()))
                    {
                        getGroup.Grade = vm.UpdateGroup.GroupName;
                        getGroup.UpdatedAt = DateTime.UtcNow;
                        db.Entry(getGroup).State = EntityState.Modified;
                        db.SaveChanges();

                        vm.Error = false;
                        var errorList = new List<string>();
                        string message = "You have successfully updated (" + getGroup.Grade + ")";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["AddGroup"] = vm;
                        return RedirectToAction("GradesIndex", "Admin");
                    }
                    else if (db.Grades.Where(g => g.Grade == vm.UpdateGroup.GroupName).Any())
                    {
                        vm.Error = true;
                        var errorList = new List<string>();
                        string message = "Failed to update "+getGroup.Grade +". "+ vm.UpdateGroup.GroupName+" is already used";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["AddGroup"] = vm;
                        return RedirectToAction("GradesIndex", "Admin");
                    }
                }
                else if (getGroup == null)
                {
                    vm.Error = true;
                    var errorList = new List<string>();
                    string message = "Failed to update the selected group. It does not exist!!";
                    errorList.Add(message);
                    vm.Message = errorList;
                    TempData["AddGroup"] = vm;
                    return RedirectToAction("GradesIndex", "Admin");
                }
            }
            vm.Error = true;
            vm.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["AddGroup"] = vm;
            return RedirectToAction("GradesIndex", "Admin");
        }

        [HttpGet]
        public ActionResult SectionIndex(Guid? id)
        {

            MultipleModel.CreateSectionVM vm = new MultipleModel.CreateSectionVM();
            vm.GroupID = id;
            vm.Sections = db.Sections.Where(s => s.GradeId == id).Where(s=>s.Deleted == false).ToList();
            var getSectionName = db.Grades.Find(id);
            vm.SectionName = getSectionName.Grade;

            var sectionCreate = TempData["AddSection"] as MultipleModel.CreateSectionVM;

            if (sectionCreate != null)
            {
                vm.CreateSection = sectionCreate.CreateSection;
                vm.Error = sectionCreate.Error;
                vm.Message = sectionCreate.Message;
            }

            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateSection(MultipleModel.CreateSectionVM vm)
        {
            if (ModelState.IsValid)
            {
                using (var db = new LibraryDbContext())
                {
                    if (db.Sections.Where(u => u.Section == vm.CreateSection.Section).Any())
                    {
                        vm.Error = true;
                        var errorList = new List<string>();
                        string message = "Section " + vm.CreateSection.Section + " is already existing";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["AddSection"] = vm;
                        return RedirectToAction("SectionIndex", new { id = vm.GroupID });
                    }
                    else
                    {
                        var grade = db.Grades.Find(vm.GroupID);
                        var section = db.Sections.Create();
                        section.Section = vm.CreateSection.Section;
                        section.CreatedAt = DateTime.UtcNow;
                        grade.Sections.Add(section);
                        db.SaveChanges();

                        vm.Error = false;
                        var errorList = new List<string>();
                        string message = "You have successfully added a section(" + vm.CreateSection.Section + ")";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["UserRegistration"] = vm;

                        return RedirectToAction("SectionIndex", new { id = section.GradeId });
                    }
                }
            }
            vm.Error = true;
            vm.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["AddSection"] = vm;
            return RedirectToAction("SectionIndex", new { id = vm.GroupID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSection(MultipleModel.CreateSectionVM vm)
        {
            if (ModelState.IsValid)
            {
                var getSection = db.Sections.Find(vm.DeleteSection.SectionId);
                if (getSection != null)
                {
                    getSection.Deleted = true;
                    getSection.UpdatedAt = DateTime.UtcNow;
                    db.Entry(getSection).State = EntityState.Modified;
                    db.SaveChanges();

                    vm.Error = false;
                    var errorList = new List<string>();
                    string message = "You have successfully deleted (" + getSection.Section + ")";
                    errorList.Add(message);
                    vm.Message = errorList;
                    TempData["AddSection"] = vm;
                    return RedirectToAction("SectionIndex", new { id = vm.GroupID});
                }
                else if (getSection == null)
                {
                    vm.Error = true;
                    var errorList = new List<string>();
                    string message = "Deletion of selected section failed. The selected section does not exist!!";
                    errorList.Add(message);
                    vm.Message = errorList;
                    TempData["AddSection"] = vm;
                    return RedirectToAction("SectionIndex", new { id = vm.GroupID });
                }
            }
            vm.Error = true;
            vm.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["AddSection"] = vm;
            return RedirectToAction("SectionIndex", new { id = vm.GroupID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSection(MultipleModel.CreateSectionVM vm)
        {
            if (ModelState.IsValid)
            {
                var getSection = db.Sections.Find(vm.UpdateSection.SectionId);
                if (getSection != null)
                {
                    if(!(db.Sections.Where(s => s.Section == vm.UpdateSection.SectionName).Any()))
                    {
                        getSection.Section = vm.UpdateSection.SectionName;
                        getSection.UpdatedAt = DateTime.UtcNow;
                        db.Entry(getSection).State = EntityState.Modified;
                        db.SaveChanges();

                        vm.Error = false;
                        var errorList = new List<string>();
                        string message = "You have successfully updated (" + getSection.Section + ")";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["AddSection"] = vm;
                        return RedirectToAction("SectionIndex", new { id = vm.GroupID });
                    }
                    else if (db.Sections.Where(s => s.Section == vm.UpdateSection.SectionName).Any())
                    {
                        vm.Error = true;
                        var errorList = new List<string>();
                        string message = "Failed to update " + getSection.Section + ". " + vm.UpdateSection.SectionName + " is already used";
                        errorList.Add(message);
                        vm.Message = errorList;
                        TempData["AddSection"] = vm;
                        return RedirectToAction("SectionIndex", new { id = vm.GroupID });
                    }
                }
                else if (getSection == null)
                {
                    vm.Error = true;
                    var errorList = new List<string>();
                    string message = "Failed to updated the selected section. It does not exist!!";
                    errorList.Add(message);
                    vm.Message = errorList;
                    TempData["AddSection"] = vm;
                    return RedirectToAction("SectionIndex", new { id = vm.GroupID });
                }
            }
            vm.Error = true;
            vm.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["AddSection"] = vm;
            return RedirectToAction("SectionIndex", new { id = vm.GroupID });
        }

        public ActionResult LibraryIndex()
        {
            try
            {
                var vm = new MultipleModel.LibraryIndexVM();
                var tempData = TempData["LibraryIndexTD"] as MultipleModel.LibraryIndexVM;
                vm.DbBooks = db.Books.ToList();
                vm.AddAuthors.Add(new AddAuthor { });
                if (tempData != null)
                {
                    if (vm.Error == true)
                    {
                        vm.CreateBook = tempData.CreateBook;
                    }
                    vm.Error = tempData.Error;
                    vm.Message = tempData.Message;
                }
                return View(vm);
            }
            catch(IOException)
            {
                return RedirectToAction("Login", "Auth");
            }
        }   
        
        public ActionResult BookInformation(Guid id)
        {
            var model = new MultipleModel.BookInformationVM();
            model.Book = db.Books.Where(b => b.Id == id && b.Delete == false).SingleOrDefault();
            return View(model);
        }

        [HttpGet]
        public ActionResult GetBookHistory(Guid id)
        {
            var model = new MultipleModel.BookInformationVM();
            model.Book = db.Books.Where(b => b.Id == id).Where(b=>b.Delete == false).SingleOrDefault();
            return PartialView("_BookHistory", model);
        }

        [HttpGet]
        public ActionResult GetBookComment(Guid id)
        {
            var model = new MultipleModel.BookInformationVM();
            model.Book = db.Books.Where(b => b.Id == id).Where(b => b.Delete == false).SingleOrDefault();
            return PartialView("_BookComment", model);
        }

        [HttpPost]
        public ActionResult DeleteBookComment(Guid id)
        {
            var model = new MultipleModel.BookInformationVM();
            var messageList = new List<string>();
            var getComment = db.Comments.Where(c => c.Id == id).Where(c => c.Delete == false).SingleOrDefault();
            try
            {
                getComment.Delete = true;
                getComment.UpdatedAt = DateTime.UtcNow;
                db.Entry(getComment).State = EntityState.Modified;
                db.SaveChanges();
                model.Error = false;
                string message = "You have successfully deleted a comment";
                messageList.Add(message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            model.Message = messageList;
            model.Book = db.Books.Where(b => b.Id == getComment.BookId).SingleOrDefault();
            return PartialView("_BookComment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBook(MultipleModel.LibraryIndexVM vm)
        {
            var messageList = new List<string>();
            if(ModelState.IsValid)
            {
                if(!(db.Books.Where(b => b.Title == vm.CreateBook.BookTitle).Any()) && !(db.Books.Where(b => b.ISBN == vm.CreateBook.ISBN).Any()))
                 {
                    var publisher = db.Publishers.Where(p => p.PublisherName == vm.CreateBook.PublisherName).SingleOrDefault();
                    var subject = db.Subjects.Where(s => s.CallNo == vm.CreateBook.CallNumber && s.Subject == vm.CreateBook.Subject).SingleOrDefault();
                    var authorList = new List<LibraryDbContext.AuthorModel>();
                    foreach(var author in vm.AddAuthors)
                    {
                        var checkAuthor = db.Authors.Where(a => a.LastName == author.LastName && a.FirstName == author.FirstName
                         && a.MiddleInitial == author.MiddleInitial).SingleOrDefault();
                        if (checkAuthor == null)
                        {
                            var newAuthor = db.Authors.Create();
                            newAuthor.LastName = author.LastName;
                            newAuthor.FirstName = author.FirstName;
                            newAuthor.MiddleInitial = author.MiddleInitial;

                            newAuthor.CreatedAt = DateTime.UtcNow;
                            db.Authors.Add(newAuthor);
                            string message = "You have successfully created new author " + newAuthor.LastName + ", " + newAuthor.FirstName + " " + newAuthor.MiddleInitial + ".";
                            messageList.Add(message);
                            db.SaveChanges();
                            authorList.Add(newAuthor);
                        }
                        else if(checkAuthor != null)
                        {
                            authorList.Add(checkAuthor);
                        }
                    }
                    if(publisher == null)
                    {
                        var newPublisher = db.Publishers.Create();
                        newPublisher.PublisherName = vm.CreateBook.PublisherName;
                        newPublisher.CreatedAt = DateTime.UtcNow;
                        db.Publishers.Add(newPublisher);
                        string message = "You have successfully created new publisher " + newPublisher.PublisherName;
                        messageList.Add(message);
                        db.SaveChanges();
                        publisher = newPublisher;
                    }
                    if(subject != null)
                    {
                        var newBook = db.Books.Create();
                        newBook.Title = vm.CreateBook.BookTitle;
                        newBook.ISBN = vm.CreateBook.ISBN;
                        newBook.Volume = vm.CreateBook.Volume.ToString();
                        newBook.Copyright = new DateTime(Convert.ToInt32(vm.CreateBook.CopyRight), 1, 1);
                        newBook.Quantity = vm.CreateBook.Quantity;
                        newBook.Price = vm.CreateBook.Price;
                        newBook.Borrow = vm.CreateBook.Borrow;
                        newBook.Synopsis = vm.CreateBook.Synopsis;
                        newBook.CreatedAt = DateTime.UtcNow;
                        db.Books.Add(newBook);
                        newBook.Subject = subject;
                        newBook.Publisher = publisher;
                        foreach(var author in authorList)
                        {
                            author.BooksAuthors.Add(new LibraryDbContext.BookAuthorModel { Author = author, Book = newBook});
                        }
                        db.SaveChanges();
                        vm = new MultipleModel.LibraryIndexVM();
                        string message = "You have successfully created new book" + newBook.Title;
                        messageList.Add(message);
                        vm.Error = false;
                    }
                }
                else if((db.Books.Where(b => b.Title == vm.CreateBook.BookTitle).Any()) && (db.Books.Where(b => b.ISBN == vm.CreateBook.ISBN).Any()))
                {
                    string message = "Title or ISBN is already existing!!";
                    messageList.Add(message);
                    vm.Error = true;
                }
            }
            else
            {
                vm.Error = true;
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            vm.Message = messageList;
            vm.DbBooks = db.Books.Where(b => b.Delete == false).OrderByDescending(b => b.CreatedAt).Take(10).ToList();
            return PartialView("_LibraryIndexBooksList", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(MultipleModel.BookInformationVM vm)
        {
            var tempData = new MultipleModel.LibraryIndexVM();
            var messageList = new List<string>();
            vm.Error = true;
            if(ModelState.IsValid)
            {
                var getBook = db.Books.Where(b => b.Id == vm.DeleteBook.BookId && b.Delete == false).SingleOrDefault();
                if(getBook != null)
                {
                    var getBookAuthors = db.BooksAuthors.Where(b => b.BookId == getBook.Id && b.Deleted == false).ToList();
                    foreach(var bookAuthor in getBookAuthors)
                    {
                        bookAuthor.Deleted = true;
                        db.Entry(bookAuthor).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    getBook.ISBN = getBook.ISBN + getBook.Id;
                    getBook.Delete = true;
                    db.Entry(getBook).State = EntityState.Modified;
                    db.SaveChanges();

                    string message = "You have successfully deleted the book";
                    messageList.Add(message);
                    vm.Error = false;
                }
                else if(getBook == null)
                {
                    string message = "The book is not existing!!";
                    messageList.Add(message);
                }
            }
            else
            {
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }

            vm.Message = messageList;

            TempData["LibraryIndexTD"] = vm;
            tempData.Error = vm.Error;
            tempData.Message = vm.Message;
            TempData["LibraryIndexTD"] = tempData;
            return RedirectToAction("LibraryIndex");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBook(MultipleModel.BookInformationVM model)
        {
            var messageList = new List<string>();
            model.Error = true;
            if(ModelState.IsValid)
            {
                var getBook = db.Books.Where(b => b.Id == model.UpdateBook.BookId).SingleOrDefault();
                if(getBook != null)
                {
                    var getSubject = db.Subjects.Where(s => s.Subject == model.UpdateBook.Subject && s.CallNo == model.UpdateBook.CallNumber).SingleOrDefault();

                    if(getBook.ISBN == model.UpdateBook.ISBN)
                    {
                        if (getSubject != null)
                        {
                            if (getBook.Publisher.PublisherName != model.UpdateBook.PublisherName)
                            {
                                var newPublisher = db.Publishers.Create();
                                newPublisher = new LibraryDbContext.PublisherModel
                                {
                                    PublisherName = model.UpdateBook.PublisherName,
                                    CreatedAt = DateTime.UtcNow
                                };
                                db.Publishers.Add(newPublisher);
                                db.SaveChanges();
                                string pubMessage = "You have successfully created " + newPublisher.PublisherName;
                                messageList.Add(pubMessage);
                                getBook.Publisher = newPublisher;
                            }
                            getBook.Title = model.UpdateBook.BookTitle;
                            getBook.ISBN = model.UpdateBook.ISBN;
                            getBook.Volume = model.UpdateBook.Volume.ToString();
                            getBook.Copyright = new DateTime(Convert.ToInt32(model.UpdateBook.CopyRight), 1, 1);
                            getBook.Quantity = model.UpdateBook.Quantity;
                            getBook.Price = model.UpdateBook.Price;
                            getBook.Borrow = model.UpdateBook.Borrow;
                            getBook.Synopsis = model.UpdateBook.Synopsis;
                            getBook.UpdatedAt = DateTime.UtcNow;
                            getBook.Subject = getSubject;

                            db.Entry(getBook).State = EntityState.Modified;
                            db.SaveChanges();
                            string message = "You have successfully updated " + getBook.Title;
                            messageList.Add(message);
                            model.Error = false;
                        }
                        else if (getSubject == null)
                        {
                            string message = "Invalid subject!!";
                            messageList.Add(message);
                        }
                    }
                    else if(getBook.ISBN != model.UpdateBook.ISBN && !(db.Books.Where(b=>b.ISBN == model.UpdateBook.ISBN).Any()))
                    {
                        string message = "The books isbn is already existing!!";
                        messageList.Add(message);
                    }
                    
                }
                else if(getBook == null)
                {
                    string message = "The book is not exisiting!!";
                    messageList.Add(message);
                }
            }
            else
            {
                model.Error = true;
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            model.Book = db.Books.Where(b => b.Id == model.UpdateBook.BookId && b.Delete == false).SingleOrDefault();
            model.Message = messageList;
            return PartialView("_BookInformation", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateBookAuthor(MultipleModel.BookInformationVM model)
        {
            var messageList = new List<string>();
            model.Error = true;
            var getBook = db.Books.Where(b => b.Id == model.BookId && b.Delete == false).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if(getBook != null && model.UpdateAuthor != null)
                {
                    var authorList = new List<LibraryDbContext.AuthorModel>();
                    foreach (var author in model.UpdateAuthor)
                    {
                        var checkAuthor = db.Authors.Where(a => a.LastName == author.LastName && a.FirstName == author.FirstName
                         && a.MiddleInitial == author.MiddleInitial).SingleOrDefault();
                        if (checkAuthor == null)
                        {
                            var newAuthor = db.Authors.Create();
                            newAuthor.LastName = author.LastName;
                            newAuthor.FirstName = author.FirstName;
                            newAuthor.MiddleInitial = author.MiddleInitial;

                            newAuthor.CreatedAt = DateTime.UtcNow;
                            db.Authors.Add(newAuthor);
                            string wmessage = "You have successfully created new author " + newAuthor.LastName + ", " + newAuthor.FirstName + " " + newAuthor.MiddleInitial + ".";
                            messageList.Add(wmessage);
                            db.SaveChanges();
                            authorList.Add(newAuthor);
                        }
                        else if (checkAuthor != null)
                        {
                            authorList.Add(checkAuthor);
                        }
                    }
                    db.BooksAuthors.Where(b => b.BookId == getBook.Id).ToList().ForEach(a=>a.Deleted =true);
                    db.SaveChanges();
                    foreach (var author in authorList)
                    {
                        if(!(db.BooksAuthors.Where(b => b.AuthorId == author.Id && b.BookId == getBook.Id).Any()))
                        {
                            var newBookAuthor = db.BooksAuthors.Create();
                            newBookAuthor.AuthorId = author.Id;
                            newBookAuthor.BookId = getBook.Id;
                            newBookAuthor.Deleted = false;
                            db.BooksAuthors.Add(newBookAuthor);
                            db.SaveChanges();
                        }
                        else if(db.BooksAuthors.Where(b => b.AuthorId == author.Id && b.BookId == getBook.Id).Any())
                        {
                            var getBookAuthor = db.BooksAuthors.Where(b => b.AuthorId == author.Id && b.BookId == getBook.Id).SingleOrDefault();
                            getBookAuthor.Deleted = false;
                            db.Entry(getBookAuthor).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    string message = "You have successfully updated the authors of " + getBook.Title;
                    messageList.Add(message);
                    model.Error = false;
                }
                else
                {
                    string mesage = "The book is not existing or You have not entered any author!!";
                    messageList.Add(mesage);
                }
            }
            else
            {
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            model.Book = db.Books.Where(b => b.Id == getBook.Id).SingleOrDefault();
            model.Message = messageList;
            return PartialView("_BookInformation", model);
        }

        [HttpGet]
        public ActionResult BorrowAndReturn()
        {
            var model = new MultipleModel.BorrowReturnDashBoardVM();
            model.BookLogs = db.BookLogs.Where(b => b.Return == false).ToList();
            model.Reservations = db.Reservations.Where(r => r.Delete == false).ToList();
            model.Subjects = db.Subjects.ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult BorrowReturnDashboard()
        {
            var model = new MultipleModel.BorrowReturnDashBoardVM();
            model.BookLogs = db.BookLogs.Where(b => b.Return == false).ToList();
            model.Reservations = db.Reservations.Where(r => r.Delete == false).ToList();
            model.Subjects = db.Subjects.ToList();
            return PartialView("_BorrowReturnDashboard", model);
        }

        [HttpGet]
        public ActionResult BorrowTab()
        {
            var getReservation = db.Reservations.Where(r => r.Delete == false).OrderByDescending(r => r.CreatedAt).Take(10).ToList();
            var model = new MultipleModel.BorrowTabVM();
            model.Reservation = getReservation;
            return PartialView("_BorrowTab" , model);
        }

        [HttpGet]
        public ActionResult ReturnTab()
        {
            var model = new MultipleModel.ReturnTabVM();
            model.BookLogs = db.BookLogs.Where(b=>b.Return == false).OrderByDescending(r => r.Deadline).Take(10).ToList();
            return PartialView("_ReturnTab", model);
        }

        [HttpGet]
        public ActionResult GiveBookModal(Guid id)
        {
            var model = new MultipleModel.BorrowTabVM();
            var getReservation = db.Reservations.Where(r => r.Id == id && r.Delete == false).ToList();
            model.Reservation = getReservation;
            return PartialView("_GiveBookModal", model);
        }

        [HttpPost]
        public ActionResult CancelReservation(Guid id)
        {
            var messageList = new List<string>();
            var model = new MultipleModel.BorrowTabVM();
            model.Error = true;
            var getReservation = db.Reservations.Where(r => r.Id == id && r.Delete == false).SingleOrDefault();
            if (getReservation != null)
            {
                getReservation.Delete = true;
                getReservation.UpdatedAt = DateTime.UtcNow;
                db.Entry(getReservation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                string message = "You have successfully canceled the reservation!!";
                messageList.Add(message);
                model.Error = false;
            }
            else if(getReservation == null)
            {
                string message = "Reservation not found!!";
                messageList.Add(message);
            }
            model.Message = messageList;
            model.Reservation = db.Reservations.Where(r => r.Delete == false).OrderByDescending(r => r.CreatedAt).Take(10).ToList();
            return PartialView("_BorrowTab", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GiveBook(MultipleModel.BorrowTabVM req)
        {
            var messageList = new List<string>();
            req.Error = true;
            if(ModelState.IsValid)
            {
                var getRervation = db.Reservations.Where(r => r.UserId == req.GiveBook.UserId && r.BookId == req.GiveBook.BookId && r.Delete == false).SingleOrDefault();
                var getBook = db.Books.Where(b => b.Id == req.GiveBook.BookId && b.Delete == false).SingleOrDefault();
                var getUser = db.Users.Where(u => u.Id == req.GiveBook.UserId && u.Deleted == false).SingleOrDefault();
                if(getBook != null)
                {
                    if(getUser != null)
                    {
                        var count = db.BookLogs.Where(b => b.BookId == getBook.Id && b.Return == false).Count();
                        if (getBook.Quantity >= count)
                        {
                            var newBookLog = db.BookLogs.Create();
                            newBookLog.Book = getBook;
                            newBookLog.User = getUser;
                            newBookLog.Deadline = req.GiveBook.Deadline;
                            newBookLog.CreatedAt = DateTime.UtcNow;
                            newBookLog.Return = false;

                            db.BookLogs.Add(newBookLog);
                            if(getRervation != null)
                            {
                                getRervation.Delete = true;
                                getRervation.UpdatedAt = DateTime.UtcNow;
                                db.Entry(getRervation).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            string message = "You have successfully borrowed the book. Enjoy!!";
                            messageList.Add(message);
                            req.Error = false;
                        }
                        else if(getBook.Quantity <= count)
                        {
                            string message = "There are no available book at this moment. Wait the current borrowers to return it";
                            messageList.Add(message);
                        }
                    }
                    else if(getUser == null)
                    {
                        string message = "The user is no longer existing!!";
                        messageList.Add(message);
                    }
                }
                else if(getBook == null)
                {
                    string message = "The book is no longer existing!!";
                    messageList.Add(message);
                }
            }
            else
            {
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            req.Message = messageList;
            req.Reservation = db.Reservations.Where(r => r.Delete == false).OrderByDescending(r => r.CreatedAt).Take(10).ToList();
            return PartialView("_BorrowTab", req);
        }

        [HttpGet]
        public ActionResult ReturnBookModal(Guid id)
        {
            var model = new MultipleModel.ReturnTabVM();
            model.BookLogs = db.BookLogs.Where(b => b.Id == id && b.Return == false).ToList();
            return PartialView("_ReturnBookModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnBook(MultipleModel.ReturnTabVM req)
        {
            var messageList = new List<string>();
            req.Error = true;
            if(ModelState.IsValid)
            {
                var getBookLog = db.BookLogs.Where(b => b.Id == req.ReturnBook.BookLogId && b.Return == false).SingleOrDefault();
                if(getBookLog != null)
                {
                    getBookLog.BookStatus = req.ReturnBook.BookStatus;
                    getBookLog.MessageLog = req.ReturnBook.MessageLog;
                    getBookLog.Return = true;
                    getBookLog.UpdatedAt = DateTime.UtcNow;

                    db.Entry(getBookLog).State = EntityState.Modified;
                    db.SaveChanges();
                    string message = "You have successfully returned the book";
                    messageList.Add(message);
                    req.Error = false;
                }
                else if(getBookLog == null)
                {
                    string message = "The book record is not existing!!";
                    messageList.Add(message);
                }
            }
            else
            {
                messageList = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            }
            req.Message = messageList;
            req.BookLogs = db.BookLogs.Where(b => b.Return == false).OrderByDescending(r => r.Deadline).Take(10).ToList();
            return PartialView("_ReturnTab", req);
        }

        [HttpGet]
        public ActionResult SMTPIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SMTPDashboard()
        {
            return PartialView("_SMTPDashboard");
        }

        [HttpGet]
        public ActionResult SMTPMessages()
        {
            var model = new MultipleModel.SMTPIndexVM();
            model.EmailMessages = db.EmailMessages.Where(e => e.Deleted == false).ToList();
            return PartialView("_SMTPMessages", model);
        }

        [HttpGet]
        public ActionResult SMTPCredentials()
        {
            var model = new MultipleModel.SMTPIndexVM();
            model.EmailCredentials = db.EmailCredentials.Where(e => e.Deleted == false).SingleOrDefault();
            return PartialView("_SMTPCredentials", model);
        }

        [HttpGet]
        public ActionResult ChartBRPercentage()
        {
            var model = new MultipleModel.BorrowReturnDashBoardVM();
            model.BookLogs = db.BookLogs.Where(b => b.Return == false).ToList();
            model.Reservations = db.Reservations.Where(r => r.Delete == false).ToList();
            return PartialView("_ChartBRPercentage", model);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AddAuthor()
        {
            return PartialView("_LibraryIndexCreateAuthors", new AddAuthor());
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult UpdateAuthor()
        {
            
            return PartialView("_LibraryIndexUpdateAuthors", new LibraryDbContext.AuthorModel());
        }

        public JsonResult SubjectAutoComplete(string term)
        {
            using (var db = new LibraryDbContext())
            {
                var subjectList = db.Subjects.Where(s => s.Subject.ToLower().Contains(term.ToLower()) ||
                    s.CallNo.ToString().Contains(term.ToLower())).ToList();
                return Json(subjectList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CallNumberAutoComplete(int term)
        {
            using (var db = new LibraryDbContext())
            {
                var subjectList = db.Subjects.Where(s => s.CallNo == term).SingleOrDefault();
                return Json(subjectList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PublisherAutoComplete(string term)
        {
            using (var db = new LibraryDbContext())
            {
                var publisherList = db.Publishers.Where(s => s.PublisherName.ToLower().Contains(term.ToLower())).ToList();
                return Json(publisherList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AuthorAutoComplete(string term, string term1)
        {
            using (var db = new LibraryDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var authorList = db.Authors.Where(a => a.LastName.ToLower().Contains(term.ToLower()) ||
                a.LastName.ToLower().Contains(term1.ToLower())).ToList();
                return Json(authorList, JsonRequestBehavior.AllowGet);
            }
        }
    }
    
}
