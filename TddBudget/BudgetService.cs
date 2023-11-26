namespace TddBudget;

public class Period
{
    private readonly DateTime _end;
    private readonly DateTime _start;

    public Period(DateTime start, DateTime end)
    {
        _start = start;
        _end = end;
    }

    public int GetOverlappingDays(Period another)
    {
        var minEnd = _end < another._end ? _end : another._end;
        var maxStart = _start > another._start ? _start : another._start;
        return (minEnd - maxStart).Days + 1;
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
        return _budgetRepo.GetAll().Sum(budget => budget.OverlappingAmount(new Period(start, end)));
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

    public Period Period()
    {
        return new Period(FirstDay(), LastDay());
    }

    public decimal OverlappingAmount(Period period)
    {
        return GetBudgetPerDay() * period.GetOverlappingDays(Period());
    }
}