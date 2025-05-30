using System;

namespace BirthdayGifts.Repository.Interfaces
{
    public class EmployeeUpdate
    {
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}