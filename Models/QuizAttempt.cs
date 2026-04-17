using System.ComponentModel.DataAnnotations;

namespace QuizCraft.Models;

public class QuizAttempt
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string PlayerName { get; set; } = string.Empty;

    public int Score { get; set; }

    public int TotalQuestions { get; set; }

    public double Percentage => TotalQuestions > 0
        ? Math.Round((double)Score / TotalQuestions * 100, 1)
        : 0;

    public int TimeTakenSeconds { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;
}
