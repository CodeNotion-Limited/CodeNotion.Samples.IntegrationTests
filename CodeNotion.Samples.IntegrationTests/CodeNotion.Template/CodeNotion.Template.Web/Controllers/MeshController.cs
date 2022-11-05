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

[Route("api/mesh")]
public class MeshController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeshController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<Mesh>> GetOdata([FromQuery] GetMeshQuery filters, [OpenApiIgnore] ODataQueryOptions<Mesh> queryOptions, [FromQuery] MeshJoins[]? joins = null) =>
        _mediator.Send(filters with {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<Mesh> GetById(int id, [FromQuery] MeshJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<Mesh>(id) {QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpPut]
    public Task<Mesh> Upsert([FromBody] Mesh entity) =>
        _mediator.Send(new UpsertEntityCommand<Mesh>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<Mesh>(id), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpGet("order-available/{serialNumber:int}")]
    public Task<Mesh[]> GetOrderAvailableMeshes(int serialNumber) =>
        _mediator.Send(new GetOrderAvailableMeshesQuery {SerialNumber = serialNumber});
}