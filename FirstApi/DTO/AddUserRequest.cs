namespace FirstApi.DTO
{
    public class AddUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Gender { get; set; }
    }
}
