using Domain.Entities;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Company
{
    public class CompanyDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? TaxNumber { get; set; }
        public string? CommercialRegistration { get; set; }
        public string? Website { get; set; }
        public string Status { get; set; } 
        public string OwnerName { get; set; }
    }
}
