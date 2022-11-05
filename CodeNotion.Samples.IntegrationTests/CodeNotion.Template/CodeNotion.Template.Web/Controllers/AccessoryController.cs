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

[Route("api/accessory")]
public class AccessoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccessoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<Accessory>> GetOdata([FromQuery] GetAccessoryQuery filters, [OpenApiIgnore] ODataQueryOptions<Accessory> queryOptions, [FromQuery] AccessoryJoins[]? joins = null) =>
        _mediator.Send(filters with { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<Accessory> GetById(int id, [FromQuery] AccessoryJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<Accessory>(id) { QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpPut]
    public Task<Accessory> Upsert([FromBody] Accessory? entity) =>
        _mediator.Send(new UpsertEntityCommand<Accessory>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<Accessory>(id), HttpContext.RequestAborted);
}