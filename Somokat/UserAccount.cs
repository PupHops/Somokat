using System;
using System.Collections.Generic;

namespace Somokat;
//dsaasgsfhgsdhds
public partial class UserAccount
{
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Bonus { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
