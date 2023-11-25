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
                int days;
                if (start.ToString("yyyyMM") == current.ToString("yyyyMM"))
                {
                    currentDate = start;
                    var endDate = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    var startDate = currentDate.Day;
                    days = endDate - startDate + 1;
                }
                else if (end.ToString("yyyyMM") == current.ToString("yyyyMM"))
                {
                    currentDate = end;
                    var endDate = currentDate.Day;
                    var startDate = new DateTime(currentDate.Year, currentDate.Month, 1).Day;
                    days = endDate - startDate + 1;
                }
                else
                {
                    currentDate = current.AddMonths(1).AddDays(-1);
                    var endDate = currentDate.Day;
                    var startDate = new DateTime(currentDate.Year, currentDate.Month, 1).Day;
                    days = endDate - startDate + 1;
                }

                var budgetPerDay = GetBudgetPerDay(budgets, currentDate.ToString("yyyyMM"));
                sum += budgetPerDay * days;

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

    private static decimal GetBudgetPerDay(List<Budget> budgets, string yearMonth)
    {
        var budgetDate = DateTime.ParseExact(yearMonth, "yyyyMM", null);
        var daysInMonth = DateTime.DaysInMonth(budgetDate.Year, budgetDate.Month);
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