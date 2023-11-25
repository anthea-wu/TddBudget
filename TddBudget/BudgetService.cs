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
            var startBudgetAmount = StartBudgetAmount(start, budgets, true);
            sum += startBudgetAmount;

            var endBudgetAmount = StartBudgetAmount(end, budgets, false);
            sum += endBudgetAmount;
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

    private static decimal StartBudgetAmount(DateTime start, List<Budget> budgets, bool isStart)
    {
        var startMonth = start.ToString("yyyyMM");
        var startBudget = budgets.FirstOrDefault(x => x.YearMonth == startMonth) ?? new Budget() { Amount = 0 };
        var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
        int startMonthDays = 0;
        if (isStart)
        {
            startMonthDays = daysInMonth - start.Day + 1;
        }
        else
        {
            startMonthDays = start.Day;
        }

        var startBudgetAmount = (startBudget.Amount / (decimal)daysInMonth * startMonthDays);
        return startBudgetAmount;
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