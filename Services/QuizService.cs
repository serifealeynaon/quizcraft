using Microsoft.EntityFrameworkCore;
using QuizCraft.Data;
using QuizCraft.DTOs;
using QuizCraft.Models;

namespace QuizCraft.Services;

public class QuizService : IQuizService
{
    private readonly AppDbContext _db;

    public QuizService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<QuizSummaryDTO> CreateQuizAsync(CreateQuizDTO dto)
    {
        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description,
            TimeLimitSeconds = dto.TimeLimitSeconds,
            ShareCode = GenerateShareCode(dto.Title),
            Questions = dto.Questions.Select((q, index) => new Question
            {
                Text = q.Text,
                Order = index + 1,
                Options = q.Options.Select(o => new Option
                {
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            }).ToList()
        };

        _db.Quizzes.Add(quiz);
        await _db.SaveChangesAsync();

        return new QuizSummaryDTO
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            ShareCode = quiz.ShareCode,
            QuestionCount = quiz.Questions.Count,
            AttemptCount = 0,
            TimeLimitSeconds = quiz.TimeLimitSeconds,
            CreatedAt = quiz.CreatedAt
        };
    }

    public async Task<QuizDetailDTO?> GetQuizByCodeAsync(string code)
    {
        var quiz = await _db.Quizzes
            .Include(q => q.Questions.OrderBy(q => q.Order))
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.ShareCode == code);

        if (quiz == null) return null;

        return new QuizDetailDTO
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            ShareCode = quiz.ShareCode,
            TimeLimitSeconds = quiz.TimeLimitSeconds,
            CreatedAt = quiz.CreatedAt,
            Questions = quiz.Questions.Select(q => new QuestionDTO
            {
                Id = q.Id,
                Text = q.Text,
                Order = q.Order,
                Options = q.Options.Select(o => new OptionDTO
                {
                    Id = o.Id,
                    Text = o.Text
                    // Note: IsCorrect is NOT sent to the client
                }).ToList()
            }).ToList()
        };
    }

    public async Task<List<QuizSummaryDTO>> GetAllQuizzesAsync()
    {
        return await _db.Quizzes
            .Include(q => q.Questions)
            .Include(q => q.Attempts)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuizSummaryDTO
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                ShareCode = q.ShareCode,
                QuestionCount = q.Questions.Count,
                AttemptCount = q.Attempts.Count,
                TimeLimitSeconds = q.TimeLimitSeconds,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteQuizAsync(int id)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        _db.Quizzes.Remove(quiz);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<QuizResultDTO> SubmitQuizAsync(string code, SubmitQuizDTO dto)
    {
        var quiz = await _db.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.ShareCode == code);

        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with code '{code}' not found.");

        var details = new List<AnswerResultDTO>();
        int score = 0;

        foreach (var answer in dto.Answers)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question == null) continue;

            var selectedOption = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

            bool isCorrect = selectedOption?.IsCorrect ?? false;
            if (isCorrect) score++;

            details.Add(new AnswerResultDTO
            {
                QuestionText = question.Text,
                SelectedAnswer = selectedOption?.Text ?? "No answer",
                CorrectAnswer = correctOption?.Text ?? "Unknown",
                IsCorrect = isCorrect
            });
        }

        // Save attempt
        var attempt = new QuizAttempt
        {
            PlayerName = dto.PlayerName,
            Score = score,
            TotalQuestions = quiz.Questions.Count,
            TimeTakenSeconds = dto.TimeTakenSeconds,
            QuizId = quiz.Id
        };

        _db.QuizAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        return new QuizResultDTO
        {
            QuizTitle = quiz.Title,
            PlayerName = dto.PlayerName,
            Score = score,
            TotalQuestions = quiz.Questions.Count,
            Percentage = quiz.Questions.Count > 0
                ? Math.Round((double)score / quiz.Questions.Count * 100, 1)
                : 0,
            TimeTakenSeconds = dto.TimeTakenSeconds,
            Details = details
        };
    }

    public async Task<List<LeaderboardEntryDTO>> GetLeaderboardAsync(string code)
    {
        var attempts = await _db.QuizAttempts
            .Include(a => a.Quiz)
            .Where(a => a.Quiz.ShareCode == code)
            .OrderByDescending(a => a.Score)
            .ThenBy(a => a.TimeTakenSeconds)
            .Take(20)
            .ToListAsync();

        return attempts.Select((a, index) => new LeaderboardEntryDTO
        {
            Rank = index + 1,
            PlayerName = a.PlayerName,
            Score = a.Score,
            TotalQuestions = a.TotalQuestions,
            Percentage = a.Percentage,
            TimeTakenSeconds = a.TimeTakenSeconds,
            CompletedAt = a.CompletedAt
        }).ToList();
    }

    /// <summary>
    /// Generates a short, URL-friendly share code from the quiz title.
    /// </summary>
    private string GenerateShareCode(string title)
    {
        var slug = title.ToLower()
            .Replace(" ", "-")
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Take(10)
            .Aggregate("", (current, c) => current + c);

        var random = Guid.NewGuid().ToString("N")[..4];
        return $"{slug}-{random}";
    }
}
