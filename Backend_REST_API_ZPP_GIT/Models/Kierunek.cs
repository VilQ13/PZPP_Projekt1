using System;
using System.Collections.Generic;

namespace Backend_REST_API_ZPP.Models;

public partial class Kierunek
{
    public int IdKierunek { get; set; }

    public string? Nazwa { get; set; }

    public string? TypKierunku { get; set; }

    public virtual ICollection<Przedmiot> Przedmiots { get; } = new List<Przedmiot>();
}
