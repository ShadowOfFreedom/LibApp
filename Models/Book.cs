using System;
using System.ComponentModel.DataAnnotations;

namespace LibApp.Models {
    public class Book
    {
        public int Id { get; set; }

		[Required(ErrorMessage = "Please enter book's name")]
		[StringLength(255)]
		public string Name { get; set; }

		[Required(ErrorMessage = "Please enter author's name")]
		public string AuthorName { get; set; }

		public Genre Genre { get; set; }

        [Required(ErrorMessage = "Please select genre")]
		public byte GenreId { get; set; }

		public DateTime DateAdded { get; set; }

		[Required(ErrorMessage = "Please enter release date")]
        public DateTime ReleaseDate { get; set; }

		[Required(ErrorMessage = "Please enter how many in stock")]
		[Range(1,20,ErrorMessage = "Please enter a number between 1-20")]
		public int NumberInStock { get; set; }

		public int NumberAvailable { get; set; }
	}
      
}
