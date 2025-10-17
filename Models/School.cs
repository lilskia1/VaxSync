// Models/School.cs
public class School
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public ICollection<Student> Students { get; set; } = new List<Student>();
}

// Models/Student.cs
public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SchoolId { get; set; } = default!;   // <-- string to match School.Id
    public School School { get; set; } = default!;     // nav
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public bool IsCompliant { get; set; }
}
