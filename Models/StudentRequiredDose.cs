namespace VaxSync.Web.Models;

public class StudentRequiredDose
{
    public int Id { get; set; }

    public string StudentId { get; set; } = default!;
    public Student Student { get; set; } = default!;

    public int VaccineScheduleId { get; set; }
    public VaccineSchedule VaccineSchedule { get; set; } = default!;

    public int DoseNumber { get; set; }
    public DateTime DueDate { get; set; }

    public bool Completed { get; set; } = false; // updated when StudentVaccine is added
    public bool Overdue => !Completed && DueDate < DateTime.Today;
    public bool Imminent => !Completed && (DueDate - DateTime.Today).TotalDays <= 30;
}
