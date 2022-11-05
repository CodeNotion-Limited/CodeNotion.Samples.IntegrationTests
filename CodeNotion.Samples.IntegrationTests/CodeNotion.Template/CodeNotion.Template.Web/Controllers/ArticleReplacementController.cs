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

[Route("api/article-replacement")]
public class ArticleReplacementController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleReplacementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<ArticleReplacement>> GetOdata([OpenApiIgnore] ODataQueryOptions<ArticleReplacement> queryOptions, [FromQuery] ArticleReplacementJoins[]? joins = null) =>
        _mediator.Send(new GetOdataQuery<ArticleReplacement> {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<ArticleReplacement> GetById(int id, [FromQuery] ArticleReplacementJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<ArticleReplacement>(id) {QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpPut]
    public Task<ArticleReplacement> Upsert([FromBody] ArticleReplacement? entity) =>
        _mediator.Send(new UpsertEntityCommand<ArticleReplacement>(entity), HttpContext.RequestAborted);

    [AllowAnonymous]
    [HttpDelete("{id:int}")]
    public Task<int> Delete(int id) =>
        _mediator.Send(new DeleteEntityByIdCommand<ArticleReplacement>(id), HttpContext.RequestAborted);
}