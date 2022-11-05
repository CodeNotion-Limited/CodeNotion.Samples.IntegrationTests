using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class OrderAccessory: IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int AccessoryId { get; set; }
    public Order? Order { get; set; }
    public Accessory? Accessory { get; set; }
}