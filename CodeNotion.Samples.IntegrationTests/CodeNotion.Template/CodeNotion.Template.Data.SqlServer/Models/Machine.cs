using System;
using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Machine : IEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? Start { get; set; }
    public ICollection<Mesh>? Meshes { get; set; }
    public ICollection<ArticleReplacement>? ArticleReplacements { get; set; }
    public ICollection<MachineAccessory>? MachineAccessories { get; set; }
    public ICollection<MachineVariant>? MachineVariants { get; set; }
}