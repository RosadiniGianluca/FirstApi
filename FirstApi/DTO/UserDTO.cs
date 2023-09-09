namespace FirstApi.DTO
{
    // DTO: Data Transfer Object (oggetto che trasferisce dati), serve per trasferire dati da una classe all'altra

    public class UserDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Gender { get; set; }
    }
}
