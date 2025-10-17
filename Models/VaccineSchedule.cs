namespace VaxSync.Web.Models
{
    public class VaccineSchedule
    {
        public int Id { get; set; }
        public int VaccineId { get; set; }
        public Vaccine Vaccine { get; set; } = default!;
        public string AgeRange { get; set; } = "";   // e.g. "12–15 months", "4–6 years"
        public int DoseNumber { get; set; }
        public bool CatchUpEligible { get; set; }    // true = green bars in table
    }
}
