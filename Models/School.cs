namespace VaxSync.Web.Models;

public class School
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";   // optional but handy, e.g., "S-001"
    public string Name { get; set; } = "";
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
