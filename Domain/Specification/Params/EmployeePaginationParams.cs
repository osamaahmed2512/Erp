namespace Domain.Specification.Params;

public sealed class EmployeePaginationParams : PaginationParams
{
    public Guid? CompanyId { get; set; }
}
