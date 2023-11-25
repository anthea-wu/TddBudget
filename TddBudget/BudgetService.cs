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
            var loop = new DateTime(start.Year, start.Month, 1);
            while (loop <= end)
            {
                var isStart = start.ToString("yyyyMM") == loop.ToString("yyyyMM");
                if (isStart)
                {
                    var yearMonth = start.ToString("yyyyMM");
                    var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                                      new Budget { Amount = 0 };
                    var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
                    var days = daysInMonth - start.Day + 1;

                    sum += monthBudget.Amount / (decimal)daysInMonth * days;
                }
                else if (end.ToString("yyyyMM") == loop.ToString("yyyyMM"))
                {
                    var yearMonth = end.ToString("yyyyMM");
                    var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                                      new Budget { Amount = 0 };
                    var daysInMonth = DateTime.DaysInMonth(end.Year, end.Month);
                    var days = end.Day;

                    sum += monthBudget.Amount / (decimal)daysInMonth * days;
                }
                else
                {
                    var day = loop.AddMonths(1).AddDays(-1);
                    var yearMonth = day.ToString("yyyyMM");
                    var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ??
                                      new Budget { Amount = 0 };
                    var daysInMonth = DateTime.DaysInMonth(day.Year, day.Month);
                    var days = day.Day;

                    sum += monthBudget.Amount / (decimal)daysInMonth * days;
                }

                loop = loop.AddMonths(1);
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