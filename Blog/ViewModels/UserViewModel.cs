using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewModels
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
