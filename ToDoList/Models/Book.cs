using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models;

public partial class Book
{
    [Key]
    public int BookId { get; set; }
    [Required]
    public string BookName { get; set; } = null!;
    [Required]
    public int UserId { get; set; }
    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual User? User { get; set; }
}
