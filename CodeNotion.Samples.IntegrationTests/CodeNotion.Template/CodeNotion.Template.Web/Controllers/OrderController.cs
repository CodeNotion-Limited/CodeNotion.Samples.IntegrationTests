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

[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<Order>> GetOdata([FromQuery] GetOrderQuery filters, [OpenApiIgnore] ODataQueryOptions<Order> queryOptions, [FromQuery] OrderJoins[]? joins = null) =>
        _mediator.Send(filters with {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{guid:guid}")]
    public Task<Order?> GetById(Guid guid, [FromQuery] OrderJoins[]? joins = null) =>
        _mediator.Send(new GetOrderByGuidQuery {Guid = guid, Joins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpPut]
    public Task<Order> Upsert([FromBody] Order entity) =>
        _mediator.Send(new UpsertEntityCommand<Order>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<Order>(id), HttpContext.RequestAborted);
}