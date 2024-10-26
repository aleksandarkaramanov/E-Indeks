using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Indeks.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Gradovis",
                table: "Gradovis");

            migrationBuilder.RenameTable(
                name: "Gradovis",
                newName: "Gradovi");

            migrationBuilder.RenameColumn(
                name: "Ime",
                table: "Gradovi",
                newName: "Name");

            migrationBuilder.AlterColumn<double>(
                name: "Prosek",
                table: "Students",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "Grad",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "IsActive",
                table: "Korisnici",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsAdmin",
                table: "Korisnici",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gradovi",
                table: "Gradovi",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Predmetis",
                columns: table => new
                {
                    Indeks = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predmetis", x => x.Indeks);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Predmetis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gradovi",
                table: "Gradovi");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Korisnici");

            migrationBuilder.RenameTable(
                name: "Gradovi",
                newName: "Gradovis");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Gradovis",
                newName: "Ime");

            migrationBuilder.AlterColumn<float>(
                name: "Prosek",
                table: "Students",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Grad",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gradovis",
                table: "Gradovis",
                column: "Id");
        }
    }
}
