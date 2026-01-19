using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OldName",
                table: "DepartamentHistories",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DepartamentHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DepartamentHistories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "DepartamentHistories",
                newName: "OldName");
        }
    }
}
