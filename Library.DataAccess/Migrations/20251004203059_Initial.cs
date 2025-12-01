using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Biography = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TotalBooksPublished = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPublishedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MostPopularGenre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ISBN = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Pages = table.Column<int>(type: "INTEGER", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthDate", "Country", "LastPublishedDate", "MostPopularGenre", "Name", "TotalBooksPublished" },
                values: new object[,]
                {
                    { 1, "Software engineer and author, known for Clean Code", new DateTime(1952, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "USA", null, "Programming", "Robert C. Martin", 1 },
                    { 2, "Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides", new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Various", null, "Programming", "Gang of Four", 1 },
                    { 3, "Authors of The Pragmatic Programmer", new DateTime(1965, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USA", null, "Programming", "David Thomas & Andrew Hunt", 1 }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Genre", "ISBN", "Pages", "Title", "Year" },
                values: new object[,]
                {
                    { 1, 1, "Programming", "978-0132350884", 464, "Clean Code", 2008 },
                    { 2, 3, "Programming", "978-0135957059", 352, "The Pragmatic Programmer", 2019 },
                    { 3, 2, "Programming", "978-0201633610", 395, "Design Patterns", 1994 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
