using Data.Contracts;
using Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mahak.Api.Models.Categories.Queries.Find;

public class GetByIdsQueryHandler: IRequestHandler<GetByIdsQuery, List<CategorySelectDto>>
{
    private readonly IRepository<Category> _repository;

    public GetByIdsQueryHandler(IRepository<Category> repository)
    {
        _repository = repository;
    }

    public Task<List<CategorySelectDto>> Handle(GetByIdsQuery request, CancellationToken cancellationToken)
    {
        return _repository.TableNoTracking.Where(i => request.CategoryIds.Contains(i.Id))
            .ProjectToType<CategorySelectDto>().ToListAsync(cancellationToken);
    }
}