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

    public int GetOverlappingDays(Period budgetPeriod)
    {
        var endD = _end < budgetPeriod._end ? _end : budgetPeriod._end;
        var startD = _start > budgetPeriod._start ? _start : budgetPeriod._start;
        var days = (endD - startD).Days + 1;
        return days;
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

        decimal sum = 0;
        var current = start;
        var budgets = _budgetRepo.GetAll();

        while (current < new DateTime(end.Year, end.Month, 1).AddMonths(1))
        {
            var budget = budgets.FirstOrDefault(x => x.YearMonth == current.ToString("yyyyMM"));
            if (budget != null)
            {
                var period = new Period(start, end);
                var days = period.GetOverlappingDays(new Period(budget.FirstDay(), budget.LastDay()));
                var budgetPerDay = budget.GetBudgetPerDay();
                sum += budgetPerDay * days;
            }

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

    public decimal GetBudgetPerDay()
    {
        var budgetFirstDay = FirstDay();
        var daysInMonth = DateTime.DaysInMonth(budgetFirstDay.Year, budgetFirstDay.Month);
        return (decimal)Amount / daysInMonth;
    }

    public DateTime FirstDay()
    {
        return DateTime.ParseExact(YearMonth, "yyyyMM", null);
    }

    public DateTime LastDay()
    {
        return new DateTime(FirstDay().Year, FirstDay().Month,
            DateTime.DaysInMonth(FirstDay().Year, FirstDay().Month));
    }
}