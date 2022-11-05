using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class ModelFile : IEntity
{
    public int Id { get; set; }
    public string? NomeFile { get; set; }
    public string? NomeOriginario { get; set; }
    public string? FilePath { get; set; }
    public Machine? Machine { get; set; }
}