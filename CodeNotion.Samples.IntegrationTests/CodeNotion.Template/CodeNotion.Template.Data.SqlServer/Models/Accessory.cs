using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Accessory : IEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public ICollection<MachineAccessory>? MachineAccessories { get; set; }
    public IEnumerable<OrderAccessory>? OrderAccessories { get; set; }
}