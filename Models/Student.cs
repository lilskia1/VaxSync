namespace VaxSync.Web.Models
{
    public class Student
    {
        public int Id { get; set; }  // ID único del estudiante
        public string FullName { get; set; } = string.Empty;  // Nombre completo
        public string SchoolId { get; set; } = string.Empty;  // ID escolar o matrícula
        public string DateOfBirth { get; set; } = string.Empty; // Fecha de nacimiento
        public string Gender { get; set; } = string.Empty; // Género (opcional)
        public string SSN { get; set; } = string.Empty;
public List<VaccineRecord> Vaccines { get; set; } = new();


        // ✅ Propiedad para saber si está al día con las vacunas
        public bool IsCompliant { get; set; }
    }
}
