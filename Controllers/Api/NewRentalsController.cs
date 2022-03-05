using LibApp.Dtos;
using LibApp.Models;
using LibApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibApp.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class NewRentalsController : ControllerBase {
        private readonly IRepositoryAsync<Rental> _rentalsRepo;
        private readonly IRepositoryAsync<Customer> _customersRepo;
        private readonly IRepositoryAsync<Book> _booksRepo;

        public NewRentalsController(IRepositoryAsync<Rental> rentalsRepo, IRepositoryAsync<Customer> customersRepo, IRepositoryAsync<Book> booksRepo) {
            _rentalsRepo = rentalsRepo;
            _customersRepo = customersRepo;
            _booksRepo = booksRepo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewRentals(NewRentalDto newRental) {
            var customer = await _customersRepo.GetById(newRental.CustomerId);
            
            var books = new List<Book>();
            foreach (var bookId in newRental.BookIds) {
                var book = await _booksRepo.GetById(bookId);
                if (book != null) books.Add(book);
            }

            foreach (var book in books) {
                if (book.NumberAvailable == 0)
                    return BadRequest("Book is no available");

                book.NumberAvailable--;

                var rental = new Rental() {
                    Customer = customer,
                    Book = book,
                    DateRented = DateTime.Now
                };
                
                await _rentalsRepo.Add(rental);
            }

            return Ok();
        }
    }
}