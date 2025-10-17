using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaxSync.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertsPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Schools_SchoolId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SchoolId1",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "SSN",
                table: "Students",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Students",
                newName: "FirstName");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Students",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Schools",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "Vaccines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentVaccines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    VaccineId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DateGiven = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentVaccines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentVaccines_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentVaccines_Vaccines_VaccineId",
                        column: x => x.VaccineId,
                        principalTable: "Vaccines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaccineSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VaccineId = table.Column<int>(type: "INTEGER", nullable: false),
                    AgeRange = table.Column<string>(type: "TEXT", nullable: false),
                    DoseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CatchUpEligible = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccineSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaccineSchedules_Vaccines_VaccineId",
                        column: x => x.VaccineId,
                        principalTable: "Vaccines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentRequiredDoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    VaccineScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRequiredDoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRequiredDoses_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentRequiredDoses_VaccineSchedules_VaccineScheduleId",
                        column: x => x.VaccineScheduleId,
                        principalTable: "VaccineSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolId_LastName_FirstName",
                table: "Students",
                columns: new[] { "SchoolId", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequiredDoses_StudentId",
                table: "StudentRequiredDoses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequiredDoses_VaccineScheduleId",
                table: "StudentRequiredDoses",
                column: "VaccineScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentVaccines_StudentId",
                table: "StudentVaccines",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentVaccines_VaccineId",
                table: "StudentVaccines",
                column: "VaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_VaccineSchedules_VaccineId",
                table: "VaccineSchedules",
                column: "VaccineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Schools_SchoolId",
                table: "Students",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Schools_SchoolId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "StudentRequiredDoses");

            migrationBuilder.DropTable(
                name: "StudentVaccines");

            migrationBuilder.DropTable(
                name: "VaccineSchedules");

            migrationBuilder.DropTable(
                name: "Vaccines");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolId_LastName_FirstName",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Students",
                newName: "SSN");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Students",
                newName: "Gender");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SchoolId1",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Schools",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

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
    }
}
