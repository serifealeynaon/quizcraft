namespace QuizCraft.DTOs;

public class QuizSummaryDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ShareCode { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public int AttemptCount { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class QuizDetailDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ShareCode { get; set; } = string.Empty;
    public int? TimeLimitSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuestionDTO> Questions { get; set; } = new();
}

public class QuestionDTO
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<OptionDTO> Options { get; set; } = new();
}

public class OptionDTO
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class QuizResultDTO
{
    public string QuizTitle { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public int TimeTakenSeconds { get; set; }
    public List<AnswerResultDTO> Details { get; set; } = new();
}

public class AnswerResultDTO
{
    public string QuestionText { get; set; } = string.Empty;
    public string SelectedAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class LeaderboardEntryDTO
{
    public int Rank { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public int TimeTakenSeconds { get; set; }
    public DateTime CompletedAt { get; set; }
}
