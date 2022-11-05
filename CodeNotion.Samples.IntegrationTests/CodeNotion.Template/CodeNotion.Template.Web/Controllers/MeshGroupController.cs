using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using NSwag.Annotations;
using CodeNotion.Template.Business.Cqrs.Commands;
using CodeNotion.Template.Business.Cqrs.Query;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Odata;

namespace CodeNotion.Template.Web.Controllers;

[Route("api/mesh-group")]
public class MeshGroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeshGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<MeshGroup>> GetOdata([FromQuery] GetMeshGroupQuery filters, [OpenApiIgnore] ODataQueryOptions<MeshGroup> queryOptions, [FromQuery] MeshGroupJoins[]? joins = null) =>
        _mediator.Send(filters with {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<MeshGroup> GetById(int id, MeshGroupJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<MeshGroup>(id) {QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpPut]
    public Task<MeshGroup> Upsert([FromBody] MeshGroup entity) =>
        _mediator.Send(new UpsertEntityCommand<MeshGroup>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<MeshGroup>(id), HttpContext.RequestAborted);
}