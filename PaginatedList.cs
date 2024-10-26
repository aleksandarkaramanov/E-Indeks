using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace E_Indeks
{
    public class PaginatedList<T>:List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        //items: Листа од податоци од тип T кои ќе бидат прикажани на тековната страница.
        //count: Вкупниот број на податоци од тип T во целата листа.
        //pageIndex: Индексот на страницата.
        //pageSize: Големината на страницата (колку податоци ќе се прикажат на една страница).
        public PaginatedList(List<T> items,int count,int pageIndex,int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public static PaginatedList<T> Create(List<T> source,int pageIndex,int pageSize)
        {
            var count = source.Count;

            //СО Skip ги прексокнуваме првите елементи,(ова е за од кој емент да почнеме со прикажување)
            //со Таке ја земаме позицијата од каде треба да почнеме со прикажување

            //ако pageIndex=2 vo Skip ke imame((2-1)*5) toa ke ni bide 5,shto znachi prvite 5 elementi gi skokame
            //So Take gi zemam narednite 5 elementi i gi stavam vo items kako lista
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

    }
}
