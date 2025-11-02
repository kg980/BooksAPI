using BooksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Data
{
    public class BooksAPIContext : DbContext
    {
        public BooksAPIContext(DbContextOptions<BooksAPIContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // default parent config of this method will still run

            modelBuilder.Entity<Book>().HasData( // adding my original static list data as seed data for the DB
                new Book { Id = 1, Title = "To Kill a Mockingbird", Author = "Harper Lee", YearPublished = 1960 },
                new Book { Id = 2, Title = "1984", Author = "George Orwell", YearPublished = 1949 },
                new Book { Id = 3, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", YearPublished = 1925 }
            );
        }
                

        // making my model class available as a DB Set
        public DbSet<BooksAPI.Models.Book> Books { get; set; }
    }
}
