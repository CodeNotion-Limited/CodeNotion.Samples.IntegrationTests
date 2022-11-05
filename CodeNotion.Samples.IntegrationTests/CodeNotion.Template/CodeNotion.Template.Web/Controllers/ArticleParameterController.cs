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

[Route("api/article-parameter")]
public class ArticleParameterController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleParameterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<ArticleParameter>> GetOdata([OpenApiIgnore] ODataQueryOptions<ArticleParameter> queryOptions, [FromQuery] ArticleParameterJoins[]? joins = null) =>
        _mediator.Send(new GetOdataQuery<ArticleParameter> { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<ArticleParameter> GetById(int id, [FromQuery] ArticleParameterJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<ArticleParameter>(id) { QueryJoins = joins?.Cast<Enum>().ToArray() });
}