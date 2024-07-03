using Microsoft.EntityFrameworkCore;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Data;

public class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
    {
    }

    public DbSet<SurveyForm> SurveyForms { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<DistributionList> DistributionLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuring Survey entities
        modelBuilder.Entity<SurveyForm>()
            .HasMany(s => s.Questions)
            .WithOne(q => q.SurveyForm)
            .HasForeignKey(q => q.SurveyFormId);

        modelBuilder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId);

        modelBuilder.Entity<SurveyForm>().HasData(new SurveyForm
        {
            SurveyFormId = 1,
            Name = "Sample Survey",
            Type = "General",
            ClosingDate = DateTime.Now.AddMonths(1)
        });

        modelBuilder.Entity<Question>().HasData(
            new Question { QuestionId = 1, Text = "Sample Question 1", Type = "Radio", SurveyFormId = 1 },
            new Question { QuestionId = 2, Text = "Sample Question 2", Type = "Open", SurveyFormId = 1 }
        );

        modelBuilder.Entity<Option>().HasData(
            new Option { OptionId = 1, Text = "Option 1", QuestionId = 1 },
            new Option { OptionId = 2, Text = "Option 2", QuestionId = 1 }
        );
    }
}
