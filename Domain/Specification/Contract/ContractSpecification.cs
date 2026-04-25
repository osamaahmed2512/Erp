using Domain.Specification.Params;


namespace Domain.Specification.Contract
{
    public class ContractSpecification:BaseSpecifications<Domain.Entities.Contract>
    {
        public ContractSpecification(Guid id)
         : base(c => c.Id == id)
        {
        }
        public ContractSpecification( ContractPaginationParams paginationParams)
          
        {
            AddInclude(c => c.Employee);

            AddOrderByDescending(c => c.StartDate);
            ApplyPagination(
                (paginationParams.PageIndex - 1) * paginationParams.PageSize,
                paginationParams.PageSize);
        }
        public ContractSpecification(Guid employeeId, ContractPaginationParams paginationParams)
            : base(c => c.EmployeeId == employeeId)
        {
            if (paginationParams.Status.HasValue)
                AddCriteria(c => c.EmployeeId == employeeId && c.Status == paginationParams.Status.Value);
        }
    }
}
