

namespace Application.Dtos.Employee
{
    public class CreateEmployeeRequestDto
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
