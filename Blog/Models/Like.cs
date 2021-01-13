using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
    }
}
