using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMeterApi.Migrations
{
    /// <inheritdoc />
    public partial class monthsAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ConsumptionDatas");

            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "ConsumptionDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ConsumptionDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "ConsumptionDatas");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ConsumptionDatas");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "ConsumptionDatas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
