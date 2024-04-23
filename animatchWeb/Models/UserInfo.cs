using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace animatchWeb.Models
{
    public class UserInfo : IdentityUser
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public int Level { get; set; }

        public string Text { get; set; }

        public string Photo { get; set; }

        public int WatchedCount { get; set; }

        public bool isAdmin { get; set; }
    }
}
