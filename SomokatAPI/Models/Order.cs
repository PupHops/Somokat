using System;
using System.Collections.Generic;

namespace SomokatAPI;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ScooterId { get; set; }

    public DateTime? OrderTime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Rating { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual Scooter? Scooter { get; set; }

    public virtual UserAccount? User { get; set; }
}
