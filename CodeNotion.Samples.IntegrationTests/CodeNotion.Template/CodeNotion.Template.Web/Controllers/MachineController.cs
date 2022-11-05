using System;
using System.Collections.Generic;
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
using CodeNotion.Template.Business.Dtos;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Odata;

namespace CodeNotion.Template.Web.Controllers;

[Route("api/machine")]
public class MachineController : ControllerBase
{
    private readonly IMediator _mediator;

    public MachineController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<Machine>> GetOdata([OpenApiIgnore] ODataQueryOptions<Machine> queryOptions, [FromQuery] MachineJoins[]? joins = null) =>
        _mediator.Send(new GetOdataQuery<Machine> {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<Machine> GetById(int id, [FromQuery] MachineJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<Machine>(id) {QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("revision")]
    public Task<ICollection<MachineRevisionGroupDto>> GetAllMachineRevision() =>
        _mediator.Send(new GetAllMachineRevisionQuery());

    [AllowAnonymous]
    [HttpPut]
    public Task<Machine> Upsert([FromBody] Machine? entity) =>
        _mediator.Send(new UpsertEntityCommand<Machine>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<Machine>(id), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpGet("serial/{serial:int}")]
    public Task<Machine?> GetMachineBySerial(int serial) =>
        _mediator.Send(new GetMachineBySerialQuery(serial), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpGet("name/{name}")]
    public Task<Machine?> GetMachineByName(string name) =>
        _mediator.Send(new GetMachineByNameQuery(name), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpGet("check/{id:int}")]
    public async Task<Boolean> CheckModelMeshAsync(int id, [FromQuery] ICollection<string> meshesIds)
    {
      var machine =  await _mediator.Send(new GetEntityByIdQuery<Machine>(id) {QueryJoins = new Enum[] {MachineJoins.MachineToMeshes}});
      if (machine.Meshes is null)
      {
          return false;
      }
      
      if (machine.Meshes.Count != meshesIds.Count)
      {
          return false;
      }

      return true;
    }
}