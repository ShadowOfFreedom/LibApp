using AutoMapper;
using LibApp.Dtos;
using LibApp.Models;
using LibApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace LibApp.Controllers.Api {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {
        private readonly IRepositoryAsync<Book> _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IRepositoryAsync<Book> bookRepository, IMapper mapper) {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(string query = null) {
            var booksQuery = await _bookRepository.GetAll();
            booksQuery = booksQuery.Where(b => b.NumberAvailable > 0).ToList();
            
            if (!String.IsNullOrWhiteSpace(query))
                booksQuery = booksQuery.Where(b => b.Name.Contains(query)).ToList();
            
            return Ok(booksQuery.ToList().Select(_mapper.Map<Book, BookDto>));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteBook(int id) {
            var book = await _bookRepository.GetById(id);
            if (book == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            await _bookRepository.Delete(id);
            return Ok(_mapper.Map<BookDto>(book));
        }
    }
}