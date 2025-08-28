namespace VaxSync.Web.Models
{
    public class AuditLog
    {
        public int Id { get; set; }  // Clave primaria
        public string User { get; set; } = string.Empty;  // Nombre o email del usuario
        public string Action { get; set; } = string.Empty;  // Descripción de la acción
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;  // Cuándo ocurrió
    }
}
