using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddErrorMessageCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(7497),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 640, DateTimeKind.Local).AddTicks(2468));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(9247),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 640, DateTimeKind.Local).AddTicks(4185));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRequest",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 6, 13, 3, 21, 263, DateTimeKind.Utc).AddTicks(9565),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 4, 12, 0, 8, 638, DateTimeKind.Utc).AddTicks(5995));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(5275),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 640, DateTimeKind.Local).AddTicks(401));

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(3047),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 639, DateTimeKind.Local).AddTicks(8252));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Categories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 640, DateTimeKind.Local).AddTicks(2468),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(7497));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 640, DateTimeKind.Local).AddTicks(4185),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(9247));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRequest",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 4, 12, 0, 8, 638, DateTimeKind.Utc).AddTicks(5995),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 6, 13, 3, 21, 263, DateTimeKind.Utc).AddTicks(9565));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 640, DateTimeKind.Local).AddTicks(401),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(5275));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 4, 16, 30, 8, 639, DateTimeKind.Local).AddTicks(8252),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 6, 17, 33, 21, 265, DateTimeKind.Local).AddTicks(3047));
        }
    }
}
