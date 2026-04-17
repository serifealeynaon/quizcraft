using System.ComponentModel.DataAnnotations;

namespace QuizCraft.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public int Order { get; set; }

    // Foreign key
    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    // Navigation
    public List<Option> Options { get; set; } = new();
}
