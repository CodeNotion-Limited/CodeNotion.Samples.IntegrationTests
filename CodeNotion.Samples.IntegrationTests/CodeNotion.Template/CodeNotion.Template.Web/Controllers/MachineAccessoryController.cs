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

[Route("api/machine-accessory")]
public class MachineAccessoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public MachineAccessoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<MachineAccessory>> GetOdata([OpenApiIgnore] ODataQueryOptions<MachineAccessory> queryOptions, [FromQuery] MachineAccessoryJoins[]? joins = null) =>
        _mediator.Send(new GetOdataQuery<MachineAccessory> { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<MachineAccessory> GetById(int id, [FromQuery] MachineAccessoryJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<MachineAccessory>(id) { QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpPut]
    public Task<MachineAccessory> Upsert([FromBody] MachineAccessory? entity) =>
        _mediator.Send(new UpsertEntityCommand<MachineAccessory>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<MachineAccessory>(id), HttpContext.RequestAborted);
}