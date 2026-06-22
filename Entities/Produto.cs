using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class Produto
{
    public string ProdutoId { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public bool Status { get; set; }

    public string? CodigoBarras { get; set; }

    public string? Imagem { get; set; }

    public virtual ICollection<ConfiguracoesMensaisProduto> ConfiguracoesMensaisProdutos { get; set; } = new List<ConfiguracoesMensaisProduto>();

    public virtual ICollection<RetiradasMensaisProduto> RetiradasMensaisProdutos { get; set; } = new List<RetiradasMensaisProduto>();
}
