using System.Diagnostics.CodeAnalysis;

namespace CodeNotion.Template.Data.SqlServer.Enums;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ArticleTypeEnum
{
    UT, // Prototype,
    FT, // TODO comment
    AC, // Purchased,
    PF, // Finished,
    SL, // Semifinished
}