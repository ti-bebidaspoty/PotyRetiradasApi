using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class ConfiguracoesMensaisProduto
{
    public string ConfiguracaoMensalId { get; set; } = null!;

    public string AnoMes { get; set; } = null!;

    public int UnidadeId { get; set; }

    public string ProdutoId { get; set; } = null!;

    public int Minimo { get; set; }

    public int Maximo { get; set; }

    public virtual Produto Produto { get; set; } = null!;

    public virtual Unidade Unidade { get; set; } = null!;
}
