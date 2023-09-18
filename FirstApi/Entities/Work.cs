using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApi.Entities
{
    public class WorkEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Company { get; set; } = null!;
        public int Salary { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Company})";
        }
    }

   
    public class WorkModel
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
    }
}
