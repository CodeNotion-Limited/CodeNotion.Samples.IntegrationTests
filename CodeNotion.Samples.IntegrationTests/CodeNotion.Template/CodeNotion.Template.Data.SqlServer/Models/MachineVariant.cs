using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class MachineVariant : IEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int MachineId { get; set; }
    public Machine? Machine { get; set; }
    public ICollection<Serial>? Serials { get; set; }
    public ICollection<MachineVariantArticleRelation>? MachineVariantArticleRelations { get; set; }
    public ICollection<MeshGroupArticleRelation>? MeshGroupArticleRelations { get; set; }
}