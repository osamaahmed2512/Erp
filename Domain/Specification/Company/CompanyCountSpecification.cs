using Domain.Specification.Params;


namespace Domain.Specification.Company
{
    public class CompanyCountSpecification:CompanySpecification
    {
        public CompanyCountSpecification(CompanyPaginationParams paginationParams)
            :base(paginationParams)
        {
        }
    }
}
