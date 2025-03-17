namespace Movies.API.Model
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }
        public string Rating { get; internal set; } = string.Empty;
        public string ImageUrl { get; internal set; } = string.Empty;
    }
}
