using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Restart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_Operations_OperationId",
                table: "Elements");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Departaments_DepartamentId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "CostRealisation",
                table: "Elements");

            migrationBuilder.RenameColumn(
                name: "CountProduct",
                table: "Elements",
                newName: "CountElement");

            migrationBuilder.AlterColumn<string>(
                name: "Subconto2Deb",
                table: "TransactionLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Subconto2Cred",
                table: "TransactionLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Subconto1Deb",
                table: "TransactionLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Subconto1Cred",
                table: "TransactionLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmountDocument",
                table: "Operations",
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
                name: "ProductionId",
                table: "Elements",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OperationId",
                table: "Elements",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Elements",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Subconto1Cred",
                table: "TransactionLogs",
                column: "Subconto1Cred");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Subconto1Deb",
                table: "TransactionLogs",
                column: "Subconto1Deb");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_Operations_OperationId",
                table: "Elements",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Departaments_DepartamentId",
                table: "Operations",
                column: "DepartamentId",
                principalTable: "Departaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_Operations_OperationId",
                table: "Elements");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Departaments_DepartamentId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_Subconto1Cred",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_Subconto1Deb",
                table: "TransactionLogs");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Elements");

            migrationBuilder.RenameColumn(
                name: "CountElement",
                table: "Elements",
                newName: "CountProduct");

            migrationBuilder.AlterColumn<decimal>(
                name: "Subconto2Deb",
                table: "TransactionLogs",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Subconto2Cred",
                table: "TransactionLogs",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Subconto1Deb",
                table: "TransactionLogs",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Subconto1Cred",
                table: "TransactionLogs",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmountDocument",
                table: "Operations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Operations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Operations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductionId",
                table: "Elements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OperationId",
                table: "Elements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "CostRealisation",
                table: "Elements",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_Operations_OperationId",
                table: "Elements",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Departaments_DepartamentId",
                table: "Operations",
                column: "DepartamentId",
                principalTable: "Departaments",
                principalColumn: "Id");
        }
    }
}
