using System.ComponentModel.DataAnnotations.Schema;

namespace VaxSync.Web.Models
{
    public class VaccineRecord
    {
        public int Id { get; set; }  // Clave primaria

        public int StudentId { get; set; }  // Clave foránea
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        public string VaccineName { get; set; } = string.Empty;  // Ej. Tdap, MMR
        public DateTime DateGiven { get; set; }  // Fecha de vacunación
        public string Status { get; set; } = string.Empty;  // Complete, Missing, Upcoming
    }
}
