using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ClienteID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ClienteID);
                });

            migrationBuilder.CreateTable(
                name: "Servico",
                columns: table => new
                {
                    ServicoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeServico = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Preco = table.Column<decimal>(type: "TEXT", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servico", x => x.ServicoID);
                });

            migrationBuilder.CreateTable(
                name: "Tecnico",
                columns: table => new
                {
                    TecnicoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Especializacao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataContratacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnico", x => x.TecnicoID);
                });

            migrationBuilder.CreateTable(
                name: "Animal",
                columns: table => new
                {
                    AnimalID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ClienteID = table.Column<int>(type: "INTEGER", nullable: false),
                    Especie = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Raca = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Peso = table.Column<decimal>(type: "TEXT", nullable: true),
                    ObservacoesMedicas = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animal", x => x.AnimalID);
                    table.ForeignKey(
                        name: "FK_Animal_Cliente_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Cliente",
                        principalColumn: "ClienteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtribuicaoTecnico",
                columns: table => new
                {
                    TecnicoID = table.Column<int>(type: "INTEGER", nullable: false),
                    ServicoID = table.Column<int>(type: "INTEGER", nullable: false),
                    NivelProficiencia = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataAtribuicao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtribuicaoTecnico", x => new { x.TecnicoID, x.ServicoID });
                    table.ForeignKey(
                        name: "FK_AtribuicaoTecnico_Servico_ServicoID",
                        column: x => x.ServicoID,
                        principalTable: "Servico",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtribuicaoTecnico_Tecnico_TecnicoID",
                        column: x => x.TecnicoID,
                        principalTable: "Tecnico",
                        principalColumn: "TecnicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendamento",
                columns: table => new
                {
                    AgendamentoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimalID = table.Column<int>(type: "INTEGER", nullable: false),
                    ServicoID = table.Column<int>(type: "INTEGER", nullable: false),
                    TecnicoID = table.Column<int>(type: "INTEGER", nullable: true),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PrecoFinal = table.Column<decimal>(type: "TEXT", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamento", x => x.AgendamentoID);
                    table.ForeignKey(
                        name: "FK_Agendamento_Animal_AnimalID",
                        column: x => x.AnimalID,
                        principalTable: "Animal",
                        principalColumn: "AnimalID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamento_Servico_ServicoID",
                        column: x => x.ServicoID,
                        principalTable: "Servico",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamento_Tecnico_TecnicoID",
                        column: x => x.TecnicoID,
                        principalTable: "Tecnico",
                        principalColumn: "TecnicoID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_AnimalID",
                table: "Agendamento",
                column: "AnimalID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_ServicoID",
                table: "Agendamento",
                column: "ServicoID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_TecnicoID",
                table: "Agendamento",
                column: "TecnicoID");

            migrationBuilder.CreateIndex(
                name: "IX_Animal_ClienteID",
                table: "Animal",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_AtribuicaoTecnico_ServicoID",
                table: "AtribuicaoTecnico",
                column: "ServicoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamento");

            migrationBuilder.DropTable(
                name: "AtribuicaoTecnico");

            migrationBuilder.DropTable(
                name: "Animal");

            migrationBuilder.DropTable(
                name: "Servico");

            migrationBuilder.DropTable(
                name: "Tecnico");

            migrationBuilder.DropTable(
                name: "Cliente");
        }
    }
}
