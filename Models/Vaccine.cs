namespace VaxSync.Web.Models
{
    public class Vaccine
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";   // short label, e.g. "MMR"
        public string Name { get; set; } = "";   // full name
        public string Description { get; set; } = "";
    }
}
