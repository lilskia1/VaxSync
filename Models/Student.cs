namespace VaxSync.Web.Models;

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SchoolId { get; set; } = default!;
    public School School { get; set; } = default!;

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }

    public bool IsCompliant { get; set; }  // you can compute this later instead of storing

    public ICollection<StudentVaccine> Vaccines { get; set; } = new List<StudentVaccine>();
}
