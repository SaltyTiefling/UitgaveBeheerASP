using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Uitgave_Beheer.Models
{
    public class ExpenseListViewModel
    {

        public List<ExpenseListItemViewModel> List { get; set; }
        [Required]
        public int SelectedMonth { get; set; }
        [Required]
        public int SelectedYear { get; set; }
        public List<int> Years { get; set; }
        public Dictionary<string, double> MostExpensiveCategories { get; set; }
        public Dictionary<string, double> CheapestCategories { get; set; }
        public Dictionary<DateTime, double> MostExpensiveDays { get; set; }
        public List<ExpenseListItemViewModel> Highest { get; set; }
        public List<ExpenseListItemViewModel> Lowest { get; set; }

    }
}
