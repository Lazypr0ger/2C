using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringForHistoriesModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldCode",
                table: "ProductionHistories");

            migrationBuilder.DropColumn(
                name: "OldPlannedCost",
                table: "ProductionHistories");

            migrationBuilder.RenameColumn(
                name: "OldType",
                table: "ProductionHistories",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "OldName",
                table: "ProductionHistories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OldDepartamentId",
                table: "ProductionHistories",
                newName: "DepartamentId");

            migrationBuilder.RenameColumn(
                name: "OldName",
                table: "OrganisationHistories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OldAccountNumOrg",
                table: "OrganisationHistories",
                newName: "AccountNumOrg");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Productions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "PlannedCost",
                table: "Productions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentId",
                table: "Productions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductionHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PlannedCost",
                table: "ProductionHistories",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProductionHistories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrganisationHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductionHistories");

            migrationBuilder.DropColumn(
                name: "PlannedCost",
                table: "ProductionHistories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductionHistories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrganisationHistories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductionHistories",
                newName: "OldName");

            migrationBuilder.RenameColumn(
                name: "DepartamentId",
                table: "ProductionHistories",
                newName: "OldDepartamentId");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ProductionHistories",
                newName: "OldType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrganisationHistories",
                newName: "OldName");

            migrationBuilder.RenameColumn(
                name: "AccountNumOrg",
                table: "OrganisationHistories",
                newName: "OldAccountNumOrg");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Productions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PlannedCost",
                table: "Productions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentId",
                table: "Productions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldCode",
                table: "ProductionHistories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldPlannedCost",
                table: "ProductionHistories",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
