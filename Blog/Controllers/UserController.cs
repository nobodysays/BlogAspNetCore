using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Like(int? param)
        {
            if (param == null)
                return NotFound();

            var postCollection = from p in dbContext.Posts where p.Id == param select p;
            var likes = from l in dbContext.Likes where l.FromId == param select l;
            var postOwner = string.Empty;
     
                if (postCollection.Count() != 0)
                {
                    var post = postCollection.First();
                    postOwner = GetUserById(post.UserId).Username;
                    if (likes.Count() == 0)
                    {

                        post.Likes++;
                        dbContext.Likes.Add(new Models.Like
                        {
                            FromId = GetCurrentUser().Id,
                            ToId = (int)param
                        });
                    }
                    dbContext.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
    

            return RedirectToAction("Profile", "User", new { param = postOwner });
        }
        [HttpGet]
        public IActionResult RemovePost(int? param)
        {
            if (param == null)
                return NotFound();

            var postCollection = from p in dbContext.Posts where p.Id == param select p;
            if(postCollection.Count()!=0)
            {
                var post = postCollection.First();
                dbContext.Remove(post);
                dbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Profile", "User");
        }
        public IActionResult Edit(ViewModels.UserViewModel userViewModel)
        {
            var currentUser = HttpContext.Session.GetString("currentUser");
            var user = (from u in dbContext.Users
                    where u.Username == currentUser
                    select u).First();
            user.Avatar = UploadedFile(userViewModel);
            dbContext.SaveChanges();
            return RedirectToAction("Settings");
        }

        private string UploadedFile(ViewModels.UserViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public IActionResult NewPost()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewPost(Post newPost)
        {
            var user = GetCurrentUser();
            newPost.UserId = user.Id;
            dbContext.Posts.Add(newPost);
            dbContext.SaveChanges();
            return RedirectToAction("Profile", "User");
        }

        public IActionResult MyProfile()
        {
            var currUser = GetCurrentUser();
            var currUserId = currUser.Id;
            ViewBag.Posts = (from p in dbContext.Posts where p.UserId == currUserId select p).ToList();
            return View(currUser);
        }
        public IActionResult Profile(string? param)
        {
            if (param == null)
            {
                return RedirectToAction("MyProfile", "User");
            }
            else if (string.Equals(HttpContext.Session.GetString("currentUser"), param))
            {
                return RedirectToAction("MyProfile", "User");
            }

            User user;

            var temp = (from u in dbContext.Users
                        where u.Username == param
                        select u);



            if (temp.Count() != 0)
            {
                user = temp.First();
            }
            else
            {
                return NotFound();
            }

            var posts = (from p in dbContext.Posts where p.UserId == user.Id select p).ToList();
            ViewBag.Posts = posts;
            return View(user);
        }

        public async Task<IActionResult> ShowUsers()
        {
            return View(await dbContext.Users.ToListAsync());
        }
        private User GetCurrentUser()
        {
            var currentUser = HttpContext.Session.GetString("currentUser");
            var user = (from u in dbContext.Users
                    where u.Username == currentUser
                    select u).First();
            return user;
        }

        private User GetUserById(int id)
        {
            var user = (from u in dbContext.Users
                        where u.Id == id
                        select u).First();
            return user;
        }
    }
}
