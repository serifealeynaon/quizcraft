using QuizCraft.DTOs;

namespace QuizCraft.Services;

public interface IQuizService
{
    Task<QuizSummaryDTO> CreateQuizAsync(CreateQuizDTO dto);
    Task<QuizDetailDTO?> GetQuizByCodeAsync(string code);
    Task<List<QuizSummaryDTO>> GetAllQuizzesAsync();
    Task<bool> DeleteQuizAsync(int id);
    Task<QuizResultDTO> SubmitQuizAsync(string code, SubmitQuizDTO dto);
    Task<List<LeaderboardEntryDTO>> GetLeaderboardAsync(string code);
}
