using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class Unidade
{
    public int UnidadeId { get; set; }

    public string Descricao { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Colaboradore> Colaboradores { get; set; } = new List<Colaboradore>();

    public virtual ICollection<ConfiguracoesMensaisProduto> ConfiguracoesMensaisProdutos { get; set; } = new List<ConfiguracoesMensaisProduto>();

    public virtual ICollection<RetiradasMensai> RetiradasMensais { get; set; } = new List<RetiradasMensai>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
