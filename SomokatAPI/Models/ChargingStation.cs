using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace SomokatAPI;

public partial class ChargingStation
{
    public int Id { get; set; }

    public NpgsqlPoint Location { get; set; }

    public int? AvailableChargers { get; set; }
}
