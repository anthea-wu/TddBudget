namespace TddBudget;

public class BudgetService
{
    private IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        var budgets = _budgetRepo.GetAll();
        decimal sum = 0;

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var endMonth = end.ToString("yyyyMM");
            var endBudget = budgets.FirstOrDefault(x => x.YearMonth == endMonth) ?? new Budget() { Amount = 0 };
            var endMonthDays = end.Day;
            var daysInEndMonth = DateTime.DaysInMonth(end.Year, end.Month);
            var endBudgetAmount = endBudget.Amount / (decimal)daysInEndMonth * endMonthDays;
            sum += endBudgetAmount;

            var startMonth = start.ToString("yyyyMM");
            var startBudget = budgets
                .FirstOrDefault(x => x.YearMonth == startMonth) ?? new Budget() { Amount = 0 };
            var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
            var totalDays = daysInMonth - start.Day + 1;
            var startBudgetAmount = (startBudget.Amount / (decimal)daysInMonth * totalDays);
            sum += startBudgetAmount;
        }
        else
        {
            var startMonth = start.ToString("yyyyMM");
            var startBudget = budgets
                .FirstOrDefault(x => x.YearMonth == startMonth) ?? new Budget() { Amount = 0 };

            var totalDays = (end - start).Days + 1;
            var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);

            var startBudgetAmount = (startBudget.Amount / (decimal)daysInMonth * totalDays);
            sum += startBudgetAmount;
        }

        return sum;
    }
}

public interface IBudgetRepo
{
    List<Budget> GetAll();
}

public class Budget
{
    public string YearMonth { get; set; }
    public int Amount { get; set; }
}