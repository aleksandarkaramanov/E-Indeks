using System.ComponentModel.DataAnnotations;

namespace E_Indeks.Models
{
    public class Predmeti
    {
        [Key]
        public int Indeks { get; set; }
        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; }
        [Required]
        public int Grade { get; set; }
    }
}
