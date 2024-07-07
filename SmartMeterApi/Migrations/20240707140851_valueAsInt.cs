using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeterApi.Migrations
{
    /// <inheritdoc />
    public partial class valueAsInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ConsumptionValue",
                table: "ConsumptionDatas",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ConsumptionValue",
                table: "ConsumptionDatas",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
