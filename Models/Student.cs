namespace VaxSync.Web.Models
{
    public class Student
    {
        public int Id { get; set; }  // ID único del estudiante (clave primaria)
        public string FullName { get; set; } = string.Empty;  // Nombre completo
        public string SchoolId { get; set; } = string.Empty;  // ID escolar o matrícula
        public string DateOfBirth { get; set; } = string.Empty; // Fecha de nacimiento
        public string Gender { get; set; } = string.Empty; // Género (opcional: Male/Female/Other)
    }
}
