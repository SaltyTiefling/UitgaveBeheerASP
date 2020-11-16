using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uitgave_Beheer.Database;
using Uitgave_Beheer.Domain;
using Uitgave_Beheer.Models;

namespace Uitgave_Beheer.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseDatabase _expenseDatabase;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ExpenseController(IExpenseDatabase expenseDatabase, IWebHostEnvironment hostEnvironment)
        {
            _expenseDatabase = expenseDatabase;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            List<ExpenseListItemViewModel> expenseList = _expenseDatabase.GetExpenses().OrderBy(s => s.Datum).Select(expense => new ExpenseListItemViewModel
            {
                Id = expense.Id,
                Omschrijving = expense.Omschrijving,
                Bedrag = expense.Bedrag,
                Datum = expense.Datum,
                Categorie = expense.Categorie
            }).ToList();

            return View(new ExpenseListViewModel
            {
                List = expenseList,
                Years = GetYears(),
                Highest = expenseList.Where(s => s.Bedrag == HoogsteBedrag(expenseList)).ToList(),
                Lowest = expenseList.Where(s => s.Bedrag == laagsteBedrag(expenseList)).ToList(),
                MostExpensiveDays = ExpensesPerDay(expenseList).Where(s => s.Value == ExpensesPerDay(expenseList).OrderBy(s => s.Value).Last().Value).ToDictionary(s => s.Key, s => s.Value),
                MostExpensiveCategories = CostsOfCategories(expenseList).Where(s => s.Value == CostsOfCategories(expenseList).OrderBy(s => s.Value).Last().Value).ToDictionary(s => s.Key, s => s.Value),
                CheapestCategories = CostsOfCategories(expenseList).Where(s => s.Value == CostsOfCategories(expenseList).OrderBy(s => s.Value).First().Value).ToDictionary(s => s.Key, s => s.Value)

            });
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index([FromForm] ExpenseListViewModel form)
        {
            IEnumerable<Expense> selections = _expenseDatabase.GetExpenses();
            if (form.SelectedMonth != 0)
            {
                selections = selections.Where(s => s.Datum.Month == form.SelectedMonth);
            }
            if (form.SelectedYear != 0)
            {
                selections = selections.Where(s => s.Datum.Year == form.SelectedYear);
            }
            var expenseList = selections.Select(expense => new ExpenseListItemViewModel
            {
                Id = expense.Id,
                Omschrijving = expense.Omschrijving,
                Bedrag = expense.Bedrag,
                Datum = expense.Datum,
                Categorie = expense.Categorie
            }).ToList();

            return View(new ExpenseListViewModel
            {
                List = expenseList,
                Years = GetYears(),
                Highest = expenseList.Where(s => s.Bedrag == HoogsteBedrag(expenseList)).ToList(),
                Lowest = expenseList.Where(s => s.Bedrag == laagsteBedrag(expenseList)).ToList(),
                MostExpensiveDays = ExpensesPerDay(expenseList).Where(s => s.Value == ExpensesPerDay(expenseList).OrderBy(s => s.Value).Last().Value).ToDictionary(s => s.Key, s => s.Value),
                MostExpensiveCategories = CostsOfCategories(expenseList).Where(s => s.Value == CostsOfCategories(expenseList).OrderBy(s => s.Value).Last().Value).ToDictionary(s => s.Key, s => s.Value),
                CheapestCategories = CostsOfCategories(expenseList).Where(s => s.Value == CostsOfCategories(expenseList).OrderBy(s => s.Value).First().Value).ToDictionary(s => s.Key, s => s.Value),
                SelectedMonth = form.SelectedMonth,
                SelectedYear = form.SelectedYear
            });
        }
        private double HoogsteBedrag(List<ExpenseListItemViewModel> list)
        {
            return list.Select(s => s.Bedrag).OrderBy(s => s).ToList().Last();
        }
        private double laagsteBedrag(List<ExpenseListItemViewModel> list)
        {
            return list.Select(s => s.Bedrag).OrderBy(s => s).ToList().First();
        }

        private Dictionary<string, double> CostsOfCategories(List<ExpenseListItemViewModel> list)
        {
            Dictionary<string, double> catagorieExpenses = new Dictionary<string, double>();
            foreach (ExpenseListItemViewModel item in list)
            {
                if (item.Categorie != string.Empty)
                {
                    if (catagorieExpenses.ContainsKey(item.Categorie))
                    {
                        catagorieExpenses[item.Categorie] += item.Bedrag;
                    }
                    else
                    {
                        catagorieExpenses.Add(item.Categorie, item.Bedrag);
                    }
                }
            }
            return catagorieExpenses;
        }

        private Dictionary<DateTime, double> ExpensesPerDay(List<ExpenseListItemViewModel> list)
        {
            Dictionary<DateTime, double> expensesListPerDay = new Dictionary<DateTime, double>();

            foreach (ExpenseListItemViewModel item in list)
            {
                if (item.Datum.ToShortDateString() != string.Empty)
                {
                    if (expensesListPerDay.ContainsKey(item.Datum))
                    {
                        expensesListPerDay[item.Datum] += item.Bedrag;
                    }
                    else
                    {
                        expensesListPerDay.Add(item.Datum, item.Bedrag);
                    }
                }
            }

            return expensesListPerDay;
        }
        private List<int> GetYears()
        {
            List<int> answer = new List<int>();

            foreach (Expense Expense in _expenseDatabase.GetExpenses())
            {
                if (!answer.Contains(Expense.Datum.Year))
                {
                    answer.Add(Expense.Datum.Year);
                }
            }
            return answer;
        }
        public IActionResult Create()
        {
            return View(new ExpenseCreateViewModel() { Datum = DateTime.Now });
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(ExpenseCreateViewModel expense)
        {
            if (!TryValidateModel(expense))
            {
                return View(expense);
            }

            Expense newExpense = new Expense
            {
                Omschrijving = expense.Omschrijving,
                Categorie = expense.Categorie,
                Bedrag = expense.Bedrag,
                Datum = expense.Datum
            };

            if (expense.Photo != null)
            {
                string uniqueFileName = UploadPhoto(expense.Photo);

                newExpense.PhotoUrl = "/expense-pics/" + uniqueFileName;
            }

            _expenseDatabase.Insert(newExpense);
            return RedirectToAction(nameof(Index));
        }
        private string UploadPhoto(IFormFile photo)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            string pathName = Path.Combine(_hostEnvironment.WebRootPath, "expense-pics");
            string fileNameWithPath = Path.Combine(pathName, uniqueFileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return uniqueFileName;
        }
        private void DeletePhoto(string photoUrl)
        {
            string path = Path.Combine(_hostEnvironment.WebRootPath, photoUrl.Substring(1));
            System.IO.File.Delete(path);
        }

        public IActionResult Delete([FromRoute] int id)
        {
            _expenseDatabase.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit([FromRoute] int id)
        {
            var expense = _expenseDatabase.GetExpense(id);
            return View(new ExpenseEditViewModel
            {
                Omschrijving = expense.Omschrijving,
                photoUrl = expense.PhotoUrl,
                Bedrag = expense.Bedrag,
                Categorie = expense.Categorie,
                Datum = expense.Datum
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, [FromForm] ExpenseEditViewModel form)
        {
            if (form.Photo != null)
            {
                DeletePhoto(form.photoUrl);
                string uniqueFileName = UploadPhoto(form.Photo);
                form.photoUrl = "/expense-pics/" + uniqueFileName;
            }

            _expenseDatabase.Update(id, new Expense
            {
                Omschrijving = form.Omschrijving,
                Bedrag = form.Bedrag,
                Categorie = form.Categorie,
                Datum = form.Datum,
                PhotoUrl = form.photoUrl
            });
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail([FromRoute] int id)
        {
            var expense = _expenseDatabase.GetExpense(id);
            return View(new ExpenseDetailViewModel
            {
                Omschrijving = expense.Omschrijving,
                Bedrag = expense.Bedrag,
                Categorie = expense.Categorie,
                Datum = expense.Datum,
                PhotoUrl = expense.PhotoUrl
            });
        }
    }
}
