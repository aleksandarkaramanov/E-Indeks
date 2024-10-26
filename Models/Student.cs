using E_Indeks.Models;
using System.ComponentModel.DataAnnotations;

namespace E_indeks.Models
{
    public class Student
    {
       
        [Key]
        public int Indeks { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public double Prosek { get; set; }


        [StringLength(50)]
        public string Grad { get; set; }
        [StringLength(50)]
        public string Kvota { get; set; }

    }
}


