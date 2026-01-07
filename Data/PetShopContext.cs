using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetShop.Models;

namespace PetShop.Data;

/// <summary>
/// DbContext - Contexto de Dados da PetShop
/// 
/// Responsabilidades principais:
/// - Gerenciar o acesso e mapeamento das entidades para o banco de dados SQLite
/// - Configurar relacionamentos entre tabelas
/// - Executar queries e operações CRUD
/// 
/// Entidades gerenciadas:
/// - Cliente: Donos dos animais
/// - Animal: Animais de estimação registados
/// - Servico: Serviços oferecidos pela PetShop
/// - Agendamento: Agendamentos de serviços
/// - Tecnico: Funcionários/Técnicos da PetShop
/// - AtribuicaoTecnico: Relação N:N entre Técnico e Serviço
/// 
/// Banco de Dados: SQLite
/// String de Conexão: Configurada em appsettings.json
/// </summary>
public class PetShopContext : DbContext
{
    /// <summary>
    /// Construtor do contexto de dados
    /// Recebe as opções de configuração via injeção de dependência
    /// </summary>
    public PetShopContext(DbContextOptions<PetShopContext> options)
        : base(options)
    {
    }

    // ===== DbSets - Representam as tabelas do banco de dados =====

    /// <summary>
    /// Tabela de Clientes
    /// Armazena dados dos proprietários de animais
    /// </summary>
    public DbSet<Cliente> Clientes { get; set; } = default!;

    /// <summary>
    /// Tabela de Animais
    /// Armazena dados dos animais de estimação
    /// </summary>
    public DbSet<Animal> Animais { get; set; } = default!;

    /// <summary>
    /// Tabela de Serviços
    /// Armazena tipos de serviços oferecidos (banho, tosa, etc.)
    /// </summary>
    public DbSet<Servico> Servicos { get; set; } = default!;

    /// <summary>
    /// Tabela de Agendamentos
    /// Armazena agendamentos de serviços para animais
    /// </summary>
    public DbSet<Agendamento> Agendamentos { get; set; } = default!;

    /// <summary>
    /// Tabela de Técnicos
    /// Armazena dados dos funcionários/técnicos da PetShop
    /// </summary>
    public DbSet<Tecnico> Tecnicos { get; set; } = default!;

    /// <summary>
    /// Tabela de Atribuições de Técnico
    /// Tabela de junção para a relação N:N entre Técnico e Serviço
    /// Define quais os serviços que cada técnico está qualificado a fazer
    /// </summary>
    public DbSet<AtribuicaoTecnico> AtribuicaoTecnico { get; set; } = default!;

    /// <summary>
    /// Método chamado durante a criação do modelo de dados
    /// Permite configurar relacionamentos específicos e restrições
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Configuração de tabelas =====

        /// Configure a tabela de Cliente
        modelBuilder.Entity<Cliente>()
            .ToTable("Cliente");

        /// Configure a tabela de Animal
        modelBuilder.Entity<Animal>()
            .ToTable("Animal");

        /// Configure a tabela de Serviço
        modelBuilder.Entity<Servico>()
            .ToTable("Servico");

        /// Configure a tabela de Agendamento
        modelBuilder.Entity<Agendamento>()
            .ToTable("Agendamento");

        /// Configure a tabela de Técnico
        modelBuilder.Entity<Tecnico>()
            .ToTable("Tecnico");

        /// Configure a tabela de Atribuição de Técnico (N:N)
        modelBuilder.Entity<AtribuicaoTecnico>()
            .ToTable("AtribuicaoTecnico");

        // ===== Configuração de chaves compostas =====

        /// Define chave composta para AtribuicaoTecnico
        /// Uma dupla (TecnicoID, ServicoID) deve ser única
        /// Isto evita que o mesmo técnico seja atribuído duas vezes ao mesmo serviço
        modelBuilder.Entity<AtribuicaoTecnico>()
            .HasKey(at => new { at.TecnicoID, at.ServicoID });

        // ===== Configuração de relacionamentos e integridade referencial =====

        /// Relacionamento: Cliente -> Animal (1:N)
        /// Um cliente pode ter vários animais
        /// Exclusão em cascata: Se deletar cliente, deletam-se os seus animais
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Cliente)
            .WithMany(c => c.Animais)
            .HasForeignKey(a => a.ClienteID)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        /// Relacionamento: Animal -> Agendamento (1:N)
        /// Um animal pode ter vários agendamentos
        /// Exclusão em cascata: Se deletar animal, deletam-se os seus agendamentos
        modelBuilder.Entity<Agendamento>()
            .HasOne(ag => ag.Animal)
            .WithMany(a => a.Agendamentos)
            .HasForeignKey(ag => ag.AnimalID)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        /// Relacionamento: Servico -> Agendamento (1:N)
        /// Um serviço pode ter vários agendamentos
        /// Exclusão em cascata: Se deletar serviço, deletam-se os agendamentos desse serviço
        modelBuilder.Entity<Agendamento>()
            .HasOne(ag => ag.Servico)
            .WithMany(s => s.Agendamentos)
            .HasForeignKey(ag => ag.ServicoID)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        /// Relacionamento: Tecnico -> Agendamento (1:N)
        /// Um técnico pode ter vários agendamentos
        /// Exclusão sem cascata: Se deletar técnico, os agendamentos ficam sem técnico (NULL)
        /// Isto é importante para não perder histórico de agendamentos
        modelBuilder.Entity<Agendamento>()
            .HasOne(ag => ag.Tecnico)
            .WithMany(t => t.Agendamentos)
            .HasForeignKey(ag => ag.TecnicoID)
            .OnDelete(DeleteBehavior.SetNull);

        /// Relacionamento: Tecnico -> AtribuicaoTecnico (1:N)
        /// Um técnico pode ter várias atribuições (múltiplos serviços)
        /// Exclusão em cascata: Se deletar técnico, deletam-se as suas atribuições
        modelBuilder.Entity<AtribuicaoTecnico>()
            .HasOne(at => at.Tecnico)
            .WithMany(t => t.AtribuicoesTecnicos)
            .HasForeignKey(at => at.TecnicoID)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        /// Relacionamento: Servico -> AtribuicaoTecnico (1:N)
        /// Um serviço pode ter várias atribuições (múltiplos técnicos)
        /// Exclusão em cascata: Se deletar serviço, deletam-se as suas atribuições
        modelBuilder.Entity<AtribuicaoTecnico>()
            .HasOne(at => at.Servico)
            .WithMany(s => s.AtribuicoesTecnicos)
            .HasForeignKey(at => at.ServicoID)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
