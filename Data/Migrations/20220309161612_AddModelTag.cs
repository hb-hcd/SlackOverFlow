using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotQuora.Data.Migrations
{
    public partial class AddModelTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TagId",
                table: "Questions",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tag_TagId",
                table: "Questions",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tag_TagId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TagId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Questions");
        }
    }
}
