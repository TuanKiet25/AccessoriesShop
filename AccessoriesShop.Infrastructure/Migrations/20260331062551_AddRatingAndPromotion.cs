using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessoriesShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingAndPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Promotions_PercentageRange",
                table: "Promotions",
                sql: "\"IsPercentage\" = FALSE OR (\"DiscountValue\" >= 0 AND \"DiscountValue\" <= 100)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Promotions_PercentageRange",
                table: "Promotions");
        }
    }
}
