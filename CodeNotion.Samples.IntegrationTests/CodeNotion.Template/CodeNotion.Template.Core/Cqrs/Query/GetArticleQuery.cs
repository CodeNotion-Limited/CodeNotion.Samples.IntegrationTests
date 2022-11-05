using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetArticleQuery : BaseGetOdataQuery<Article>, IRequest<ManagedPageResult<Article>>
{
    public int? Serial { get; init; }
}

internal class GetArticleQueryHandler : BaseGetOdataQueryHandler<Article>, IRequestHandler<GetArticleQuery, ManagedPageResult<Article>>
{
    public GetArticleQueryHandler(FullCodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual async Task<ManagedPageResult<Article>> Handle(GetArticleQuery request, CancellationToken ct)
    {
        var query = Context.Set<Article>().AsQueryable();
        if (request.Serial is not null)
        {
            query = query.Where(x => x.ArticleChildRelations!.Any(m => m.MachineVariantArticleRelations!.Any(mv => mv.MachineVariant!.Serials!.Any(s => s.SerialNumber == request.Serial))) ||
                                     x.ArticleParentRelations!.Any(m => m.MachineVariantArticleRelations!.Any(mv => mv.MachineVariant!.Serials!.Any(s => s.SerialNumber == request.Serial))));
        }

        return await ToPagedResult(query, request);
    }
}