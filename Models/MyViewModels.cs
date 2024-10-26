using PagedList;

namespace E_Indeks.Models
{
    public class MyViewModels
    {
        public PaginatedList<E_indeks.Models.Student> myStudents { get; set; }
        public List<Predmeti> myPredmeti { get; set; }
        //во случај да некоја вредност зависи од друга
        public Dictionary<string, double> SubjectAverageGrades { get; set; }//на ваков начин се декларира речник
        public List<int> MostFrequentGrades { get; set; }
        public string CenterGrades { get; set; } // Add this property

    }
}
