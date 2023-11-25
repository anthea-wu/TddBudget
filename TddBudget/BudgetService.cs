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
        if (end < start)
        {
            return 0;
        }

        var budgets = _budgetRepo.GetAll();
        decimal sum = 0;

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var loop = new DateTime(start.Year, start.Month, 1);
            while (end > loop)
            {
                var isStart = start.ToString("yyyyMM") == loop.ToString("yyyyMM");
                if (isStart)
                {
                    sum += BudgetAmount(start, budgets, isStart);
                }
                else if (end.ToString("yyyyMM") == loop.ToString("yyyyMM"))
                {
                    sum += BudgetAmount(end, budgets, isStart);
                }
                else
                {
                    sum += BudgetAmount(loop.AddMonths(1).AddDays(-1), budgets, isStart);
                }

                loop = loop.AddMonths(1);
            }
            // var startBudgetAmount = BudgetAmount(start, budgets, true);
            // sum += startBudgetAmount;
            //
            // var endBudgetAmount = BudgetAmount(end, budgets, false);
            // sum += endBudgetAmount;
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

    private static decimal BudgetAmount(DateTime day, List<Budget> budgets, bool isStart)
    {
        var yearMonth = day.ToString("yyyyMM");
        var monthBudget = budgets.FirstOrDefault(x => x.YearMonth == yearMonth) ?? new Budget() { Amount = 0 };
        var daysInMonth = DateTime.DaysInMonth(day.Year, day.Month);
        int days = 0;
        if (isStart)
        {
            days = daysInMonth - day.Day + 1;
        }
        else
        {
            days = day.Day;
        }

        return monthBudget.Amount / (decimal)daysInMonth * days;
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