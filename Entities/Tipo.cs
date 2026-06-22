using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class Tipo
{
    public string TipoId { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public int QuantidadeRetirada { get; set; }

    public virtual ICollection<Colaboradore> Colaboradores { get; set; } = new List<Colaboradore>();
}
