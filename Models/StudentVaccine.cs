namespace VaxSync.Web.Models;

public class StudentVaccine
{
    public int Id { get; set; }

    public string StudentId { get; set; } = default!;
    public Student Student { get; set; } = default!;

    public int VaccineId { get; set; }
    public Vaccine Vaccine { get; set; } = default!;

    public int DoseNumber { get; set; }       
    public DateTime? DateGiven { get; set; }  
}
