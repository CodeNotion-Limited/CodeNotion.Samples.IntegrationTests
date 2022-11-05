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

[Route("api/order-article-relation")]
public class OrderArticleRelationController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderArticleRelationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<OrderArticleRelation>> GetOdata([OpenApiIgnore] ODataQueryOptions<OrderArticleRelation> queryOptions, [FromQuery] OrderArticleRelationJoins[]? joins = null) =>
        _mediator.Send(new GetOdataQuery<OrderArticleRelation>() {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<OrderArticleRelation> GetById(int id, [FromQuery] OrderArticleRelationJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<OrderArticleRelation>(id) {QueryJoins = joins?.Cast<Enum>().ToArray()});
}