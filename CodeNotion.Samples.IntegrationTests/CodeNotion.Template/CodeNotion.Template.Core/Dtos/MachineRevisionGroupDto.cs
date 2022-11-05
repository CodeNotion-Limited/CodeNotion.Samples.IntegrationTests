using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models;

namespace CodeNotion.Template.Business.Dtos;

public class MachineRevisionGroupDto
{
    public string? Name { get; set; }
    public ICollection<MachineRevisionDto>? Children { get; set; }
}

public class MachineRevisionDto 
{
    public int RevisionNumber { get; set; }
    public Machine? Machine { get; set; }
}