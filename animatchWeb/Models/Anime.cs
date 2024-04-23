using System.ComponentModel.DataAnnotations;

namespace animatchWeb.Models
{
    public class Anime
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        public int Year { get; set; }

        public string Photo { get; set; }

        public double Imdbrate { get; set; }
    }
}
