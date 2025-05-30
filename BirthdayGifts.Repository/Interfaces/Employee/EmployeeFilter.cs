using System;

namespace BirthdayGifts.Repository.Interfaces
{
    public class EmployeeFilter
    {
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}