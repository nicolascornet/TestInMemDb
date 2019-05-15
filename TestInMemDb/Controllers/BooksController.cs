using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestInMemDb.Data;

namespace TestInMemDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        TestDbContext _context;

        public BooksController(TestDbContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Book>> Get()
        {
            return _context.Books;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Book> Get(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return NotFound(id);
            return book;
        }

        // POST api/values
        [HttpPost]
        public ActionResult<int> Post([FromBody] string name)
        {
            var book = new Book
            {
                Name = name
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            return book.Id;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public ActionResult<int> Put(int id, [FromBody] string name)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return NotFound(id);

            book.Name = name;
            _context.SaveChanges();

            return id;
        }
    }
}
