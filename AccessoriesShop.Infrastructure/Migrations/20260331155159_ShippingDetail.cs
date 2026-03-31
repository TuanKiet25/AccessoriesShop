using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessoriesShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShippingDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingDetail",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingDetail",
                table: "Orders");
        }
    }
}
