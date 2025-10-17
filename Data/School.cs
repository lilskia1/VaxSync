namespace VaxSync.Web.Data;

public class School
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public ICollection<VaxSync.Web.Models.Student> Students { get; set; } = new List<VaxSync.Web.Models.Student>();
}
