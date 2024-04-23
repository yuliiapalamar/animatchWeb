using System.ComponentModel.DataAnnotations;

namespace animatchWeb.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int AnimeId { get; set; }

        public string Text { get; set; }

        [Required]
        public int Rate { get; set; }
    }
}
