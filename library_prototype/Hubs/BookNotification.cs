using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Security.Principal;
using library_prototype.DAL;
namespace library_prototype.Hubs
{
    public class BookNotification : Hub
    {
        public void Hello()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)Context.User.Identity;
            var getUserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).
                Select(c => c.Value).SingleOrDefault();
            var db = new LibraryDbContext();
            var getUser = db.Users.Where(u => u.Id == new Guid(getUserId)).SingleOrDefault();
            var name = getUser.Student.FirstName + " " + getUser.Student.MiddleInitial + ". " + getUser.Student.LastName;
            Clients.All.hello();
            Clients.User(getUserId).addNewMessageToPage("Rodner", "From context");
            Clients.User(name).addNewMessageToPage("Rodner", "From database");
        }
    }
}