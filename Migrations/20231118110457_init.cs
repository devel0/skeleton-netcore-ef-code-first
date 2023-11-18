using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace skeleton_netcore_ef_code_first.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ERecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ERecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ARecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ARecords_BRecords_BObjectId",
                        column: x => x.BObjectId,
                        principalTable: "BRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DRecords_CRecords_CObjectId",
                        column: x => x.CObjectId,
                        principalTable: "CRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TableE_ManyTableF_Many",
                columns: table => new
                {
                    EObjectsId = table.Column<int>(type: "INTEGER", nullable: false),
                    FObjectsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableE_ManyTableF_Many", x => new { x.EObjectsId, x.FObjectsId });
                    table.ForeignKey(
                        name: "FK_TableE_ManyTableF_Many_ERecords_EObjectsId",
                        column: x => x.EObjectsId,
                        principalTable: "ERecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableE_ManyTableF_Many_FRecords_FObjectsId",
                        column: x => x.FObjectsId,
                        principalTable: "FRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARecords_BObjectId",
                table: "ARecords",
                column: "BObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DRecords_CObjectId",
                table: "DRecords",
                column: "CObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TableE_ManyTableF_Many_FObjectsId",
                table: "TableE_ManyTableF_Many",
                column: "FObjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARecords");

            migrationBuilder.DropTable(
                name: "DRecords");

            migrationBuilder.DropTable(
                name: "TableE_ManyTableF_Many");

            migrationBuilder.DropTable(
                name: "BRecords");

            migrationBuilder.DropTable(
                name: "CRecords");

            migrationBuilder.DropTable(
                name: "ERecords");

            migrationBuilder.DropTable(
                name: "FRecords");
        }
    }
}
