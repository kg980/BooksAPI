namespace BooksAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!; // giving null value for string to stop warnings
        public string Author { get; set; } = null!;
        public int YearPublished { get; set; }
    }
}
