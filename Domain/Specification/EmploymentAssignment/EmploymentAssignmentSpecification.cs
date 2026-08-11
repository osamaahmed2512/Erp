namespace Domain.Specification.EmploymentAssignment
{
    public class EmploymentAssignmentSpecification : BaseSpecifications<Entities.EmploymentAssignment>
    {
        public EmploymentAssignmentSpecification(Guid employeeId)
            : base(a => a.EmployeeId == employeeId)
        {
            AddOrderByDescending(a => a.EffectiveFrom);
        }

        public EmploymentAssignmentSpecification(Guid employeeId, DateOnly effectiveDate)
            : base(a => a.EmployeeId == employeeId &&
                        a.EffectiveFrom <= effectiveDate &&
                        (!a.EffectiveTo.HasValue || a.EffectiveTo.Value >= effectiveDate))
        {
            AddOrderByDescending(a => a.EffectiveFrom);
        }
    }
}
