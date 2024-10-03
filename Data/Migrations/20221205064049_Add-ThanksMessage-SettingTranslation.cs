using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThanksMessageSettingTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastScreenMessage",
                table: "SettingTranslations",
                newName: "ThanksMessage");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 937, DateTimeKind.Local).AddTicks(2369),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 324, DateTimeKind.Local).AddTicks(1491));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 937, DateTimeKind.Local).AddTicks(4838),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 324, DateTimeKind.Local).AddTicks(3761));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 936, DateTimeKind.Local).AddTicks(9494),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 323, DateTimeKind.Local).AddTicks(8754));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 936, DateTimeKind.Local).AddTicks(5416),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 323, DateTimeKind.Local).AddTicks(5786));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThanksMessage",
                table: "SettingTranslations",
                newName: "LastScreenMessage");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 324, DateTimeKind.Local).AddTicks(1491),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 937, DateTimeKind.Local).AddTicks(2369));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 324, DateTimeKind.Local).AddTicks(3761),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 937, DateTimeKind.Local).AddTicks(4838));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 323, DateTimeKind.Local).AddTicks(8754),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 936, DateTimeKind.Local).AddTicks(9494));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 4, 16, 19, 45, 323, DateTimeKind.Local).AddTicks(5786),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 5, 10, 10, 48, 936, DateTimeKind.Local).AddTicks(5416));
        }
    }
}
