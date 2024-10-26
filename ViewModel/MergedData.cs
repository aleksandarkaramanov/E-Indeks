using E_indeks.Models;
using E_Indeks.Models;

namespace E_Indeks.ViewModel
{
    public class MergedData
    {
        public PaginatedList<Student> Students { get; set; }
        public PaginatedList<Korisnik> Korisnici { get; set; }
    }
}
