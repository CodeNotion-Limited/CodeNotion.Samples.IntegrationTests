using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using NSwag.Annotations;
using CodeNotion.Template.Business.Cqrs.Query;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Business.Dtos;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Odata;

namespace CodeNotion.Template.Web.Controllers;

[Route("api/article-relation")]
public class ArticleRelationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleRelationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<ArticleRelation>> GetOdata([FromQuery] GetArticleRelationQuery filters, [OpenApiIgnore] ODataQueryOptions<ArticleRelation> queryOptions, [FromQuery] ArticleRelationJoins[]? joins = null) =>
        _mediator.Send(filters with { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<ArticleRelation> GetById(int id, [FromQuery] ArticleRelationJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<ArticleRelation>(id) { QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("tree-flat")]
    public Task<List<ArticleFlatNodeDto>> GetArticleFlatTree([FromQuery] GetArticleFlatTreeQuery filters, [FromQuery] ArticleRelationJoins[]? joins = null) =>
        _mediator.Send(filters with { QueryJoins = joins?.Cast<Enum>().ToArray() });

    [AllowAnonymous]
    [HttpGet("odata/computed")]
    public Task<ManagedPageResult<ArticleRelation>> GetArticleRelationComputed([FromQuery] GetArticleRelationComputedQuery filters, [OpenApiIgnore] ODataQueryOptions<ArticleRelation> queryOptions, [FromQuery] ArticleRelationJoins[]? joins = null) =>
        _mediator.Send(filters with { ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray() });
}