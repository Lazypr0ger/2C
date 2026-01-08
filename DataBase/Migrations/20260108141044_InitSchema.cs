using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class InitSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartOfAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NumChart = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Subconto1 = table.Column<string>(type: "text", nullable: true),
                    Subconto2 = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartOfAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departament",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ChartOfAccountId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departament", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departament_ChartOfAccount_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organisation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AccountNumOrg = table.Column<string>(type: "text", nullable: false),
                    ChartOfAccountId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisation_ChartOfAccount_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Production",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PlannedCost = table.Column<decimal>(type: "numeric", nullable: false),
                    DepartamentId = table.Column<string>(type: "text", nullable: false),
                    ChartOfAccountId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Production", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Production_ChartOfAccount_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Production_Departament_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Operation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NameDocument = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DateOperation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganisationId = table.Column<string>(type: "text", nullable: true),
                    DepartamentId = table.Column<string>(type: "text", nullable: true),
                    TotalAmountDocument = table.Column<decimal>(type: "numeric", nullable: false),
                    Agent = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operation_Departament_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departament",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operation_Organisation_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Element",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CountProduct = table.Column<int>(type: "integer", nullable: false),
                    CostRealisation = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalCostElement = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductionId = table.Column<string>(type: "text", nullable: true),
                    OperationId = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Element_Operation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Element_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DateOperation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Subconto1Deb = table.Column<decimal>(type: "numeric", nullable: false),
                    Subconto2Deb = table.Column<decimal>(type: "numeric", nullable: false),
                    Subconto1Cred = table.Column<decimal>(type: "numeric", nullable: false),
                    Subconto2Cred = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    OperationId = table.Column<string>(type: "text", nullable: true),
                    ChartOfAccountId = table.Column<string>(type: "text", nullable: false),
                    ChartOfAccount2Id = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLog_ChartOfAccount_ChartOfAccount2Id",
                        column: x => x.ChartOfAccount2Id,
                        principalTable: "ChartOfAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionLog_ChartOfAccount_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionLog_Operation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccount_NumChart",
                table: "ChartOfAccount",
                column: "NumChart",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departament_ChartOfAccountId",
                table: "Departament",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Departament_Name",
                table: "Departament",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Element_OperationId",
                table: "Element",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Element_ProductionId",
                table: "Element",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_DepartamentId",
                table: "Operation",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_OrganisationId",
                table: "Operation",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_ChartOfAccountId",
                table: "Organisation",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_ChartOfAccountId",
                table: "Production",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Production_Code_Name",
                table: "Production",
                columns: new[] { "Code", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Production_DepartamentId",
                table: "Production",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_ChartOfAccount2Id",
                table: "TransactionLog",
                column: "ChartOfAccount2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_ChartOfAccountId",
                table: "TransactionLog",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_OperationId",
                table: "TransactionLog",
                column: "OperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Element");

            migrationBuilder.DropTable(
                name: "TransactionLog");

            migrationBuilder.DropTable(
                name: "Production");

            migrationBuilder.DropTable(
                name: "Operation");

            migrationBuilder.DropTable(
                name: "Departament");

            migrationBuilder.DropTable(
                name: "Organisation");

            migrationBuilder.DropTable(
                name: "ChartOfAccount");
        }
    }
}
