using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using NSwag.Annotations;
using CodeNotion.Template.Business.Cqrs.Query;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Odata;

namespace CodeNotion.Template.Web.Controllers;

[Route("api/mesh-group-article-relation")]
public class MeshGroupArticleRelationController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeshGroupArticleRelationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<MeshGroupArticleRelation>> GetOdata([OpenApiIgnore] ODataQueryOptions<MeshGroupArticleRelation> queryOptions, [FromQuery] MeshGroupArticleRelationJoins[]? joins = null) =>
        _mediator.Send(new GetOdataQuery<MeshGroupArticleRelation> { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<MeshGroupArticleRelation> GetById(int id, [FromQuery] MeshGroupArticleRelationJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<MeshGroupArticleRelation>(id) { QueryJoins = joins?.Cast<Enum>().ToArray() });
}