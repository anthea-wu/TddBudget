namespace TddBudget;

public class BudgetService
{
    private IBudgetRepo _budgetRepo;

    public decimal Query(DateTime start, DateTime end)
    {
        var budgets = _budgetRepo.GetAll();
        var startMonth = start.ToString("yyyyMM");
        var budget = budgets
            .FirstOrDefault(x => x.YearMonth == startMonth) ?? new Budget() { Amount = 0 };

        var totalDays = (end - start).TotalDays;
        
        var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);

        return (decimal)(budget.Amount / daysInMonth * totalDays);
    }
}

internal interface IBudgetRepo
{
    List<Budget> GetAll();
}

internal class Budget
{
    public string YearMonth { get; set; }
    public int Amount { get; set; }
}