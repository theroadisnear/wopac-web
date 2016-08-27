using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using library_prototype.Models;
using library_prototype.DAL;
using SimpleCrypto;
using System.Security.Claims;
using System.Net.Http;
using System.Data.Entity;
using library_prototype.CustomClass;
using System.Net;
namespace library_prototype.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        
        [HttpGet]
        public ActionResult Login()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            if( authManager.User.IsInRole("administrator") )
            {
                return RedirectToAction("GradesIndex", "Admin");
            }
            else if ( authManager.User.IsInRole("staff") )
            {
                return RedirectToAction("Books", "User");
            }
            else if (authManager.User.IsInRole("student"))
            {
                return RedirectToAction("Books", "User");
            }
            MultipleModel.LoginModelVM loginVM = new MultipleModel.LoginModelVM();
            var loginTD = TempData["LoginTD"] as MultipleModel.LoginModelVM;

            if(loginTD != null)
            {
                loginVM.Error = loginTD.Error;
                loginVM.Message = loginTD.Message;
            }

            return View(loginVM);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(MultipleModel.LoginModelVM user)
        {
            if (ModelState.IsValid)
            {
                using (var db = new LibraryDbContext())
                {
                    var crypto = new SimpleCrypto.PBKDF2();
                    var emailCheck = db.Users.FirstOrDefault(u => u.Email == user.AuthModel.Email);
                    var getPasswordSalt = db.Users.Where(u => u.Email == user.AuthModel.Email).Select(u => u.PasswordSalt);
                    
                    if ((emailCheck != null) && (getPasswordSalt != null) && (emailCheck.Deleted == false) && (emailCheck.Status == true))
                    {
                        var materializePasswordSalt = getPasswordSalt.ToList();
                        var passwordSalt = materializePasswordSalt[0];
                        var encryptedPassword = crypto.Compute(user.AuthModel.Password, passwordSalt);

                        if (user.AuthModel.Email != null && emailCheck.Password == encryptedPassword)
                        {
                            var name = emailCheck.Student.FirstName + " "+ emailCheck.Student.MiddleInitial + ". " + emailCheck.Student.LastName;

                            var getEmail = db.Users.Where(u => u.Id == emailCheck.Id).Select(u => u.Email);
                            var materializeEmail = getEmail.ToList();
                            var email = materializeEmail[0];

                            var getRole = db.Users.Where(u => u.Id == emailCheck.Id).Select(u => u.Role);
                            var materializeRole = getRole.ToList();
                            var role = materializeRole[0];

                            var identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, name),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Role, role),
                            new Claim(ClaimTypes.NameIdentifier, emailCheck.Id.ToString())
                        }, "ApplicationCookie");

                            var ctx = Request.GetOwinContext();
                            var authManager = ctx.Authentication;
                            authManager.SignIn(identity);

                            if (emailCheck.Role == "administrator")
                            {
                                return RedirectToAction("GradesIndex", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Books", "User");
                            }
                        }
                        else
                        {
                            user.Error = true;
                            ModelState.AddModelError("", "Invalid email or password");
                        }
                    }
                    else if((emailCheck != null) && (emailCheck.Status == false) && (emailCheck.Deleted == false) )
                    {
                        user.Error = true;
                        ModelState.AddModelError("", "Please activate the account");
                    }
                    else if(emailCheck == null || ((emailCheck.Deleted == true) && (emailCheck.Status == false)))
                    {
                        user.Error = true;
                        ModelState.AddModelError("", "Account does not exist");
                    }
                }
                
            }
            user.Error = true;
            return View(user);
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public ActionResult AccountActivationSaveData([Bind(Prefix ="Item2")] ActivationModel act)
        {
            var authModel = new AuthModel();
            var activationModel = new ActivationModel();
            var tuple = Tuple.Create(authModel, activationModel);
            tuple.Item2.Email = act.Email;
            tuple.Item2.PinCode = act.PinCode;
            //var gg= "<p class='uk-text-danger'>" + act.Email + "</p>";
            return Json(new { success = "true" },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Book()
        {
            return View();
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ActivateAccount(MultipleModel.LoginModelVM login)
        {
            if(ModelState.IsValid)
            {
                using (var db = new LibraryDbContext())
                {
                    var emailCheck = db.Users.Where(u => u.Email == login.ActivationModel.Email).ToList();
                    if(emailCheck[0] !=null)
                    {
                        var email = db.Users.SingleOrDefault(u => u.Email == login.ActivationModel.Email);
                        var crypto = new PBKDF2();
                        if ((email.Password != null) && (email.PasswordSalt != null))
                        {
                            login.Error = true;
                            ModelState.AddModelError("", "The account is already activated");
                            return View("Login", login);
                        }
                        else if ((email != null) && (email.Pincode == crypto.Compute(login.ActivationModel.PinCode, email.PincodeSalt)))
                        {
                            var ctx = Request.GetOwinContext();
                            var authManager = ctx.Authentication;

                            var identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, "acc_act"),
                            new Claim(ClaimTypes.Role, "activation")
                        }, "ApplicationCookie");

                            authManager.SignIn(identity);
                            return RedirectToAction("ActivateAccount2", new { id = email.Id });
                        }
                        else if ((email != null) && (email.Pincode != login.ActivationModel.PinCode))
                        {
                            login.Error = true;
                            ModelState.AddModelError("", "Incorrect pin entered");
                            return View("Login", login);
                        }
                    }
                    else if (emailCheck[0] == null)
                    {
                        login.Error = true;
                        ModelState.AddModelError("", "The account does not exist");
                    }
                    
                }
            }
            login.Error = true;
            return View("Login", login);
        }

        [Authorize(Roles = "activation")]
        [HttpGet]
        public ActionResult ActivateAccount2(Guid? id)
        {
            if (id.HasValue)
            {
                var db = new LibraryDbContext();
                MultipleModel.AuthModelVM vm = new MultipleModel.AuthModelVM();
                var userActivation = TempData["UserActivation"] as MultipleModel.AuthModelVM;

                if (userActivation != null)
                {
                    vm = userActivation;
                }

                vm.UserModel = db.Users.Where(u => u.Id == id && u.Deleted == false).SingleOrDefault();
                return View(vm);
            }

            return RedirectToAction("Login");
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ActivateAccount2(MultipleModel.AuthModelVM request)
        {
            if(ModelState.IsValid)
            {
                using (var db = new LibraryDbContext())
                {
                    MultipleModel.AuthModelVM vm = new MultipleModel.AuthModelVM();
                    vm.UserModel = db.Users.SingleOrDefault(u => u.Id == request.UserModel.Id);
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encrypPass = crypto.Compute(request.ActivationModel1.Password);

                    vm.UserModel.PasswordSalt = crypto.Salt;
                    vm.UserModel.Password = encrypPass;
                    vm.UserModel.SecretQuestion = request.ActivationModel1.SecretQuestion;
                    vm.UserModel.SecretAnswer = request.ActivationModel1.SecretAnswer;
                    vm.UserModel.Status = true;
                    vm.UserModel.Deleted = false;
                    vm.UserModel.UpdatedAt = DateTime.UtcNow;

                    vm.UserModel.Student.Birthday = request.ActivationModel1.Birthday;
                    vm.UserModel.Student.StudentAddress = new LibraryDbContext.StudentAddressModel
                    {
                        ZipCode = request.ActivationModel1.ZipCode, Address1 = request.ActivationModel1.Address1,
                        Address2 = request.ActivationModel1.Address2, City = request.ActivationModel1.City,
                        Country = request.ActivationModel1.Country, CreatedAt = DateTime.UtcNow, 
                    };
                    db.Entry(vm.UserModel).State = EntityState.Modified;
                    db.SaveChanges();

                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignOut("ApplicationCookie");

                    var loginVM = new MultipleModel.LoginModelVM();
                    loginVM.Error = false;
                    var errorList = new List<string>();
                    string message = "You have successfully activated your account. Please log in";
                    errorList.Add(message);
                    loginVM.Message = errorList;
                    TempData["LoginTD"] = loginVM;

                    return RedirectToAction("Login");
                }
            }
            request.Error = true;
            request.Message = CustomValidationMessage.GetErrorList(ViewData.ModelState);
            TempData["UserActivation"] = request;
            return RedirectToAction("ActivateAccount2", new { id = request.UserModel.Id });
        }

    }
}
