using E_indeks.Models;
using E_Indeks.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Indeks.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        //za da mozheme po lesno da koristime vo bazata na podatoci za Korisnici
        public DbSet<Korisnik> Korisnici { get; set; }
        //za da mozheme po lesno da pristapuvame do tabelata na gradovi
        public DbSet<Gradovi> Gradovi { get; set; }
        //za da mozheme po lesno da pristauvame do tabelata na studenti
        public DbSet<Student> Students { get; set; }
        public DbSet<Predmeti> Predmetis { get; set; }



    }
}
