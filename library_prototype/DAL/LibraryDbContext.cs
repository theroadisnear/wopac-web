using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel;
using System.Xml.Serialization;

namespace library_prototype.DAL
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext() : base("LibraryDbContext")
        {
            //Database.SetInitializer<LibraryDbContext>(new LibraryDbSeed());
            //Database.SetInitializer<LibraryDbContext>(new CreateDatabaseIfNotExists<LibraryDbContext>());
            //Database.SetInitializer<LibraryDbContext>(new DropCreateDatabaseIfModelChanges<LibraryDbContext>());

        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<StudentAddressModel> StudentAddresses { get; set; }
        public DbSet<StudentModel> Students { get; set; }
        public DbSet<SectionsModel> Sections { get; set; }
        public DbSet<GradesModel> Grades { get; set; }
        public DbSet<BookModel> Books { get; set; }
        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<SubjectModel> Subjects { get; set; }
        public DbSet<PublisherModel> Publishers { get; set; }
        public DbSet<BookAuthorModel> BooksAuthors { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }
        public DbSet<BookLogsModel> BookLogs { get; set; }
        public DbSet<BookRecommendationsModel> BookRecommendations { get; set; }
        public DbSet<EmailCredentialModel> EmailCredentials { get; set; }
        public DbSet<EmailMessageModel> EmailMessages { get; set; }
        public DbSet<ImageModel> Images { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookRecommendationsModel>().HasRequired(r => r.FromUser).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<BookRecommendationsModel>().HasRequired(r => r.ToUser).WithMany().WillCascadeOnDelete(false);
        }
        
        [Table("Users")]
        public class UserModel
        {
            [Column(Order = 0), Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("Student")]
            public Guid StudentId { get; set; }
            [ForeignKey("Image")]
            public Guid? ImageId { get; set; }
            [StringLength(450)]
            [Index(IsUnique = true)]
            public string Email { get; set; }
            public string Password { get; set; }
            public string PasswordSalt { get; set; }
            public string Pincode { get; set; }
            public string PincodeSalt { get; set; }
            public string Role { get; set; }
            public string SecretQuestion { get; set; }
            public string SecretAnswer { get; set; }
            [DefaultValue(false)]
            public bool Deleted { get; set; }
            public bool Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual StudentModel Student { get; set; }
            public virtual ImageModel Image { get; set; }
            public virtual List<CommentModel> Comments { get; set; }
            public virtual List<BookLogsModel> BookLogs { get; set; }
            public virtual List<RatingModel> Ratings { get; set; }
            public virtual List<BookRecommendationsModel> HomeRecommendations { get; set; }
            public virtual List<BookRecommendationsModel> AwayRecommendations { get; set; }
        }

        [Table("Students")]
        public class StudentModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("Section")]
            public Guid SectionId { get; set; }
            [ForeignKey("StudentAddress")]
            public Guid? StudentAddressId { get; set; }
            public string FirstName { get; set; }
            public string MiddleInitial { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateTime? Birthday { get; set; }
            public string ContactNumber { get; set; }
            public bool Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual SectionsModel Section { get; set; }
            public virtual StudentAddressModel StudentAddress { get; set; }
        }

        [Table("StudentAddresses")]
        public class StudentAddressModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            public int ZipCode { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            
        }


        [Table("Sections")]
        public class SectionsModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("Grade")]
            public Guid GradeId { get; set; }
            [ForeignKey("Image")]
            public Guid? ImageId { get; set; }
            [StringLength(450)]
            [Index(IsUnique = true)]
            public String Section { get; set; }
            [DefaultValue(false)]
            public bool Deleted { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual GradesModel Grade { get; set; }
            public virtual ImageModel Image { get; set; }

        }

        [Table("Grades")]
        public class GradesModel
        {
            [Column(Order = 0), Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("Image")]
            public Guid? ImageId { get; set; }
            [StringLength(450)]
            [Index(IsUnique = true)]
            public string Grade { get; set; }
            [DefaultValue(false)]
            public bool Delete { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual List<SectionsModel> Sections { get; set; }
            public virtual ImageModel Image { get; set; }

        }

        [Table("Subject")]
        public class SubjectModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [Index(IsUnique = true)]
            public int CallNo { get; set; }
            public string Subject { get; set; }

            public List<BookModel> Books { get; set; }
        }

        [Table("Books")]
        public class BookModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("Subject")]
            public Guid SubjectId { get; set; }
            [ForeignKey("Publisher")]
            public Guid PublisherId { get; set; }
            public string ISBN { get; set; }
            [StringLength(450)]
            [Index(IsUnique =true)]
            public string Title { get; set; }
            [StringLength(1024)]
            public string Synopsis { get; set; }
            public DateTime Copyright { get; set; }
            public string Volume { get; set; }
            public int NoOfPages { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public bool Borrow { get; set; }
            [DefaultValue(false)]
            public bool Delete { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual List<BookAuthorModel> BooksAuthors{ get; set; }
            public virtual SubjectModel Subject { get; set; }
            public virtual PublisherModel Publisher { get; set; }
            public virtual List<CommentModel> Comments{ get; set; }
            public virtual List<RatingModel> Ratings { get; set; }
            public virtual List<ReservationModel> Reservations { get; set; }
            public virtual List<BookLogsModel> BookLogs { get; set; }
        }

        [Table("Authors")]
        public class AuthorModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            [StringLength(1, MinimumLength =1)]
            public string MiddleInitial { get; set; }
            [DefaultValue(false)]
            public bool Delete { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual List<BookAuthorModel> BooksAuthors { get; set; }
        }

        [Table("Publishers")]
        public class PublisherModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [StringLength(450)]
            [Index(IsUnique = true)]
            public string PublisherName { get; set; }
            [DefaultValue(false)]
            public bool Delete { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public List<BookModel> Book { get; set; }
        }
        
        [Table("BooksAuthors")]
        public class BookAuthorModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("Book")]
            public Guid BookId { get; set; }
            [ForeignKey("Author")]
            public Guid AuthorId { get; set; }
            [DefaultValue(false)]
            public bool Deleted { get; set; }

            public virtual BookModel Book { get; set; }
            public virtual AuthorModel Author { get; set; }
        }

        [Table("Comments")]
        public class CommentModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("User")]
            public Guid UserId { get; set; }
            [ForeignKey("Book")]
            public Guid BookId { get; set; }
            public string Comment { get; set; }
            [DefaultValue(false)]
            public bool Delete { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual UserModel User { get; set; }
            public virtual BookModel Book { get; set; }
        }

        [Table("Ratings")]
        public class RatingModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("User")]
            public Guid UserId { get; set; }
            [ForeignKey("Book")]
            public Guid BookId { get; set; }
            public int Rate { get; set; }
            public DateTime? CreatedAt { get; set; }

            public virtual UserModel User { get; set; }
            public virtual BookModel Book { get; set; }
        }

        [Table("Reservations")]
        public class ReservationModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("User")]
            public Guid UserId { get; set; }
            [ForeignKey("Book")]
            public Guid BookId { get; set; }
            [DefaultValue(false)]
            public bool Delete { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual UserModel User { get; set; }
            public virtual BookModel Book { get; set; }
        }

        [Table("BookLogs")]
        public class BookLogsModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("User")]
            public Guid UserId { get; set; }
            [ForeignKey("Book")]
            public Guid BookId { get; set; }
            [DefaultValue(false)]
            public bool Return { get; set; }
            [DefaultValue(true)]
            public bool BookStatus { get; set; }
            public string MessageLog { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime Deadline { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual UserModel User { get; set; }
            public virtual BookModel Book { get; set; }
        }

        [Table("BookRecommendations")]
        public class BookRecommendationsModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("FromUser")]
            public Guid FromUserId { get; set; }
            [ForeignKey("ToUser")]
            public Guid ToUserId { get; set; }
            [ForeignKey("Book")]
            public Guid BookId { get; set; }
            public bool Deleted { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            
            [InverseProperty("AwayRecommendations")]
            public virtual UserModel FromUser { get; set; }
            [InverseProperty("HomeRecommendations")]
            public virtual UserModel ToUser { get; set; }
            public virtual BookModel Book { get; set; }
        }

        [Table("EmailCredentials")]
        public class EmailCredentialModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            public string Host { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool Deleted { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual List<EmailMessageModel> EmailMessages { get; set; }
        }

        [Table("EmailMessages")]
        public class EmailMessageModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }
            [ForeignKey("EmailCredential")]
            public Guid EmailCredentialId { get; set; }
            public string Type { get; set; }
            public string From { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool Deleted { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public virtual EmailCredentialModel EmailCredential { get; set; }
        }
        [Table("Images")]
        public class ImageModel
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid ImageId { get; set; }
            public string Path { get; set; }
            public string Name { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

    }
}