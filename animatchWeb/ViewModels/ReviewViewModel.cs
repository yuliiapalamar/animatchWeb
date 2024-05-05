namespace animatchWeb.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AnimeId { get; set; }
        public string Username { get; set; }
        public string AnimeName { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }
    }
}
