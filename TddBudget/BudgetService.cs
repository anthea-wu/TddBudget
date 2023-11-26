namespace TddBudget;

public class Period
{
    public DateTime _end;
    public DateTime _start;

    public Period(DateTime start, DateTime end)
    {
        _start = start;
        _end = end;
    }

    public int GetIntervalDays(DateTime current)
    {
        var startDate = new DateTime(current.Year, current.Month, 1);
        var endDate = new DateTime(current.Year, current.Month,
            DateTime.DaysInMonth(current.Year, current.Month));
        var endD = _end < endDate ? _end : endDate;
        var startD = _start > startDate ? _start : startDate;
        return (endD - startD).Days + 1;
    }
}

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (end < start) return 0;

        var budgets = _budgetRepo.GetAll();
        decimal sum = 0;

        var current = new DateTime(start.Year, start.Month, 1);
        while (current <= end)
        {
            var monthlyBudget = budgets.FirstOrDefault(x => x.YearMonth == current.ToString("yyyyMM"));

            var days = new Period(start, end).GetIntervalDays(current);
            var amount = monthlyBudget?.GetAmount(days) ?? 0;
            sum += amount;

            current = current.AddMonths(1);
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

    public decimal GetAmount(int days)
    {
        var budgetDate = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        var budgetPerDay = Amount /
                           (decimal)DateTime.DaysInMonth(budgetDate.Year, budgetDate.Month);

        return budgetPerDay * days;
    }
}