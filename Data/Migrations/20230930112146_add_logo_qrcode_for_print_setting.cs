using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class add_logo_qrcode_for_print_setting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryTags",
                table: "CategoryTags");

            migrationBuilder.DropIndex(
                name: "IX_CategoryTags_TagsId",
                table: "CategoryTags");

            migrationBuilder.AddColumn<string>(
                name: "LogoPhysicalPath",
                table: "PrinterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeUrl",
                table: "PrinterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(2191),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(5096));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(3176),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(6079));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(474),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(2654));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 406, DateTimeKind.Local).AddTicks(8957),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(931));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(3981),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(6872));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryTags",
                table: "CategoryTags",
                columns: new[] { "TagsId", "CategoriesId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTags_CategoriesId",
                table: "CategoryTags",
                column: "CategoriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryTags",
                table: "CategoryTags");

            migrationBuilder.DropIndex(
                name: "IX_CategoryTags_CategoriesId",
                table: "CategoryTags");

            migrationBuilder.DropColumn(
                name: "LogoPhysicalPath",
                table: "PrinterSettings");

            migrationBuilder.DropColumn(
                name: "QRCodeUrl",
                table: "PrinterSettings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(5096),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(2191));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "ItemImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(6079),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(3176));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(2654),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(474));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(931),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 406, DateTimeKind.Local).AddTicks(8957));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InsertTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 20, 14, 24, 14, 647, DateTimeKind.Local).AddTicks(6872),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 30, 14, 51, 46, 407, DateTimeKind.Local).AddTicks(3981));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryTags",
                table: "CategoryTags",
                columns: new[] { "CategoriesId", "TagsId" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTags_TagsId",
                table: "CategoryTags",
                column: "TagsId");
        }
    }
}
