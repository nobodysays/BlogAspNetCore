using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public int Likes { get; set; }
    }
}
