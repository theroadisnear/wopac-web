namespace library_prototype.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SimpleCrypto;
    using System.Collections.Generic;
    using library_prototype.DAL;
    using library_prototype.CustomClass;

    internal sealed class Configuration : DbMigrationsConfiguration<library_prototype.DAL.LibraryDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(library_prototype.DAL.LibraryDbContext context)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            var encrypPass = crypto.Compute("rodnerraymundo");
            string pin = RandomPassword.Generate(6, PasswordGroup.Lowercase, PasswordGroup.Lowercase, PasswordGroup.Numeric);
            var cryptoPin = new SimpleCrypto.PBKDF2();
            var encrypPin = crypto.Compute(pin);

            var grades = new List<library_prototype.DAL.LibraryDbContext.GradesModel>
            {
                new library_prototype.DAL.LibraryDbContext.GradesModel
                {
                    Grade = "Administrator", CreatedAt = DateTime.UtcNow,
                    Sections = new List<library_prototype.DAL.LibraryDbContext.SectionsModel>
                    {
                        context.Sections.SingleOrDefault(s=>s.Section == "Developer")
                    }
                }
            };
            grades.ForEach(g => context.Grades.AddOrUpdate(g));

            var sections = new List<library_prototype.DAL.LibraryDbContext.SectionsModel>
            {
                new library_prototype.DAL.LibraryDbContext.SectionsModel
                {
                    Section = "Developer", CreatedAt = DateTime.UtcNow,
                    
                }
            };
            sections.ForEach(s => context.Sections.AddOrUpdate(s));

            var addresses = new List<library_prototype.DAL.LibraryDbContext.StudentAddressModel>
            {
                new DAL.LibraryDbContext.StudentAddressModel
                {
                    Address1 = "Lumang Dito", Address2 = "Banda Rito", City = "Pineapple City",
                    Country = "Philippines", CreatedAt = DateTime.UtcNow, ZipCode=1234
                },
                new DAL.LibraryDbContext.StudentAddressModel
                {
                    Address1="Matuwid na Daan", Address2="Pork Doon", City="Apple City", Country="Philippines",
                    CreatedAt =DateTime.UtcNow, ZipCode=5678
                },
                new DAL.LibraryDbContext.StudentAddressModel
                {
                    Address1="Dating Dito", Address2="Banda Doon", City="Pineapple City", Country="Philippines",
                    CreatedAt =DateTime.UtcNow, ZipCode=9012
                }
            };
            addresses.ForEach(a => context.StudentAddresses.AddOrUpdate(a));
            context.SaveChanges();

            var accounts = new List<library_prototype.DAL.LibraryDbContext.UserModel>
            {

                new library_prototype.DAL.LibraryDbContext.UserModel
                {
                    Email = "administrator@gmail.com",
                    Password = encrypPass, PasswordSalt = crypto.Salt, Pincode = encrypPin, PincodeSalt = cryptoPin.Salt,
                    Role = "administrator", SecretQuestion = "Who are you?", SecretAnswer = "rodnerraymundo",
                    CreatedAt = DateTime.UtcNow, Status = true,
                    Student = new DAL.LibraryDbContext.StudentModel
                    {
                        FirstName = "Rodner", MiddleInitial = "A", LastName = "Raymundo", Status = true, Birthday=DateTime.UtcNow.AddYears(-20),
                        ContactNumber = "09176508082", CreatedAt =DateTime.UtcNow, Gender = "male",
                        StudentAddress = context.StudentAddresses.SingleOrDefault(a=>a.ZipCode == 9012),
                        Section = context.Sections.SingleOrDefault(s=>s.Section =="Developer")
                    }
                },
                new library_prototype.DAL.LibraryDbContext.UserModel
                {
                    Email = "staff@gmail.com",
                    Password = encrypPass, PasswordSalt = crypto.Salt, Pincode = encrypPin, PincodeSalt = cryptoPin.Salt,
                    Role = "staff", SecretQuestion = "Who are you?", SecretAnswer = "rodnerraymundo",
                    CreatedAt = DateTime.UtcNow, Status = true,
                    Student = new DAL.LibraryDbContext.StudentModel
                    {
                        FirstName = "Kevin", MiddleInitial = "G", LastName = "Tiu", Status = true, Birthday = DateTime.UtcNow.AddYears(-20),
                        ContactNumber = "09176508082", CreatedAt = DateTime.UtcNow, Gender = "male",
                        StudentAddress = context.StudentAddresses.SingleOrDefault(a=>a.ZipCode == 5678),
                        Section = context.Sections.SingleOrDefault(s=>s.Section =="Developer")
                    }
                },
                new library_prototype.DAL.LibraryDbContext.UserModel
                {
                    Email = "student@gmail.com",
                    Password = encrypPass, PasswordSalt = crypto.Salt, Pincode = encrypPin, PincodeSalt = cryptoPin.Salt,
                    Role = "student", SecretQuestion = "Who are you?", SecretAnswer = "rodnerraymundo",
                    CreatedAt = DateTime.UtcNow, Status = true,
                    Student = new DAL.LibraryDbContext.StudentModel
                    {
                        FirstName = "Jake", MiddleInitial = "S", LastName = "Arroyo", Status = true, Birthday=DateTime.UtcNow.AddYears(-15),
                        ContactNumber = "09176508082", CreatedAt = DateTime.UtcNow, Gender = "male",
                        StudentAddress = context.StudentAddresses.SingleOrDefault(a=>a.ZipCode == 1234),
                        Section = context.Sections.SingleOrDefault(s=>s.Section =="Developer")
                    }
                },
            };
            accounts.ForEach(a => context.Users.AddOrUpdate(a));
            try
            {
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            var publishers = new List<DAL.LibraryDbContext.PublisherModel>
            {
                new DAL.LibraryDbContext.PublisherModel
                {
                    PublisherName = "Kewl Publisher", CreatedAt = DateTime.UtcNow,
                }
            };
            publishers.ForEach(p => context.Publishers.AddOrUpdate(p));

            var subjects = SubjectSeeder.Subject();
            subjects.ForEach(s => context.Subjects.AddOrUpdate(s));

            context.SaveChanges();

            var books = new List<library_prototype.DAL.LibraryDbContext.BookModel>
            {
                new library_prototype.DAL.LibraryDbContext.BookModel
                {
                    Title = "Discrete Mathematics for Kids", ISBN = "978-971-95546-0-8", Copyright = new DateTime(2012, 1, 1),
                    NoOfPages = 215, Price = 165.00, Quantity = 2, Synopsis = "This book is for students who failed Discrete Mathematics",
                    Borrow = true, CreatedAt = DateTime.UtcNow, Volume="1",
                    Subject = context.Subjects.SingleOrDefault(s=>s.CallNo == 001),
                    Publisher = context.Publishers.SingleOrDefault(p=>p.PublisherName == "Kewl Publisher")
                }
            };
            books.ForEach(b=>context.Books.AddOrUpdate(b));

            var authors = new List<library_prototype.DAL.LibraryDbContext.AuthorModel>
            {
                new library_prototype.DAL.LibraryDbContext.AuthorModel
                {
                    LastName = "Gonzales", FirstName = "George", MiddleInitial = "A",
                }
            };
            authors.ForEach(a=>context.Authors.AddOrUpdate(a));
            
            var booksauthors = new List<library_prototype.DAL.LibraryDbContext.BookAuthorModel>
            {
                new library_prototype.DAL.LibraryDbContext.BookAuthorModel
                {
                    Book = context.Books.SingleOrDefault(b=>b.Title == "Discrete Mathematics for Kids"),
                    Author = context.Authors.SingleOrDefault(a=>a.LastName == "Gonzales"),
                }
            };
            booksauthors.ForEach(b=>context.BooksAuthors.AddOrUpdate(b));

            context.SaveChanges();

            var emailCredential = new List<DAL.LibraryDbContext.EmailCredentialModel>
            {
                new DAL.LibraryDbContext.EmailCredentialModel
                {
                    Host = "smtp.sendgrid.net",
                    Username = "azure_324d8597ef63a3693116719bfa792c04@azure.com",
                    Password = CustomEncrypt.Encrypt("bg5PSAAPof9L2TW"),
                    CreatedAt = DateTime.UtcNow,
                    Deleted = false,
                    EmailMessages = new List<DAL.LibraryDbContext.EmailMessageModel>
                    {
                        new DAL.LibraryDbContext.EmailMessageModel
                        {
                            Type="notification", From="stvpslibrary@gmail.com", Subject="Book Deadline",
                            Body ="This is a reminder that your borrowed book's deadline is coming near. We urge you to return the book on or before it's deadline. Thank you",
                            CreatedAt =DateTime.UtcNow, Deleted = false,
                        },
                        new DAL.LibraryDbContext.EmailMessageModel
                        {
                            Type="accountpincode", From="stvpslibrary@gmail.com", Subject="Account Activation",
                            Body ="You have received because you are registered at Santo Tomas de Villanueva Parochial School Web and Android Online Public Access Catalog System. Otherwise please disregard this email.", CreatedAt=DateTime.UtcNow, Deleted = false,
                        }
                    }
                }
            };
            emailCredential.ForEach(e => context.EmailCredentials.AddOrUpdate(e));

            context.SaveChanges();
            /*var information = new List<library_prototype.DAL.LibraryDbContext.StudentModel>
            {
                new DAL.LibraryDbContext.StudentModel
                {
                    FirstName = "Rodner", MiddleInitial = "Y", LastName = "Raymundo", Status = true,
                    ContactNumber = "09176508082", CreatedAt = DateTime.UtcNow, Gender = "male",
                }
            };
            information.ForEach(i => context.Students.AddOrUpdate(i));
            */
            /*
            var sections = new List<library_prototype.DAL.LibraryDbContext.SectionsModel>
            {
                new DAL.LibraryDbContext.SectionsModel
                {
                    Section = "Administrator", CreatedAt = DateTime.UtcNow,
                },

                new DAL.LibraryDbContext.SectionsModel
                {
                    Section = "Co-Administrator", CreatedAt = DateTime.UtcNow,
                }
            };
            var nonStudentGroup = context.Grades.FirstOrDefault(g => g.Grade == "Non-student");
            sections.ForEach(s => nonStudentGroup.Sections.Add(s));
            context.SaveChanges();
            */
            base.Seed(context);
        }
    }
}
