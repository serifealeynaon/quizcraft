using System.ComponentModel.DataAnnotations;

namespace QuizCraft.Models;

public class Quiz
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(20)]
    public string ShareCode { get; set; } = string.Empty;

    public int? TimeLimitSeconds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Question> Questions { get; set; } = new();
    public List<QuizAttempt> Attempts { get; set; } = new();
}
