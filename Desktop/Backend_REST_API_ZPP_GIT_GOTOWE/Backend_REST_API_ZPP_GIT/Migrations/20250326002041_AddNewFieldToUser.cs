using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendRESTAPIZPP.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kierunek",
                columns: table => new
                {
                    IDKierunek = table.Column<int>(name: "ID_Kierunek", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TypKierunku = table.Column<string>(name: "Typ_Kierunku", type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Kierunek__9FEFDE1E8AD0DBD7", x => x.IDKierunek);
                });

            migrationBuilder.CreateTable(
                name: "Prowadzacy",
                columns: table => new
                {
                    IDProwadzacy = table.Column<int>(name: "ID_Prowadzacy", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Nazwisko = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Skrot = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Tytul = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prowadza__8EF96D06400BC302", x => x.IDProwadzacy);
                });

            migrationBuilder.CreateTable(
                name: "Sala",
                columns: table => new
                {
                    IDSala = table.Column<int>(name: "ID_Sala", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Budynek = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Pomieszczenie = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sala__2071DEA7ECDCAAA0", x => x.IDSala);
                });

            migrationBuilder.CreateTable(
                name: "Przedmiot",
                columns: table => new
                {
                    IDTowaru = table.Column<int>(name: "ID_Towaru", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Typ = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Skrot = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IDProwadzacy = table.Column<int>(name: "ID_Prowadzacy", type: "int", nullable: true),
                    IDSala = table.Column<int>(name: "ID_Sala", type: "int", nullable: true),
                    IDKierunek = table.Column<int>(name: "ID_Kierunek", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Przedmio__B4EDE03199A8138C", x => x.IDTowaru);
                    table.ForeignKey(
                        name: "FK__Przedmiot__ID_Ki__3F466844",
                        column: x => x.IDKierunek,
                        principalTable: "Kierunek",
                        principalColumn: "ID_Kierunek");
                    table.ForeignKey(
                        name: "FK__Przedmiot__ID_Pr__3D5E1FD2",
                        column: x => x.IDProwadzacy,
                        principalTable: "Prowadzacy",
                        principalColumn: "ID_Prowadzacy");
                    table.ForeignKey(
                        name: "FK__Przedmiot__ID_Sa__3E52440B",
                        column: x => x.IDSala,
                        principalTable: "Sala",
                        principalColumn: "ID_Sala");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Przedmiot_ID_Kierunek",
                table: "Przedmiot",
                column: "ID_Kierunek");

            migrationBuilder.CreateIndex(
                name: "IX_Przedmiot_ID_Prowadzacy",
                table: "Przedmiot",
                column: "ID_Prowadzacy");

            migrationBuilder.CreateIndex(
                name: "IX_Przedmiot_ID_Sala",
                table: "Przedmiot",
                column: "ID_Sala");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Przedmiot");

            migrationBuilder.DropTable(
                name: "Kierunek");

            migrationBuilder.DropTable(
                name: "Prowadzacy");

            migrationBuilder.DropTable(
                name: "Sala");
        }
    }
}
