using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaxSync.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddVaccineTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolId1",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolId1",
                table: "Students",
                column: "SchoolId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Schools_SchoolId1",
                table: "Students",
                column: "SchoolId1",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Schools_SchoolId1",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SchoolId1",
                table: "Students");
        }
    }
}
