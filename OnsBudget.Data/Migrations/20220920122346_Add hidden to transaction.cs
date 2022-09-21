using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnsBudget.Data.Migrations
{
    public partial class Addhiddentotransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "Transactions");
        }
    }
}
