using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetArticleRelationComputedQuery : BaseGetOdataQuery<ArticleRelation>, IRequest<ManagedPageResult<ArticleRelation>>
{
    public int? MeshGroupId { get; init; }
    public int? MachineAccessoryId { get; init; }
    public int? Serial { get; init; }
}

internal class GetArticleRelationComputedQueryHandler : BaseGetOdataQueryHandler<ArticleRelation>, IRequestHandler<GetArticleRelationComputedQuery, ManagedPageResult<ArticleRelation>>
{
    public GetArticleRelationComputedQueryHandler(CodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual Task<ManagedPageResult<ArticleRelation>> Handle(GetArticleRelationComputedQuery request, CancellationToken ct)
    {
        var query = Context.Set<ArticleRelation>().AsQueryable();
        if (request.Serial is not null)
        {
            query = query.Where(x => x.MachineVariantArticleRelations!.Any(mva => mva.MachineVariant!.Serials!.Any(s => s.SerialNumber == request.Serial)));
        }

        if (request.MeshGroupId is not null)
        {
            query = query.Where(x => x.MeshGroupArticleRelations!.Any(mg => mg.MeshGroupId == request.MeshGroupId));
        }

        if (request.MachineAccessoryId is not null)
        {
            query = query.Where(x => x.MachineAccessoryArticleRelations!.Any(ma => ma.MachineAccessoryId == request.MachineAccessoryId));
        }

        return ToPagedResult(query, request);
    }
}