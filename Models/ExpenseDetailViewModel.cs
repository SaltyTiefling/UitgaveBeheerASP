using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Uitgave_Beheer.Models
{
    public class ExpenseDetailViewModel
    {
        public string Omschrijving { get; set; }

        public DateTime Datum { get; set; }

        public double Bedrag { get; set; }

        public string Categorie { get; set; }

        public string PhotoUrl { get; set; }
    }
}
