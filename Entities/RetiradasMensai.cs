using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class RetiradasMensai
{
    public string RetiradaMensalId { get; set; } = null!;

    public string AnoMes { get; set; } = null!;

    public int UnidadeId { get; set; }

    public string ColaboradorId { get; set; } = null!;

    public string UsuarioId { get; set; } = null!;

    public string Operador { get; set; } = null!;

    public DateTime DataHora { get; set; }

    public string? RetiradoPor { get; set; }

    public virtual Colaboradore Colaborador { get; set; } = null!;

    public virtual ICollection<RetiradasMensaisProduto> RetiradasMensaisProdutos { get; set; } = new List<RetiradasMensaisProduto>();

    public virtual Unidade Unidade { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
