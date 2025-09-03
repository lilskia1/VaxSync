using System.ComponentModel.DataAnnotations.Schema;

namespace VaxSync.Web.Models
{
    [NotMapped]
    public class VaccineRecord
    {
        public string VaccineName { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;

        // Possible values: "Compliant", "Pending", "Not Compliant"
        public string Status { get; set; } = "Pending";
    }
}
