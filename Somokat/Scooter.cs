using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Somokat;

public partial class Scooter
{
    public int Id { get; set; }

    public NpgsqlPoint Location { get; set; }

    public int? BatteryLevel { get; set; }

    public int? IdStatus { get; set; }

    public virtual Status? IdStatusNavigation { get; set; }

    public virtual ICollection<LocationJournal> LocationJournals { get; set; } = new List<LocationJournal>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
