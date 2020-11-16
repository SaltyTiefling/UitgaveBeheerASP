using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uitgave_Beheer.Domain;

namespace Uitgave_Beheer.Database
{
    public interface IExpenseDatabase
    {
        Expense Insert(Expense expense);
        IEnumerable<Expense> GetExpenses();
        Expense GetExpense(int id);
        void Delete(int id);
        void Update(int id, Expense expense);
    }

    public class ExpenseDatabase : IExpenseDatabase
    {
        private int _counter;
        private readonly List<Expense> _Expenses;

        public ExpenseDatabase()
        {
            if (_Expenses == null)
            {
                _Expenses = new List<Expense>();
            }
            Mockdata();
        }

        private void Mockdata()
        {
            Insert(new Expense{ Omschrijving = "Nvidia GeForce RTX 3080", Datum = DateTime.Parse("12/11/2020"), Categorie = "Investment", Bedrag = 699});
            Insert(new Expense { Omschrijving = "Printer", Datum = DateTime.Parse("15/01/2019"), Categorie = "Investment", Bedrag = 69.42 });
            Insert(new Expense { Omschrijving = "loon Ken", Datum = DateTime.Parse("20/12/2020"), Categorie = "Lonen", Bedrag = 8.80 });

        }

        public Expense GetExpense(int id)
        {
            return _Expenses.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Expense> GetExpenses()
        {
            return _Expenses;
        }

        public Expense Insert(Expense expense)
        {
            _counter++;
            expense.Id = _counter;
            _Expenses.Add(expense);
            return expense;
        }

        public void Delete(int id)
        {
            var Contact = _Expenses.SingleOrDefault(x => x.Id == id);
            if (Contact != null)
            {
                _Expenses.Remove(Contact);
            }
        }

        public void Update(int id, Expense updatedExpense)
        {
            var expense = _Expenses.SingleOrDefault(x => x.Id == id);
            if (expense != null)
            {
                expense.Omschrijving = updatedExpense.Omschrijving;
                expense.Bedrag = updatedExpense.Bedrag;
                expense.Datum = updatedExpense.Datum;
                expense.Categorie = updatedExpense.Categorie;
                expense.PhotoUrl = updatedExpense.PhotoUrl;
            }
        }
    }
}
