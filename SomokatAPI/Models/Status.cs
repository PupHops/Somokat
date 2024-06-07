using System;
using System.Collections.Generic;

namespace SomokatAPI;

public partial class Status
{
    public int Id { get; set; }

    public string Status1 { get; set; } = null!;

    public virtual ICollection<Scooter> Scooters { get; set; } = new List<Scooter>();
}
