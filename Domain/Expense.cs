using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uitgave_Beheer.Domain
{
    public class Expense
    {
        public int Id { get; set; }
        public string Omschrijving { get; set; }
        public DateTime Datum { get; set; }
        public double Bedrag { get; set; }
        public string Categorie { get; set; }
        public string PhotoUrl { get; set; }

    }
}
