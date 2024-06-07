using System;
using System.Collections.Generic;

namespace SomokatAPI;

public partial class Complaint
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public int? OrderId { get; set; }

    public DateTime? Time { get; set; }

    public string? ImageLink { get; set; }

    public int? Rating { get; set; }

    public virtual Order? Order { get; set; }
}
