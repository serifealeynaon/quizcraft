using Microsoft.EntityFrameworkCore;
using QuizCraft.Models;

namespace QuizCraft.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Quiz
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasIndex(q => q.ShareCode).IsUnique();
            entity.HasMany(q => q.Questions)
                  .WithOne(q => q.Quiz)
                  .HasForeignKey(q => q.QuizId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(q => q.Attempts)
                  .WithOne(a => a.Quiz)
                  .HasForeignKey(a => a.QuizId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Question
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasMany(q => q.Options)
                  .WithOne(o => o.Question)
                  .HasForeignKey(o => o.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
