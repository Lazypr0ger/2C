using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartOfAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NumChart = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Subconto1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Subconto2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountNumOrg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartamentHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DepartamentId = table.Column<string>(type: "text", nullable: false),
                    OldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartamentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartamentHistories_Departaments_DepartamentId",
                        column: x => x.DepartamentId,
                        principalTable: "Departaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PlannedCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DepartamentId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productions", x => x.Id);
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
                    NameDocument = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateOperation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganisationId = table.Column<string>(type: "text", nullable: true),
                    DepartamentId = table.Column<string>(type: "text", nullable: true),
                    TotalAmountDocument = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Agent = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OrganisationId = table.Column<string>(type: "text", nullable: false),
                    OldName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OldAccountNumOrg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationHistories_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProductionId = table.Column<string>(type: "text", nullable: false),
                    OldCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OldType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OldName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OldPlannedCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    OldDepartamentId = table.Column<string>(type: "text", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionHistories_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CountProduct = table.Column<int>(type: "integer", nullable: false),
                    CostRealisation = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    TotalCostElement = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
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
                    Subconto1Deb = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Subconto2Deb = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Subconto1Cred = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Subconto2Cred = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OperationId = table.Column<string>(type: "text", nullable: true),
                    ChartOfAccountDebId = table.Column<string>(type: "text", nullable: false),
                    ChartOfAccountCredId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_ChartOfAccounts_ChartOfAccountCredId",
                        column: x => x.ChartOfAccountCredId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_ChartOfAccounts_ChartOfAccountDebId",
                        column: x => x.ChartOfAccountDebId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_NumChart",
                table: "ChartOfAccounts",
                column: "NumChart",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentHistories_DepartamentId",
                table: "DepartamentHistories",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentHistories_ValidFrom",
                table: "DepartamentHistories",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentHistories_ValidTo",
                table: "DepartamentHistories",
                column: "ValidTo");

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
                name: "IX_Operations_DateOperation",
                table: "Operations",
                column: "DateOperation");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DepartamentId",
                table: "Operations",
                column: "DepartamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OrganisationId",
                table: "Operations",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_Type_DateOperation",
                table: "Operations",
                columns: new[] { "Type", "DateOperation" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationHistories_OrganisationId",
                table: "OrganisationHistories",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationHistories_ValidFrom",
                table: "OrganisationHistories",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_AccountNumOrg",
                table: "Organisations",
                column: "AccountNumOrg",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionHistories_ProductionId",
                table: "ProductionHistories",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionHistories_ValidFrom",
                table: "ProductionHistories",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionHistories_ValidTo",
                table: "ProductionHistories",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_Productions_Code",
                table: "Productions",
                column: "Code",
                unique: true);

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
                name: "IX_TransactionLogs_ChartOfAccountCredId_DateOperation",
                table: "TransactionLogs",
                columns: new[] { "ChartOfAccountCredId", "DateOperation" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_ChartOfAccountDebId_DateOperation",
                table: "TransactionLogs",
                columns: new[] { "ChartOfAccountDebId", "DateOperation" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_DateOperation",
                table: "TransactionLogs",
                column: "DateOperation");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_OperationId",
                table: "TransactionLogs",
                column: "OperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartamentHistories");

            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "OrganisationHistories");

            migrationBuilder.DropTable(
                name: "ProductionHistories");

            migrationBuilder.DropTable(
                name: "TransactionLogs");

            migrationBuilder.DropTable(
                name: "Productions");

            migrationBuilder.DropTable(
                name: "ChartOfAccounts");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Departaments");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
