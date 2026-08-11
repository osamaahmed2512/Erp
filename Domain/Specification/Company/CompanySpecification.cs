
using Domain.Specification.Helper;
using Domain.Specification.Params;

using System.Linq.Expressions;

namespace Domain.Specification.Company
{
    public class CompanySpecification:BaseSpecifications<Domain.Entities.Company>
    {
        public CompanySpecification(string phone , string email)
            :base(x =>x.Phone==phone ||x.Email==email)
        {
            
        }
        public CompanySpecification(string phone, string email , Guid companyId)
        : base(x => (x.Phone == phone || x.Email == email)&&x.Id != companyId )
        {

        }
        public CompanySpecification(Guid Id)
            :base(x => x.Id == Id)
        {
            
        }
        public CompanySpecification(Guid Id, bool includeOwner=true)
         : base(x => x.Id == Id)
        {
            AddInclude(x => x.Owner);
        }
        public CompanySpecification(Guid? OwnerId ,bool owner=true)
            
        {
            if( OwnerId != null)
            {
                AddCriteria(x => x.OwnerId == OwnerId.Value);
            }
        }
        public CompanySpecification(CompanyPaginationParams Params)
        {
            Expression<Func<Domain.Entities.Company, bool>> criteria = c => true;
            criteria = criteria.AndAlso(e => e.Status != Enum.EntityStatus.Deleted);
            if (!string.IsNullOrWhiteSpace(Params.Search))
            {
                var s = Params.Search.Trim().ToLower();
                criteria = criteria.AndAlso(e => e.Name.Contains(s) ||
                        e.Email.Contains(s) ||
                        e.Phone.Contains(s)||
                        e.Owner.FirstName.Contains(s)||
                        (e.Owner.LastName != null && e.Owner.LastName.Contains(s))
                        );
            }
            if (Params.OwnerId != Guid.Empty)
            {
                criteria = criteria.AndAlso(e => e.OwnerId == Params.OwnerId);
            }
            AddCriteria(criteria); 

            AddOrderByDescending(e => e.CreatedAt);
        }
    }
}
