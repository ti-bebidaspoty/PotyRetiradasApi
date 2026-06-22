using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class Colaboradore
{
    public string ColaboradorId { get; set; } = null!;

    public int? CodigoAlternativo { get; set; }

    public string Nome { get; set; } = null!;

    public int UnidadeId { get; set; }

    public bool Status { get; set; }

    public string? TipoId { get; set; }

    public virtual ICollection<RetiradasMensai> RetiradasMensais { get; set; } = new List<RetiradasMensai>();

    public virtual Tipo? Tipo { get; set; }

    public virtual Unidade Unidade { get; set; } = null!;
}
