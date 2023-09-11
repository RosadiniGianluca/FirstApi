namespace FirstApi.DTO
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Gender { get; set; }
        public int WorkId { get; set; }
    }
}
