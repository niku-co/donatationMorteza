using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class RemoveItemIdFromItemTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTranslations_Items_ItemId1",
                table: "ItemTranslations");

            migrationBuilder.DropIndex(
                name: "IX_ItemTranslations_ItemId1",
                table: "ItemTranslations");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                table: "ItemTranslations");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "ItemTranslations",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(7042),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 640, DateTimeKind.Local).AddTicks(2796));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(8944),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 640, DateTimeKind.Local).AddTicks(4830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(4866),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 640, DateTimeKind.Local).AddTicks(695));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(2545),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 639, DateTimeKind.Local).AddTicks(8412));

            migrationBuilder.CreateIndex(
                name: "IX_ItemTranslations_ItemId",
                table: "ItemTranslations",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTranslations_Items_ItemId",
                table: "ItemTranslations",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTranslations_Items_ItemId",
                table: "ItemTranslations");

            migrationBuilder.DropIndex(
                name: "IX_ItemTranslations_ItemId",
                table: "ItemTranslations");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "ItemTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemId1",
                table: "ItemTranslations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 640, DateTimeKind.Local).AddTicks(2796),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(7042));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 640, DateTimeKind.Local).AddTicks(4830),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(8944));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 640, DateTimeKind.Local).AddTicks(695),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(4866));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 8, 27, 13, 52, 20, 639, DateTimeKind.Local).AddTicks(8412),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 8, 27, 13, 54, 29, 636, DateTimeKind.Local).AddTicks(2545));

            migrationBuilder.CreateIndex(
                name: "IX_ItemTranslations_ItemId1",
                table: "ItemTranslations",
                column: "ItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTranslations_Items_ItemId1",
                table: "ItemTranslations",
                column: "ItemId1",
                principalTable: "Items",
                principalColumn: "Id");
        }
    }
}
