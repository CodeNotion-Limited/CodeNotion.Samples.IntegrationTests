using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class MachineAccessory : IEntity
{
    public int Id { get; set; }
    public int AccessoryId { get; set; }
    public int MachineId { get; set; }
    public Machine? Machine { get; set; }
    public Accessory? Accessory { get; set; }
    public string? Rules { get; set; }
    public IEnumerable<MachineAccessoryArticleRelation>? MachineAccessoryArticleRelations { get; set; }
}