using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Uitgave_Beheer.Models
{
    public class ExpenseCreateViewModel
    {
        [DisplayName("Omschrijving")]
        [Required]
        public string Omschrijving { get; set; }

        [DisplayName("Datum")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [DisplayName("Bedrag")]
        [DataType(DataType.Currency)]
        [Required, Range(0, double.MaxValue)]
        public double Bedrag { get; set; }

        [DisplayName("Foto")]
        public IFormFile Photo { get; set; }

        [DisplayName("Categorie")]
        public string Categorie { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem(),
            new SelectListItem(){Text="Investment", Value="Investment" },
            new SelectListItem(){Text="Lonen", Value="Lonen" },
            new SelectListItem(){Text="Aankoop", Value="Aankoop" },
            new SelectListItem(){Text="Voorzieningen", Value="Voorzieningen" }
        };
    }
}
