using BooksAPI.Data;
using BooksAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        // Making static list so that data and changes to data persists across requests
        // (load once when ctrlr instantiated, then each http request will use the same list. If not static, changes would be lost between requests (e.g. deleted recs come back))
        // This has moved to the DB seed data in BooksAPIContext.cs
        //static private List<Book> books = new List<Book>
        //{
        //    new Book { Id = 1, Title = "To Kill a Mockingbird", Author = "Harper Lee", YearPublished = 1960 },
        //    new Book { Id = 2, Title = "1984", Author = "George Orwell", YearPublished = 1949 },
        //    new Book { Id = 3, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", YearPublished = 1925 },
        //};


        private readonly BooksAPIContext _context;
        public BooksController(BooksAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAllBooks()  // return type ActionResult<List<Book>> allows returning HTTP status codes along with data
        {
            return Ok(await _context.Books.ToListAsync()); // wrapping in OK() object creates the status code of 200 OK with response body
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(); // returns 404 Not Found if book not found
            }
            return Ok(book);

        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            if (book == null)
            {
                return BadRequest(); // returns 400 Bad Request if input is null
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(); // returns 404 Not Found if book not found
            }
            // Update book details
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.YearPublished = updatedBook.YearPublished;

            await _context.SaveChangesAsync();
            return NoContent(); // returns 204 No Content to indicate successful update
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(); // returns 404 Not Found if book not found
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent(); // returns 204 No Content to indicate successful deletion
        }

    }
}
