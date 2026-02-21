using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPaymentSystem.DAL.Persistence.Migrations;

/// <inheritdoc />
public partial class Rename_Field_Payment : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "AmountPayed",
            table: "Payments",
            newName: "AmountPaid");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "AmountPaid",
            table: "Payments",
            newName: "AmountPayed");
    }
}
