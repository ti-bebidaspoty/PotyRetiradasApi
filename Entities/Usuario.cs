using System;
using System.Collections.Generic;

namespace PotyRetiradasApi.Entities;

public partial class Usuario
{
    public string UsuarioId { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public bool Administrador { get; set; }

    public bool Status { get; set; }

    public int UnidadeId { get; set; }

    public virtual ICollection<RetiradasMensai> RetiradasMensais { get; set; } = new List<RetiradasMensai>();

    public virtual Unidade Unidade { get; set; } = null!;
}
