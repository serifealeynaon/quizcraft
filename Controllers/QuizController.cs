using Microsoft.AspNetCore.Mvc;
using QuizCraft.DTOs;
using QuizCraft.Services;

namespace QuizCraft.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    /// <summary>
    /// Create a new quiz.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<QuizSummaryDTO>> CreateQuiz([FromBody] CreateQuizDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate: each question must have exactly one correct answer
        foreach (var question in dto.Questions)
        {
            int correctCount = question.Options.Count(o => o.IsCorrect);
            if (correctCount != 1)
            {
                return BadRequest(new { error = $"Each question must have exactly one correct answer. '{question.Text}' has {correctCount}." });
            }
        }

        var result = await _quizService.CreateQuizAsync(dto);
        return CreatedAtAction(nameof(GetQuiz), new { code = result.ShareCode }, result);
    }

    /// <summary>
    /// Get a quiz by its share code (for playing).
    /// </summary>
    [HttpGet("{code}")]
    public async Task<ActionResult<QuizDetailDTO>> GetQuiz(string code)
    {
        var quiz = await _quizService.GetQuizByCodeAsync(code);
        if (quiz == null)
            return NotFound(new { error = $"Quiz with code '{code}' not found." });

        return Ok(quiz);
    }

    /// <summary>
    /// List all quizzes.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<QuizSummaryDTO>>> GetAllQuizzes()
    {
        var quizzes = await _quizService.GetAllQuizzesAsync();
        return Ok(quizzes);
    }

    /// <summary>
    /// Delete a quiz.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        var deleted = await _quizService.DeleteQuizAsync(id);
        if (!deleted)
            return NotFound(new { error = "Quiz not found." });

        return NoContent();
    }

    /// <summary>
    /// Submit answers for a quiz.
    /// </summary>
    [HttpPost("{code}/submit")]
    public async Task<ActionResult<QuizResultDTO>> SubmitQuiz(string code, [FromBody] SubmitQuizDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _quizService.SubmitQuizAsync(code, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get leaderboard for a quiz.
    /// </summary>
    [HttpGet("{code}/leaderboard")]
    public async Task<ActionResult<List<LeaderboardEntryDTO>>> GetLeaderboard(string code)
    {
        var leaderboard = await _quizService.GetLeaderboardAsync(code);
        return Ok(leaderboard);
    }
}
