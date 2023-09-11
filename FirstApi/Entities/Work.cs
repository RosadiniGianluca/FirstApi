namespace FirstApi.Entities
{
    public class WorkEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Company { get; set; } = null!;
    }

   
    public class WorkModel
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
    }
}
