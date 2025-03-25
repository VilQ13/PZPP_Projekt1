using System;
using System.Collections.Generic;

namespace Backend_REST_API_ZPP.Models;

public partial class Prowadzacy
{
    public int IdProwadzacy { get; set; }

    public string? Imie { get; set; }

    public string? Nazwisko { get; set; }

    public string? Skrot { get; set; }

    public string? Tytul { get; set; }

    public virtual ICollection<Przedmiot> Przedmiots { get; } = new List<Przedmiot>();
}
