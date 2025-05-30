using System;

namespace BirthdayGifts.Services.DTOs.Employee
{
    public class EmployeeFilterDto
    {
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
} 