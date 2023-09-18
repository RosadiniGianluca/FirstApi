using System.ComponentModel.DataAnnotations;

namespace FirstApi.DTO
{
    public class AddUserRequest
    {
        [Required(ErrorMessage = "Il campo 'FirstName' è obbligatorio.")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Il campo 'LastName' è obbligatorio.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Il campo 'Username' è obbligatorio.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Il campo 'Password' è obbligatorio.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Il campo 'EnrollmentDate' è obbligatorio.")]
        public DateTime EnrollmentDate { get; set; }
        [Required(ErrorMessage = "Il campo 'Gender' è obbligatorio.")]
        public int Gender { get; set; }
        [Required(ErrorMessage = "Il campo 'WorkId' è obbligatorio.")]
        public int WorkId { get; set; }
    }
}
