using System.ComponentModel.DataAnnotations;

namespace E_Indeks.Models
{
    public class Korisnik
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public int IsActive { get; set; }
        public int IsAdmin { get; set; }

    }
}
