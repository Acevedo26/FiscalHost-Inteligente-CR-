using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalHost.Api.CR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLlavesCriptograficas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TribuCr",
                table: "ConfiguracionesTributarias",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "LlavesCriptograficas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnfitrionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContenidoCifrado = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LlavesCriptograficas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriasLlave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveCriptograficaId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriasLlave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriasLlave_LlavesCriptograficas_LlaveCriptograficaId",
                        column: x => x.LlaveCriptograficaId,
                        principalTable: "LlavesCriptograficas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasLlave_LlaveCriptograficaId",
                table: "AuditoriasLlave",
                column: "LlaveCriptograficaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AuditoriasLlave");
            migrationBuilder.DropTable(name: "LlavesCriptograficas");

            migrationBuilder.AlterColumn<string>(
                name: "TribuCr",
                table: "ConfiguracionesTributarias",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
