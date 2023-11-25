namespace TddBudget;

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
            var period = new Period
            {
                StartDate = start,
                EndDate = end
            };
            var currentPeriod = new Period
            {
                StartDate = new DateTime(current.Year, current.Month, 1),
                EndDate = new DateTime(current.Year, current.Month,
                    DateTime.DaysInMonth(current.Year, current.Month))
            };

            var perDay = GetAmount(period, currentPeriod, budgets);
            sum += perDay;

            current = current.AddMonths(1);
        }

        return sum;
    }

    private static decimal GetAmount(Period period, Period currentPeriod, List<Budget> budgets)
    {
        var days = period.GetIntervalDays(currentPeriod);

        var yearMonth = currentPeriod.StartDate.ToString("yyyyMM");
        var budgetDate = DateTime.ParseExact(yearMonth, "yyyyMM", null);
        var daysInMonth = DateTime.DaysInMonth(budgetDate.Year, budgetDate.Month);
        var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                          new Budget { Amount = 0 };
        var budgetPerDay = monthBudget.Amount / (decimal)daysInMonth;
        var perDay = budgetPerDay * days;
        return perDay;
    }
}

public class Period
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int GetIntervalDays(Period currentPeriod)
    {
        var startDay = StartDate > currentPeriod.StartDate ? StartDate : currentPeriod.StartDate;
        var endDay = EndDate < currentPeriod.EndDate ? EndDate : currentPeriod.EndDate;
        return endDay.Day - startDay.Day + 1;
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