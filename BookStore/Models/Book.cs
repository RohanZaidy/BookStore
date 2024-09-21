using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Book
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
    public string Image { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
