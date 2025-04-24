using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models;

public partial class User
{
    [Key]
    public int ID { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string? Password { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
