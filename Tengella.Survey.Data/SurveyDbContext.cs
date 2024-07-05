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
    public DbSet<SurveyAnalysis> SurveyAnalyses { get; set; }
    public DbSet<QuestionAnalysis> QuestionAnalyses { get; set; }
    public DbSet<OptionAnalysis> OptionAnalyses { get; set; }

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

        // SurveyAnalysis to QuestionAnalysis relationship
        modelBuilder.Entity<SurveyAnalysis>()
            .HasMany(sa => sa.QuestionAnalyses)
            .WithOne(qa => qa.SurveyAnalysis)
            .HasForeignKey(qa => qa.SurveyAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        // QuestionAnalysis to OptionAnalysis relationship
        modelBuilder.Entity<QuestionAnalysis>()
            .HasMany(qa => qa.OptionAnalyses)
            .WithOne(oa => oa.QuestionAnalysis)
            .HasForeignKey(oa => oa.QuestionAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        // QuestionAnalysis to Question relationship
        modelBuilder.Entity<QuestionAnalysis>()
            .HasOne(qa => qa.Question)
            .WithMany()
            .HasForeignKey(qa => qa.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        // OptionAnalysis to Option relationship
        modelBuilder.Entity<OptionAnalysis>()
            .HasOne(oa => oa.Option)
            .WithMany()
            .HasForeignKey(oa => oa.OptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
