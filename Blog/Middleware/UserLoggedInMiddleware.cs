using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Middleware
{
    public class UserLoggedInMiddleware
    {
        private readonly RequestDelegate next;

        public UserLoggedInMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var s = context.Session.GetString("currentUser");
            var currentPath = context.Request.Path.Value;
            if (currentPath == "/Home/SignIn" || currentPath == "/Home/SignUp" || currentPath == "/Home/Index")
                await next.Invoke(context);
            else
            {
                if (string.IsNullOrEmpty(s))
                {
                    context.Response.Redirect("/Home/Index");
                }
                else
                {
                    await next.Invoke(context);
                }
            }
        }
    }
}
