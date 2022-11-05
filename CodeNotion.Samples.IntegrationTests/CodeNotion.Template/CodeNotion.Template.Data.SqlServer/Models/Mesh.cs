using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Mesh : IEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int MachineId { get; set; }
    public Machine? Machine { get; set; }
    public int? MeshGroupId { get; set; }
    public MeshGroup? MeshGroup { get; set; }
    public IEnumerable<OrderMesh>? OrderMeshes { get; set; }
}