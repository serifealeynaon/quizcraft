using System.ComponentModel.DataAnnotations;

namespace QuizCraft.Models;

public class Option
{
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    // Foreign key
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}
