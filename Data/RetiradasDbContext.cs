using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Data;

public partial class RetiradasDbContext : DbContext
{
    public RetiradasDbContext(DbContextOptions<RetiradasDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Colaboradore> Colaboradores { get; set; }

    public virtual DbSet<ConfiguracoesMensaisProduto> ConfiguracoesMensaisProdutos { get; set; }

    public virtual DbSet<Produto> Produtos { get; set; }

    public virtual DbSet<RetiradasMensai> RetiradasMensais { get; set; }

    public virtual DbSet<RetiradasMensaisProduto> RetiradasMensaisProdutos { get; set; }

    public virtual DbSet<Tipo> Tipos { get; set; }

    public virtual DbSet<Unidade> Unidades { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Colaboradore>(entity =>
        {
            entity.HasKey(e => e.ColaboradorId);

            entity.Property(e => e.ColaboradorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ColaboradorID");
            entity.Property(e => e.Nome)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TipoId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TipoID");
            entity.Property(e => e.UnidadeId).HasColumnName("UnidadeID");

            entity.HasOne(d => d.Tipo).WithMany(p => p.Colaboradores)
                .HasForeignKey(d => d.TipoId)
                .HasConstraintName("FK_Colaboradores_Tipos");

            entity.HasOne(d => d.Unidade).WithMany(p => p.Colaboradores)
                .HasForeignKey(d => d.UnidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Colaboradores_Unidades");
        });

        modelBuilder.Entity<ConfiguracoesMensaisProduto>(entity =>
        {
            entity.HasKey(e => e.ConfiguracaoMensalId);

            entity.Property(e => e.ConfiguracaoMensalId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ConfiguracaoMensalID");
            entity.Property(e => e.AnoMes)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProdutoId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("ProdutoID");
            entity.Property(e => e.UnidadeId).HasColumnName("UnidadeID");

            entity.HasOne(d => d.Produto).WithMany(p => p.ConfiguracoesMensaisProdutos)
                .HasForeignKey(d => d.ProdutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfiguracoesMensaisProdutos_Produtos");

            entity.HasOne(d => d.Unidade).WithMany(p => p.ConfiguracoesMensaisProdutos)
                .HasForeignKey(d => d.UnidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfiguracoesMensaisProdutos_Unidades");
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.Property(e => e.ProdutoId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("ProdutoID");
            entity.Property(e => e.CodigoBarras)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Imagem)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RetiradasMensai>(entity =>
        {
            entity.HasKey(e => e.RetiradaMensalId);

            entity.Property(e => e.RetiradaMensalId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RetiradaMensalID");
            entity.Property(e => e.AnoMes)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ColaboradorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ColaboradorID");
            entity.Property(e => e.DataHora).HasColumnType("datetime");
            entity.Property(e => e.Operador)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RetiradoPor)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnidadeId).HasColumnName("UnidadeID");
            entity.Property(e => e.UsuarioId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UsuarioID");

            entity.HasOne(d => d.Colaborador).WithMany(p => p.RetiradasMensais)
                .HasForeignKey(d => d.ColaboradorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetiradasMensais_Colaboradores");

            entity.HasOne(d => d.Unidade).WithMany(p => p.RetiradasMensais)
                .HasForeignKey(d => d.UnidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetiradasMensais_Unidades");

            entity.HasOne(d => d.Usuario).WithMany(p => p.RetiradasMensais)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetiradasMensais_Usuarios");
        });

        modelBuilder.Entity<RetiradasMensaisProduto>(entity =>
        {
            entity.HasKey(e => e.RetiradaMensalProdutoId);

            entity.Property(e => e.RetiradaMensalProdutoId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RetiradaMensalProdutoID");
            entity.Property(e => e.ProdutoId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("ProdutoID");
            entity.Property(e => e.RetiradaMensalId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RetiradaMensalID");

            entity.HasOne(d => d.Produto).WithMany(p => p.RetiradasMensaisProdutos)
                .HasForeignKey(d => d.ProdutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetiradasMensaisProdutos_Produtos");

            entity.HasOne(d => d.RetiradaMensal).WithMany(p => p.RetiradasMensaisProdutos)
                .HasForeignKey(d => d.RetiradaMensalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetiradasMensaisProdutos_RetiradasMensais");
        });

        modelBuilder.Entity<Tipo>(entity =>
        {
            entity.HasKey(e => e.TipoId).HasName("PK__Tipo__97099E97F12E3028");

            entity.Property(e => e.TipoId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TipoID");
            entity.Property(e => e.Descricao)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Unidade>(entity =>
        {
            entity.Property(e => e.UnidadeId)
                .ValueGeneratedNever()
                .HasColumnName("UnidadeID");
            entity.Property(e => e.Descricao)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.UsuarioId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UsuarioID");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SenhaHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UnidadeId).HasColumnName("UnidadeID");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Usuario");

            entity.HasOne(d => d.Unidade).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.UnidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Unidades");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
