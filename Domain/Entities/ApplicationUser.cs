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
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }  
        public DateTime SessionExpiryTime { get; set; }
        public bool IsExpired => DateTime.UtcNow >= RefreshTokenExpiryTime;
        public bool SessionExpired => DateTime.UtcNow > SessionExpiryTime;
    }
}
