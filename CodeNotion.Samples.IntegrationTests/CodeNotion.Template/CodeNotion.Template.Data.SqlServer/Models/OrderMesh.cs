using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class OrderMesh: IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MeshId { get; set; }
    public Order? Order { get; set; }
    public Mesh? Mesh { get; set; }
}