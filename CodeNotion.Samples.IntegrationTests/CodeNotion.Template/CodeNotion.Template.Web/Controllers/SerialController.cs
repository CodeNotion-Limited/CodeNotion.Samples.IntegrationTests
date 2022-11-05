﻿using System;
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

[Route("api/serial")]
public class SerialController : ControllerBase
{
    private readonly IMediator _mediator;

    public SerialController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("odata")]
    public Task<ManagedPageResult<Serial>> GetOdata([FromQuery] GetSerialQuery filters, [OpenApiIgnore] ODataQueryOptions<Serial> queryOptions, [FromQuery] SerialJoins[]? joins = null) =>
        _mediator.Send(filters with {ODataOptions = queryOptions, QueryJoins = joins?.Cast<Enum>().ToArray()});

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public Task<Serial> GetById(int id, [FromQuery] SerialJoins[]? joins = null) =>
        _mediator.Send(new GetEntityByIdQuery<Serial>(id) {QueryJoins = joins?.Cast<Enum>().ToArray()});
}