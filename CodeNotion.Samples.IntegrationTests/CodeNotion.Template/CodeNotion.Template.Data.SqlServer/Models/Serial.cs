using System;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Serial : IEntity
{
    public int Id { get; set; }
    public int SerialNumber { get; set; }
    public DateTime CreationDate { get; set; }
    public int MachineVariantId { get; set; }
    public MachineVariant? MachineVariant { get; set; }
}