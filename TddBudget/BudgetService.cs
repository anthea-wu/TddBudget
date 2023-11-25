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
                    var yearMonth = currentDate.ToString("yyyyMM");
                    var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                                      new Budget { Amount = 0 };
                    var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    var days = daysInMonth - currentDate.Day + 1;

                    sum += monthBudget.Amount / (decimal)daysInMonth * days;
                }
                else if (end.ToString("yyyyMM") == current.ToString("yyyyMM"))
                {
                    currentDate = end;
                    var yearMonth = currentDate.ToString("yyyyMM");
                    var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                                      new Budget { Amount = 0 };
                    var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    var days = currentDate.Day;

                    sum += monthBudget.Amount / (decimal)daysInMonth * days;
                }
                else
                {
                    currentDate = current.AddMonths(1).AddDays(-1);
                    var yearMonth = currentDate.ToString("yyyyMM");
                    var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                                      new Budget { Amount = 0 };
                    var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    var days = currentDate.Day;

                    sum += monthBudget.Amount / (decimal)daysInMonth * days;
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