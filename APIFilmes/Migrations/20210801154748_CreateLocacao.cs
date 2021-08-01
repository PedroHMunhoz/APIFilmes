using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace APIFilmes.Migrations
{
    public partial class CreateLocacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filmes_Generos_GeneroID",
                table: "Filmes");

            migrationBuilder.AlterColumn<int>(
                name: "GeneroID",
                table: "Filmes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Locacao",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CPFCliente = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    DataLocacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacao", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Locacao_Item",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocacaoID = table.Column<int>(type: "int", nullable: false),
                    FilmeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacao_Item", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Locacao_Item_Filmes_FilmeID",
                        column: x => x.FilmeID,
                        principalTable: "Filmes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locacao_Item_Locacao_LocacaoID",
                        column: x => x.LocacaoID,
                        principalTable: "Locacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locacao_Item_FilmeID",
                table: "Locacao_Item",
                column: "FilmeID");

            migrationBuilder.CreateIndex(
                name: "IX_Locacao_Item_LocacaoID",
                table: "Locacao_Item",
                column: "LocacaoID");

            migrationBuilder.AddForeignKey(
                name: "FK_Filmes_Generos_GeneroID",
                table: "Filmes",
                column: "GeneroID",
                principalTable: "Generos",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filmes_Generos_GeneroID",
                table: "Filmes");

            migrationBuilder.DropTable(
                name: "Locacao_Item");

            migrationBuilder.DropTable(
                name: "Locacao");

            migrationBuilder.AlterColumn<int>(
                name: "GeneroID",
                table: "Filmes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Filmes_Generos_GeneroID",
                table: "Filmes",
                column: "GeneroID",
                principalTable: "Generos",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
