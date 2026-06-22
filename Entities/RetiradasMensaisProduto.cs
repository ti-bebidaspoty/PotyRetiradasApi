using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class RetiradasMensaisProduto
{
    public string RetiradaMensalProdutoId { get; set; } = null!;

    public string RetiradaMensalId { get; set; } = null!;

    public string ProdutoId { get; set; } = null!;

    public int Quantidade { get; set; }

    public virtual Produto Produto { get; set; } = null!;

    public virtual RetiradasMensai RetiradaMensal { get; set; } = null!;
}
