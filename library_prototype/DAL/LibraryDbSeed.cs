using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using library_prototype.DAL;
using SimpleCrypto;

namespace library_prototype.DAL
{
    public class LibraryDbSeed : System.Data.Entity.DropCreateDatabaseIfModelChanges<LibraryDbContext>
    {
        protected override void Seed(LibraryDbContext context)
        {
            
        }
    }
}