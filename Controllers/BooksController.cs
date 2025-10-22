using BooksAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        // Making static list so that data and changes to data persists across requests
        // (load once when ctrlr instantiated, then each http request will use the same list. If not static, changes would be lost between requests (e.g. deleted recs come back))
        static private List<Book> books = new List<Book>
        {
            new Book { Id = 1, Title = "To Kill a Mockingbird", Author = "Harper Lee", YearPublished = 1960 },
            new Book { Id = 2, Title = "1984", Author = "George Orwell", YearPublished = 1949 },
            new Book { Id = 3, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", YearPublished = 1925 },
        };

        [HttpGet]
        public ActionResult<List<Book>> GetBooks()  // return type ActionResult<List<Book>> allows returning HTTP status codes along with data
        {
                return Ok(books); // wrapping in OK() object creates the status code of 200 OK with response body
        }
    }
}
