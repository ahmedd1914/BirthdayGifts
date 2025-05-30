namespace BirthdayGifts.Services.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
} 