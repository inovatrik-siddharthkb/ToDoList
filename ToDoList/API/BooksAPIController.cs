using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoList.Models;
using Microsoft.Extensions.Options;
using System.Data;

namespace ToDoList.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _baseUrl;

        public BooksAPIController(AppDbContext context, IOptions<ApiSettings> apiSettings)
        {
            _context = context;
            _baseUrl = apiSettings.Value.BaseUrl;
        }

        // GET: api/BooksAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await _context.Books
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        // GET: api/BooksAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var book = await _context.Books
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookId == id);

            //if (book == null || book.UserId != userId)
            if(book == null)
                return NotFound();

            return book;
        }

        // PUT: api/BooksAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] Book book)
        {
            ModelState.Remove("User");

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.BookId)
            {
                return BadRequest("Book ID mismatch.");
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.BookId == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        // POST: api/BooksAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody] BookDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            //book.UserId = userId;

            var book = new Book
            {
                BookName = dto.BookName,
                UserId = userId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return Ok(book);
        }

        // DELETE: api/BooksAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            
            if(book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(book);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
