using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Cqrs.Query;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Services;

public interface IOrderArticlesElaboratorService
{
    Task<(Order, ICollection<ArticleRelation>)> ElaborateAsync(Order order);
}

public class OrderArticlesElaboratorService : IOrderArticlesElaboratorService
{
    private readonly IMediator _mediator;
    private readonly FullCodeNotionTemplateContext _context;

    public OrderArticlesElaboratorService(IMediator mediator, FullCodeNotionTemplateContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task<(Order, ICollection<ArticleRelation>)> ElaborateAsync(Order order)
    {
        if (order.OrderMeshes is null || !order.OrderMeshes.Any() && (order.OrderAccessories is null || !order.OrderAccessories.Any()))
        {
            throw new ArgumentException("Articles in order cannot be null or empty");
        }

        var meshes = _context
            .Set<Mesh>()
            .AsNoTracking()
            .Where(mesh => order.OrderMeshes.Select(orderMesh => orderMesh.MeshId).Contains(mesh.Id))
            .ToArray();
        var articleRelations = new HashSet<ArticleRelation>();
        foreach (var meshGroupId in meshes.Select(mesh => mesh.MeshGroupId).Distinct().ToArray())
        {
            var listTmp = await _mediator
                .Send(new GetArticleRelationQuery
                {
                    MeshGroupId = meshGroupId,
                    Depth = 6,
                    SerialId = order.SerialId,
                    QueryJoins = new Enum[] {ArticleRelationJoins.ArticleRelationToChild}
                }).Then(x => x.Items);
            articleRelations.AddRange(listTmp);
        }

        var accessory = order.OrderAccessories?.Select(orderMesh => orderMesh.AccessoryId).ToArray() ?? Array.Empty<int>();
        foreach (var accessoryId in accessory)
        {
            var listTmp = await _mediator
                .Send(new GetArticleRelationQuery
                {
                    MachineAccessoryId = accessoryId,
                    Depth = 6,
                    SerialId = order.SerialId,
                    QueryJoins = new Enum[] {ArticleRelationJoins.ArticleRelationToChild}
                }).Then(x => x.Items);
            articleRelations.AddRange(listTmp);
        }

        var orderArticleRelations = new HashSet<ArticleRelation>();
        foreach (var articleRelation in articleRelations)
        {
            var orderArticleRelation = new OrderArticleRelation {OrderId = order.Id, ArticleRelationId = articleRelation.Id};
            _context.Add(orderArticleRelation);
            orderArticleRelations.Add(articleRelation);
        }

        await _context.SaveChangesAsync();
        return (order, orderArticleRelations);
    }
}