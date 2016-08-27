namespace library_prototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LibraryDbContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleInitial = c.String(maxLength: 1),
                        Delete = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BooksAuthors",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BookId = c.Guid(nullable: false),
                        AuthorId = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authors", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SubjectId = c.Guid(nullable: false),
                        PublisherId = c.Guid(nullable: false),
                        ISBN = c.String(),
                        Title = c.String(maxLength: 450),
                        Synopsis = c.String(maxLength: 1024),
                        Copyright = c.DateTime(nullable: false),
                        Volume = c.String(),
                        NoOfPages = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Borrow = c.Boolean(nullable: false),
                        Delete = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Publishers", t => t.PublisherId, cascadeDelete: true)
                .ForeignKey("dbo.Subject", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.SubjectId)
                .Index(t => t.PublisherId)
                .Index(t => t.Title, unique: true);
            
            CreateTable(
                "dbo.BookLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Return = c.Boolean(nullable: false),
                        BookStatus = c.Boolean(nullable: false),
                        MessageLog = c.String(),
                        CreatedAt = c.DateTime(),
                        Deadline = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StudentId = c.Guid(nullable: false),
                        ImageId = c.Guid(),
                        Email = c.String(maxLength: 450),
                        Password = c.String(),
                        PasswordSalt = c.String(),
                        Pincode = c.String(),
                        PincodeSalt = c.String(),
                        Role = c.String(),
                        SecretQuestion = c.String(),
                        SecretAnswer = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.ImageId)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.ImageId)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.BookRecommendations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FromUserId = c.Guid(nullable: false),
                        ToUserId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.FromUserId)
                .ForeignKey("dbo.Users", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Comment = c.String(),
                        Delete = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        ImageId = c.Guid(nullable: false, identity: true),
                        Path = c.String(),
                        Name = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ImageId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Rate = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SectionId = c.Guid(nullable: false),
                        StudentAddressId = c.Guid(),
                        FirstName = c.String(),
                        MiddleInitial = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        Birthday = c.DateTime(),
                        ContactNumber = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .ForeignKey("dbo.StudentAddresses", t => t.StudentAddressId)
                .Index(t => t.SectionId)
                .Index(t => t.StudentAddressId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        GradeId = c.Guid(nullable: false),
                        ImageId = c.Guid(),
                        Section = c.String(maxLength: 450),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Grades", t => t.GradeId, cascadeDelete: true)
                .ForeignKey("dbo.Images", t => t.ImageId)
                .Index(t => t.GradeId)
                .Index(t => t.ImageId)
                .Index(t => t.Section, unique: true);
            
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ImageId = c.Guid(),
                        Grade = c.String(maxLength: 450),
                        Delete = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.ImageId)
                .Index(t => t.ImageId)
                .Index(t => t.Grade, unique: true);
            
            CreateTable(
                "dbo.StudentAddresses",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ZipCode = c.Int(nullable: false),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PublisherName = c.String(maxLength: 450),
                        Delete = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PublisherName, unique: true);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Delete = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Subject",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CallNo = c.Int(nullable: false),
                        Subject = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CallNo, unique: true);
            
            CreateTable(
                "dbo.EmailCredentials",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Host = c.String(),
                        Username = c.String(),
                        Password = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailMessages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EmailCredentialId = c.Guid(nullable: false),
                        Type = c.String(),
                        From = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailCredentials", t => t.EmailCredentialId, cascadeDelete: true)
                .Index(t => t.EmailCredentialId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailMessages", "EmailCredentialId", "dbo.EmailCredentials");
            DropForeignKey("dbo.BooksAuthors", "BookId", "dbo.Books");
            DropForeignKey("dbo.Books", "SubjectId", "dbo.Subject");
            DropForeignKey("dbo.Reservations", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reservations", "BookId", "dbo.Books");
            DropForeignKey("dbo.Books", "PublisherId", "dbo.Publishers");
            DropForeignKey("dbo.BookLogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "StudentId", "dbo.Students");
            DropForeignKey("dbo.Students", "StudentAddressId", "dbo.StudentAddresses");
            DropForeignKey("dbo.Students", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Sections", "ImageId", "dbo.Images");
            DropForeignKey("dbo.Sections", "GradeId", "dbo.Grades");
            DropForeignKey("dbo.Grades", "ImageId", "dbo.Images");
            DropForeignKey("dbo.Ratings", "UserId", "dbo.Users");
            DropForeignKey("dbo.Ratings", "BookId", "dbo.Books");
            DropForeignKey("dbo.Users", "ImageId", "dbo.Images");
            DropForeignKey("dbo.Comments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "BookId", "dbo.Books");
            DropForeignKey("dbo.BookRecommendations", "ToUserId", "dbo.Users");
            DropForeignKey("dbo.BookRecommendations", "FromUserId", "dbo.Users");
            DropForeignKey("dbo.BookRecommendations", "BookId", "dbo.Books");
            DropForeignKey("dbo.BookLogs", "BookId", "dbo.Books");
            DropForeignKey("dbo.BooksAuthors", "AuthorId", "dbo.Authors");
            DropIndex("dbo.EmailMessages", new[] { "EmailCredentialId" });
            DropIndex("dbo.Subject", new[] { "CallNo" });
            DropIndex("dbo.Reservations", new[] { "BookId" });
            DropIndex("dbo.Reservations", new[] { "UserId" });
            DropIndex("dbo.Publishers", new[] { "PublisherName" });
            DropIndex("dbo.Grades", new[] { "Grade" });
            DropIndex("dbo.Grades", new[] { "ImageId" });
            DropIndex("dbo.Sections", new[] { "Section" });
            DropIndex("dbo.Sections", new[] { "ImageId" });
            DropIndex("dbo.Sections", new[] { "GradeId" });
            DropIndex("dbo.Students", new[] { "StudentAddressId" });
            DropIndex("dbo.Students", new[] { "SectionId" });
            DropIndex("dbo.Ratings", new[] { "BookId" });
            DropIndex("dbo.Ratings", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "BookId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.BookRecommendations", new[] { "BookId" });
            DropIndex("dbo.BookRecommendations", new[] { "ToUserId" });
            DropIndex("dbo.BookRecommendations", new[] { "FromUserId" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Users", new[] { "ImageId" });
            DropIndex("dbo.Users", new[] { "StudentId" });
            DropIndex("dbo.BookLogs", new[] { "BookId" });
            DropIndex("dbo.BookLogs", new[] { "UserId" });
            DropIndex("dbo.Books", new[] { "Title" });
            DropIndex("dbo.Books", new[] { "PublisherId" });
            DropIndex("dbo.Books", new[] { "SubjectId" });
            DropIndex("dbo.BooksAuthors", new[] { "AuthorId" });
            DropIndex("dbo.BooksAuthors", new[] { "BookId" });
            DropTable("dbo.EmailMessages");
            DropTable("dbo.EmailCredentials");
            DropTable("dbo.Subject");
            DropTable("dbo.Reservations");
            DropTable("dbo.Publishers");
            DropTable("dbo.StudentAddresses");
            DropTable("dbo.Grades");
            DropTable("dbo.Sections");
            DropTable("dbo.Students");
            DropTable("dbo.Ratings");
            DropTable("dbo.Images");
            DropTable("dbo.Comments");
            DropTable("dbo.BookRecommendations");
            DropTable("dbo.Users");
            DropTable("dbo.BookLogs");
            DropTable("dbo.Books");
            DropTable("dbo.BooksAuthors");
            DropTable("dbo.Authors");
        }
    }
}
