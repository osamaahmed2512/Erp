using Microsoft.AspNetCore.Identity;


namespace Domain.Entities
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImgUrl { get; set; }
        public DateTime? EmailConfirmationExpiry { get; set; }
        public ICollection<Company> OwnedCompanies { get; set; }
    }
}
