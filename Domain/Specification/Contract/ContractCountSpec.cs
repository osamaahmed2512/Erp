using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Contract
{
    public class ContractCountSpec:ContractSpecification
    {
        public ContractCountSpec(ContractPaginationParams paginationParams):base(paginationParams)
        {
            if (paginationParams.Status.HasValue)
                AddCriteria(c => c.Status == paginationParams.Status.Value);

            if (paginationParams.EmployeeId.HasValue)
                AddCriteria(c => c.EmployeeId == paginationParams.EmployeeId.Value);

            if (paginationParams.SearchWords.Any())
                AddCriteria(c =>
                    paginationParams.SearchWords.All(word =>
                        c.Employee.User.FirstName.ToLower().Contains(word) ||
                        c.Employee.User.LastName.ToLower().Contains(word)));
        }
    }
}
