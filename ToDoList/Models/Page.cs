using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models;

public partial class Page
{
    [Key]
    public int PageId { get; set; }
    [Required]
    public string Pcontent { get; set; } = null!;
    [Required]
    public int PbookId { get; set; }

    public virtual Book? Pbook { get; set; }
}
