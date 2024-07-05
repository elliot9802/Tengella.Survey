using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tengella.Survey.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DistributionLists",
                columns: table => new
                {
                    DistributionListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrgNmr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonNmr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionLists", x => x.DistributionListId);
                });

            migrationBuilder.CreateTable(
                name: "SurveyForms",
                columns: table => new
                {
                    SurveyFormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyForms", x => x.SurveyFormId);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyFormId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_SurveyForms_SurveyFormId",
                        column: x => x.SurveyFormId,
                        principalTable: "SurveyForms",
                        principalColumn: "SurveyFormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyAnalyses",
                columns: table => new
                {
                    SurveyAnalysisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyFormId = table.Column<int>(type: "int", nullable: false),
                    AnalysisDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalResponses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyAnalyses", x => x.SurveyAnalysisId);
                    table.ForeignKey(
                        name: "FK_SurveyAnalyses_SurveyForms_SurveyFormId",
                        column: x => x.SurveyFormId,
                        principalTable: "SurveyForms",
                        principalColumn: "SurveyFormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.OptionId);
                    table.ForeignKey(
                        name: "FK_Options_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnalyses",
                columns: table => new
                {
                    QuestionAnalysisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SurveyAnalysisId = table.Column<int>(type: "int", nullable: false),
                    TotalResponses = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnalyses", x => x.QuestionAnalysisId);
                    table.ForeignKey(
                        name: "FK_QuestionAnalyses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionAnalyses_SurveyAnalyses_SurveyAnalysisId",
                        column: x => x.SurveyAnalysisId,
                        principalTable: "SurveyAnalyses",
                        principalColumn: "SurveyAnalysisId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponseGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyFormId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: true),
                    TextResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_Responses_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "OptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Responses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Responses_SurveyForms_SurveyFormId",
                        column: x => x.SurveyFormId,
                        principalTable: "SurveyForms",
                        principalColumn: "SurveyFormId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OptionAnalyses",
                columns: table => new
                {
                    OptionAnalysisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    QuestionAnalysisId = table.Column<int>(type: "int", nullable: false),
                    ResponseCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionAnalyses", x => x.OptionAnalysisId);
                    table.ForeignKey(
                        name: "FK_OptionAnalyses_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "OptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OptionAnalyses_QuestionAnalyses_QuestionAnalysisId",
                        column: x => x.QuestionAnalysisId,
                        principalTable: "QuestionAnalyses",
                        principalColumn: "QuestionAnalysisId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionAnalyses_OptionId",
                table: "OptionAnalyses",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionAnalyses_QuestionAnalysisId",
                table: "OptionAnalyses",
                column: "QuestionAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_QuestionId",
                table: "Options",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnalyses_QuestionId",
                table: "QuestionAnalyses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnalyses_SurveyAnalysisId",
                table: "QuestionAnalyses",
                column: "SurveyAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyFormId",
                table: "Questions",
                column: "SurveyFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_OptionId",
                table: "Responses",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_QuestionId",
                table: "Responses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_SurveyFormId",
                table: "Responses",
                column: "SurveyFormId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnalyses_SurveyFormId",
                table: "SurveyAnalyses",
                column: "SurveyFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributionLists");

            migrationBuilder.DropTable(
                name: "OptionAnalyses");

            migrationBuilder.DropTable(
                name: "Responses");

            migrationBuilder.DropTable(
                name: "QuestionAnalyses");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "SurveyAnalyses");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "SurveyForms");
        }
    }
}
