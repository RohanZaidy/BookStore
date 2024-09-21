using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Cart
{
    public int Id { get; set; }

    [Required]
    public int BookIsbn { get; set; }

    [Required]
    public virtual Book BookIsbnNavigation { get; set; } = null!;
}
