using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SomokatAPI;
public partial class UserAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;
    public string? name { get; set; } 
    public string? surname{ get; set; } 

    public int? Bonus { get; set; }
    public int? balance { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
