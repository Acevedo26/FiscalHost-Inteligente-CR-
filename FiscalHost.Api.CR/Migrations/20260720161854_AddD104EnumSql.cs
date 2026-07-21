using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalHost.Api.CR.Migrations
{
    /// <inheritdoc />
    public partial class AddD104EnumSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TYPE fiscalhost_db.tipo_formulario ADD VALUE IF NOT EXISTS 'D104';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
