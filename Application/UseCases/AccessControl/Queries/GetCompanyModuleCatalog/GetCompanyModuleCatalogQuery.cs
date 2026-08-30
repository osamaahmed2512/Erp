using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetCompanyModuleCatalog;

public sealed record GetCompanyModuleCatalogQuery(Guid ActorId)
    : IRequest<IReadOnlyList<CompanyModuleDto>>;
