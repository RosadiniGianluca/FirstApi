using System.ComponentModel.DataAnnotations;
namespace FirstApi.DTO
{
    public class AddWorkRequest
    {
        [Required(ErrorMessage = "Il campo 'Name' è obbligatorio.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Il campo 'Company' è obbligatorio.")]
        public string? Company { get; set; }
        [Required(ErrorMessage = "Il campo 'Salary' è obbligatorio.")]
        public int Salary { get; set; }
    }
}
