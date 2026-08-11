

using Domain.Enum;

namespace Application.Dtos.Employee
{
    public class CreateEmployeeRequestDto
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NationalityNumber { get; set; }
        public Guid NationalityId { get; set; }
        public string Address { get; set; }
        public MaritalStatus MartielStatus { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
    }
}
