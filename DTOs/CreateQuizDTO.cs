using System.ComponentModel.DataAnnotations;

namespace QuizCraft.DTOs;

public class CreateQuizDTO
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? TimeLimitSeconds { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateQuestionDTO> Questions { get; set; } = new();
}

public class CreateQuestionDTO
{
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    [MaxLength(6)]
    public List<CreateOptionDTO> Options { get; set; } = new();
}

public class CreateOptionDTO
{
    [Required]
    [MaxLength(300)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

public class SubmitQuizDTO
{
    [Required]
    [MaxLength(100)]
    public string PlayerName { get; set; } = string.Empty;

    public int TimeTakenSeconds { get; set; }

    [Required]
    public List<AnswerDTO> Answers { get; set; } = new();
}

public class AnswerDTO
{
    public int QuestionId { get; set; }
    public int SelectedOptionId { get; set; }
}
