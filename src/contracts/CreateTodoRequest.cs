using System.ComponentModel.DataAnnotations;

namespace MonPremierProjet.Contracts;

public class CreateTodoRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public bool IsCompleted { get; set; }
}