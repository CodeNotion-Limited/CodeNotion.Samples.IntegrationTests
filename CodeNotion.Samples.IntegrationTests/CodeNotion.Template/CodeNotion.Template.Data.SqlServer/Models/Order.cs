using System;
using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Order : IEntity
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string? Notes { get; set; }
    public DateTime CreationDate { get; set; }
    public int SerialId { get; set; }
    public Serial? Serial { get; set; }

    public ICollection<OrderMesh>? OrderMeshes { get; set; }
    public ICollection<OrderArticleRelation>? OrderArticlesRelations { get; set; }
    public ICollection<OrderAccessory>? OrderAccessories { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}