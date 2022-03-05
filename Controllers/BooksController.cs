using LibApp.Models;
using LibApp.Repositories.Interfaces;
using LibApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LibApp.Controllers {
    [Authorize]
    public class BooksController : Controller {
        private readonly IRepositoryAsync<Book> _booksRepo;
        private readonly IRepositoryAsync<Genre> _genresRepo;

        public BooksController( IRepositoryAsync<Book> booksRepo, IRepositoryAsync<Genre> genresRepo) {
            _booksRepo = booksRepo;
            _genresRepo = genresRepo;
        }

        public async Task<IActionResult> Index() =>
            View( await _booksRepo.GetAll());

        public async Task<IActionResult> Details(int id) {
            var book = await _booksRepo.GetById(id);

            if (book == null)
                return Content("Book not found");

            return View(book);
        }


        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> New() {
            var viewModel = new BookFormViewModel {
                Genres = await _genresRepo.GetAll()
            };

            return View("BookForm", viewModel);
        }


        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int id) {
            var book = await _booksRepo.GetById(id);
            if (book == null)
                return NotFound();

            var viewModel = new BookFormViewModel(book) {
                Genres = await _genresRepo.GetAll()
            };

            return View("BookForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "StoreManager")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Save(Book book) {
            if (!ModelState.IsValid) {
                var viewModel = new BookFormViewModel(book) {
                    Genres = await _genresRepo.GetAll()
                };

                return View("BookForm", viewModel);
            }

            try {
                if (book.Id == 0) {
                    book.DateAdded = DateTime.Now;
                    await _booksRepo.Add(book);
                }
                else
                    await _booksRepo.Update(book);
            }
            catch (DbUpdateException e) {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Books");
        }
    }
}