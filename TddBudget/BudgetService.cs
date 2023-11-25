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

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var current = new DateTime(start.Year, start.Month, 1);
            while (current <= end)
            {
                DateTime currentDate;
                if (start.ToString("yyyyMM") == current.ToString("yyyyMM"))
                {
                    currentDate = start;
                    var days = DateTime.DaysInMonth(currentDate.Year, currentDate.Month) - currentDate.Day + 1;

                    var budgetPerDay = GetBudgetPerDay(currentDate, budgets);
                    sum += budgetPerDay * days;
                }
                else if (end.ToString("yyyyMM") == current.ToString("yyyyMM"))
                {
                    currentDate = end;
                    var days = currentDate.Day;

                    var budgetPerDay = GetBudgetPerDay(currentDate, budgets);
                    sum += budgetPerDay * days;
                }
                else
                {
                    currentDate = current.AddMonths(1).AddDays(-1);
                    var days = currentDate.Day;

                    var budgetPerDay = GetBudgetPerDay(currentDate, budgets);
                    sum += budgetPerDay * days;
                }

                current = current.AddMonths(1);
            }
        }
        else
        {
            var startMonth = start.ToString("yyyyMM");
            var startBudget = budgets
                .FirstOrDefault(x => x.YearMonth == startMonth) ?? new Budget { Amount = 0 };

            var totalDays = (end - start).Days + 1;
            var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);

            var startBudgetAmount = startBudget.Amount / (decimal)daysInMonth * totalDays;
            sum += startBudgetAmount;
        }

        return sum;
    }

    private static decimal GetBudgetPerDay(DateTime currentDate, List<Budget> budgets)
    {
        var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        var yearMonth = currentDate.ToString("yyyyMM");
        var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                          new Budget { Amount = 0 };
        return monthBudget.Amount / (decimal)daysInMonth;
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