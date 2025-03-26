using System;
using System.Collections.Generic;

namespace Backend_REST_API_ZPP.Models;

public partial class Sala
{
    public int IdSala { get; set; }

    public string Budynek { get; set; } = null!;

    public string Pomieszczenie { get; set; } = null!;

    public virtual ICollection<Przedmiot> Przedmiots { get; } = new List<Przedmiot>();
}
