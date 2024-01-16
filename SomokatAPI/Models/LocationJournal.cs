using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Somokat;

public partial class LocationJournal
{
    public DateTime? Time { get; set; }

    public int Id { get; set; }

    public int? ScooterId { get; set; }

    public NpgsqlPoint? Location { get; set; }

    public virtual Scooter? Scooter { get; set; }
}
