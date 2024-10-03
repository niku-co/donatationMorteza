using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class IntentionCodeItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(1072),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 253, DateTimeKind.Local).AddTicks(7529));

            migrationBuilder.AddColumn<string>(
                name: "IntentionCode",
                table: "Items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(1797),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 254, DateTimeKind.Local).AddTicks(510));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(73),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 253, DateTimeKind.Local).AddTicks(6293));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 981, DateTimeKind.Local).AddTicks(8949),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 253, DateTimeKind.Local).AddTicks(3258));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(2723),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 254, DateTimeKind.Local).AddTicks(1281));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntentionCode",
                table: "Items");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 253, DateTimeKind.Local).AddTicks(7529),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(1072));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 254, DateTimeKind.Local).AddTicks(510),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(1797));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 253, DateTimeKind.Local).AddTicks(6293),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(73));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 253, DateTimeKind.Local).AddTicks(3258),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 981, DateTimeKind.Local).AddTicks(8949));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 2, 18, 35, 39, 254, DateTimeKind.Local).AddTicks(1281),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 4, 7, 18, 9, 982, DateTimeKind.Local).AddTicks(2723));
        }
    }
}
