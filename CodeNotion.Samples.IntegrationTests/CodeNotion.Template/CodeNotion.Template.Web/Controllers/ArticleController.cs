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

[Route("api/article")]
public class ArticleController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<Article>> GetOdata([FromQuery] GetArticleQuery filters, [OpenApiIgnore] ODataQueryOptions<Article> queryOptions, [FromQuery] ArticleJoins[]? joins = null) =>
        _mediator.Send(filters with { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<Article> GetById(int id, [FromQuery] ArticleJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<Article>(id) { QueryJoins = joins?.Cast<Enum>().ToArray() });
}