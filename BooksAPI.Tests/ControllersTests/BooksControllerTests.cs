using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksAPI.Controllers;
using BooksAPI.Data;
using BooksAPI.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Tests.ControllersTests
{
    public class BooksControllerTests
    {

        private readonly BooksController _controller;
        private readonly BooksAPIContext _context;
        private readonly DbSet<Book> _fakeDbSet;


        public BooksControllerTests()
        {
            // Fake dependencies
            _context = A.Fake<BooksAPIContext>();
            _fakeDbSet = A.Fake<DbSet<Book>>();

            // Inject fake DbSet into the fake DbContext
            _context.Books = _fakeDbSet;

            // System Under Test (SUT)
            _controller = new BooksController(_context);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsOk_WithBooksList()
        {
            // Arrange
            var fakeBooks = new List<Book>
        {
            new() { Id = 1, Title = "Book A" },
            new() { Id = 2, Title = "Book B" }
        }.AsQueryable();

            A.CallTo(() => _fakeDbSet.ToListAsync(default))
                .Returns(Task.FromResult(fakeBooks.ToList()));

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }


        [Fact]
        public async Task GetBookById_ReturnsNotFound_WhenBookMissing()
        {
            // Arrange
            A.CallTo(() => _fakeDbSet.FindAsync(999))
                .Returns(ValueTask.FromResult<Book?>(null));

            // Act
            var result = await _controller.GetBookById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }


        [Fact]
        public async Task GetBookById_ReturnsOk_WithBook()
        {
            // Arrange
            var fakeBook = new Book { Id = 1, Title = "1984", Author = "Orwell" };
            A.CallTo(() => _fakeDbSet.FindAsync(1))
                .Returns(ValueTask.FromResult<Book?>(fakeBook));

            // Act
            var result = await _controller.GetBookById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<Book>(okResult.Value);
            Assert.Equal("1984", model.Title);
        }


        [Fact]
        public async Task CreateBook_ReturnsBadRequest_WhenBookIsNull()
        {
            // Act
            var result = await _controller.CreateBook(null);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }


        [Fact]
        public async Task CreateBook_ReturnsCreatedAtAction_WhenValid()
        {
            // Arrange
            var book = new Book { Id = 3, Title = "New Book" };

            // Act
            var result = await _controller.CreateBook(book);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var model = Assert.IsType<Book>(created.Value);
            Assert.Equal("New Book", model.Title);

            A.CallTo(() => _context.Books.Add(book)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _context.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
        }


        [Fact]
        public async Task UpdateBook_ReturnsNotFound_WhenBookMissing()
        {
            // Arrange
            A.CallTo(() => _fakeDbSet.FindAsync(1))
                .Returns(ValueTask.FromResult<Book?>(null));

            // Act
            var result = await _controller.UpdateBook(1, new Book());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task DeleteBook_RemovesBook_WhenFound()
        {
            // Arrange
            var fakeBook = new Book { Id = 1, Title = "To Delete" };
            A.CallTo(() => _fakeDbSet.FindAsync(1))
                .Returns(ValueTask.FromResult<Book?>(fakeBook));

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            A.CallTo(() => _fakeDbSet.Remove(fakeBook)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _context.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
        }




    }
}
