namespace BirthdayGifts.Services.DTOs.Employee
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}