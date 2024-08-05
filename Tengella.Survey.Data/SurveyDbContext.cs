using Microsoft.EntityFrameworkCore;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Data;

public class SurveyDbContext(DbContextOptions<SurveyDbContext> options) : DbContext(options)
{
    public DbSet<SurveyForm> SurveyForms { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<Recipient> Recipients { get; set; }
    public DbSet<AnalysisLog> AnalysisLogs { get; set; }
    public DbSet<DistributionList> DistributionLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SurveyForm to Question relationship
        modelBuilder.Entity<SurveyForm>()
            .HasMany(s => s.Questions)
            .WithOne(q => q.SurveyForm)
            .HasForeignKey(q => q.SurveyFormId)
            .OnDelete(DeleteBehavior.Cascade);

        // Question to Option relationship
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Question to Response relationship
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Responses)
            .WithOne(r => r.Question)
            .HasForeignKey(r => r.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Response to SurveyForm relationship
        modelBuilder.Entity<Response>()
            .HasOne(r => r.SurveyForm)
            .WithMany()
            .HasForeignKey(r => r.SurveyFormId)
            .OnDelete(DeleteBehavior.Restrict);

        // Response to Option relationship
        modelBuilder.Entity<Response>()
            .HasOne(r => r.Option)
            .WithMany()
            .HasForeignKey(r => r.OptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}