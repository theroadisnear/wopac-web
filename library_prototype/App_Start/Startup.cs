﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace library_prototype.App_Start
{
    public class Startup
    {
        public void Configuration (IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "ApplicationCookie",
                    LoginPath = new PathString("/auth/login")
                });
            app.MapSignalR();
        }
    }
}