using System;

namespace BirthdayGifts.Services.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
} 