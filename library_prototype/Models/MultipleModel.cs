using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using library_prototype.DAL;
using library_prototype.Models;
namespace library_prototype.Models
{
    public class MultipleModel
    {
        public class AuthModelVM
        {
            public ActivationModel1 ActivationModel1 { get; set; }
            public LibraryDbContext.UserModel UserModel { get; set; }
            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class LoginModelVM
        {
            public AuthModel AuthModel { get; set; }
            public ActivationModel ActivationModel { get; set; }
            public LibraryDbContext.UserModel UserModel { get; set; }
            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class ProfileVM
        {

            public LibraryDbContext.UserModel UserModel { get; set; }
            public LibraryDbContext.StudentAddressModel StudentAddressModel { get; set; }
            public LibraryDbContext.StudentModel StudentModel { get; set; }
            public Models.ImageModel ImageModel { get; set; }
        }

        public class CreateGradeVM
        {
            public ICollection<LibraryDbContext.GradesModel> Grades { get; set; }
            public ICollection<LibraryDbContext.ImageModel> Images { get; set; }
            public ImageModel ImageModel { get; set; }
            public CreateGradeModel CreateGrade { get; set; }
            public DeleteGroup DeleteGrade { get; set; }
            public UpdateGroup UpdateGroup { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class CreateSectionVM
        {
            public ICollection<LibraryDbContext.SectionsModel> Sections { get; set; }
            public ICollection<LibraryDbContext.ImageModel> Images { get; set; }
            public ImageModel ImageModel { get; set; }
            public CreateSectionModel CreateSection { get; set; }
            public DeleteSection DeleteSection { get; set; }
            public UpdateSection UpdateSection { get; set; }

            public string SectionName { get; set; }
            public Guid? GroupID { get; set; }
            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class UserIndexVM
        {
            public ICollection<LibraryDbContext.UserModel> Users { get; set; }
            public RegisterModel Register { get; set; }
            public Guid? SectionID { get; set; }
            public Guid? GroupID { get; set; }
            public string SectionName { get; set; }
            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class UserInformationVM
        {
            public LibraryDbContext.UserModel User { get; set; }
            public LibraryDbContext.SectionsModel Section { get; set; }
            public UpdateUser NewUserInfo { get; set; }
            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class LibraryIndexVM
        {
            public LibraryIndexVM()
            {
                AddAuthors = new List<AddAuthor>();
            }
            public ICollection<LibraryDbContext.BookModel> DbBooks { get; set; }
            public ICollection<LibraryDbContext.AuthorModel> DbAuthors { get; set; }
            public CreateBook CreateBook { get; set; }
            public List<AddAuthor> AddAuthors { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class BooksVM
        {
            public ICollection<LibraryDbContext.BookModel> Books { get; set; }
            public LibraryDbContext.BookModel Book { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class CommentsVM
        {
            public LibraryDbContext.BookModel Book { get; set; }
            public AddNewCommentModel NewComment { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class RecommendBookVM
        {
            public LibraryDbContext.BookModel Book { get; set; }
            public RecommendBook Recommend { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class UserProfileVM
        {
            public LibraryDbContext.UserModel User { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class BookInformationVM
        {
            public LibraryDbContext.BookModel Book { get; set; }
            public UpdateBook UpdateBook { get; set; }
            public DeleteBook DeleteBook { get; set; }
            public List<AddAuthor> UpdateAuthor { get; set; }
            public Guid BookId { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class BorrowTabVM
        {
            public List<LibraryDbContext.ReservationModel> Reservation { get; set; }
            public GiveBook GiveBook { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class ReturnTabVM
        {
            public List<LibraryDbContext.BookLogsModel> BookLogs { get; set; }
            public ReturnBook ReturnBook { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }

        public class BorrowReturnDashBoardVM
        {
            public List<LibraryDbContext.BookLogsModel> BookLogs { get; set; }
            public List<LibraryDbContext.ReservationModel> Reservations { get; set; }
            public List<LibraryDbContext.SubjectModel> Subjects { get; set; }
        }

        public class SMTPIndexVM
        {
            public List<LibraryDbContext.EmailMessageModel> EmailMessages { get; set; }
            public LibraryDbContext.EmailCredentialModel EmailCredentials { get; set; }
            public UpdateEmailMessage UpdateEmailMessage { get; set; }

            public bool? Error { get; set; }
            public List<string> Message { get; set; }
        }
    }
}