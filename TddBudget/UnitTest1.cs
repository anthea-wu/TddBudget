using NSubstitute;

namespace TddBudget;

public class Tests
{
    private IBudgetRepo _budgetRepo;
    private BudgetService _budgetService;

    [SetUp]
    public void Setup()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }

    [Test]
    public void one_day_budget()
    {
        GivenBudgets(new List<Budget>
        {
            new() { YearMonth = "202311", Amount = 30 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 25), new DateTime(2023, 11, 25));
        Assert.AreEqual(1, actual);
    }

    [Test]
    public void multiple_days_budget_in_same_month()
    {
        GivenBudgets(new List<Budget>
        {
            new() { YearMonth = "202311", Amount = 30 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 25), new DateTime(2023, 11, 27));
        Assert.AreEqual(3, actual);
    }

    [Test]
    public void multiple_days_budget_cross_month()
    {
        GivenBudgets(new List<Budget>
        {
            new() { YearMonth = "202311", Amount = 30 },
            new() { YearMonth = "202312", Amount = 310 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2023, 12, 2));
        Assert.AreEqual(21, actual);
    }

    [Test]
    public void multiple_days_budget_cross_three_month()
    {
        GivenBudgets(new List<Budget>
        {
            new() { YearMonth = "202311", Amount = 30 },
            new() { YearMonth = "202312", Amount = 3100 },
            new() { YearMonth = "202401", Amount = 310 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 29), new DateTime(2024, 1, 1));
        Assert.AreEqual(3112, actual);
    }

    [Test]
    public void no_budget_should_get_0()
    {
        GivenBudgets(new List<Budget>());

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2024, 1, 2));
        Assert.AreEqual(0, actual);
    }


    [Test]
    public void invalid_period_should_get_0()
    {
        GivenBudgets(new List<Budget>
        {
            new() { YearMonth = "202311", Amount = 30 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2023, 11, 21));
        Assert.AreEqual(0, actual);
    }

    private void GivenBudgets(List<Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }
}