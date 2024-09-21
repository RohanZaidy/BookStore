using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class BookViewModel
    {
        public int Isbn { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Author { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public int Price { get; set; }

        [Required]
        public string Language { get; set; } = null!;

        [Required]
        public int Pages { get; set; }

        [Required]
        public string Genre { get; set; } = null!;

        [Required]
        public IFormFile Pic { get; set; } = null!;
    }
}
