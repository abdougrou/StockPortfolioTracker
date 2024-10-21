using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPortfolioTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionStatusAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionStatusID",
                table: "Transaction",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransactionStatusID",
                table: "Transaction",
                column: "TransactionStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_TransactionStatus_TransactionStatusID",
                table: "Transaction",
                column: "TransactionStatusID",
                principalTable: "TransactionStatus",
                principalColumn: "TransactionStatusID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_TransactionStatus_TransactionStatusID",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_TransactionStatusID",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TransactionStatusID",
                table: "Transaction");
        }
    }
}
