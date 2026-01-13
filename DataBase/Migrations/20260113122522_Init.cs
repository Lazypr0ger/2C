using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartOfAccounts",
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
                    table.PrimaryKey("PK_ChartOfAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departaments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ChartOfAccountId = table.Column<string>(type: "text", nullable: false),
                    DepChartNum = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departaments_ChartOfAccounts_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
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
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisations_ChartOfAccounts_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Productions",
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
                    table.PrimaryKey("PK_Productions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productions_ChartOfAccounts_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productions_Departaments_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
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
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Departaments_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departaments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operations_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Elements",
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
                    table.PrimaryKey("PK_Elements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Elements_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Elements_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
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
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_ChartOfAccounts_ChartOfAccount2Id",
                        column: x => x.ChartOfAccount2Id,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_ChartOfAccounts_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_NumChart",
                table: "ChartOfAccounts",
                column: "NumChart",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departaments_ChartOfAccountId",
                table: "Departaments",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Departaments_Name",
                table: "Departaments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Elements_OperationId",
                table: "Elements",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_ProductionId",
                table: "Elements",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DepartamentId",
                table: "Operations",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OrganisationId",
                table: "Operations",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_ChartOfAccountId",
                table: "Organisations",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Productions_ChartOfAccountId",
                table: "Productions",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Productions_Code_Name",
                table: "Productions",
                columns: new[] { "Code", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productions_DepartamentId",
                table: "Productions",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_ChartOfAccount2Id",
                table: "TransactionLogs",
                column: "ChartOfAccount2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_ChartOfAccountId",
                table: "TransactionLogs",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_OperationId",
                table: "TransactionLogs",
                column: "OperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "TransactionLogs");

            migrationBuilder.DropTable(
                name: "Productions");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Departaments");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "ChartOfAccounts");
        }
    }
}
