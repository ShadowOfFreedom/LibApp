using LibApp.Models;
using System.Collections.Generic;

namespace LibApp.ViewModels {
    public class BookFormViewModel {
        public Book Book { get; set; }
        public IEnumerable<Genre> Genres { get; set; }

        public string Title {
            get {
                if (Book != null && Book.Id != 0)
                    return "Edit Book";

                return "New Book";
            }
        }

        public BookFormViewModel() => Book = new Book {Id = 0};

        public BookFormViewModel(Book book) => Book = book;
    }
}
