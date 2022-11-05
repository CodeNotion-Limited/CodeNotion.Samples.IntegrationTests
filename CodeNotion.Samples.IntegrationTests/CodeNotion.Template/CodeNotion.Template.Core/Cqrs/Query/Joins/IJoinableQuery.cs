using System;

namespace CodeNotion.Template.Business.Cqrs.Query.Joins;

public interface IJoinableQuery
{
    public Enum[]? QueryJoins { get; init; }
}