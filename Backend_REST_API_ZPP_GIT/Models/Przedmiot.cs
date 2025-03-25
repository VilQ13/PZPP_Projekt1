using System;
using System.Collections.Generic;

namespace Backend_REST_API_ZPP.Models;

public partial class Przedmiot
{
    public int IdTowaru { get; set; }

    public string Nazwa { get; set; } = null!;

    public string? Typ { get; set; }

    public string? Skrot { get; set; }

    public int? IdProwadzacy { get; set; }

    public int? IdSala { get; set; }

    public int? IdKierunek { get; set; }

    public virtual Kierunek? IdKierunekNavigation { get; set; }

    public virtual Prowadzacy? IdProwadzacyNavigation { get; set; }

    public virtual Sala? IdSalaNavigation { get; set; }
}
