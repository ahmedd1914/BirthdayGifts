namespace BirthdayGifts.Services.DTOs.Authentication
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public int? EmployeeId { get; set; }
    }
}