using NSubstitute;

namespace TddBudget;

public class Tests
{
    private BudgetService _budgetService;
    private IBudgetRepo _budgetRepo;

    [SetUp]
    public void Setup()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }

    [Test]
    public void sameday()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget() { YearMonth = "202311", Amount = 30 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 25), new DateTime(2023, 11, 25));
        Assert.AreEqual(1, actual);
    }

    [Test]
    public void samemonth()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget() { YearMonth = "202311", Amount = 30 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 25), new DateTime(2023, 11, 27));
        Assert.AreEqual(3, actual);
    }

    [Test]
    public void cross_month()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget() { YearMonth = "202311", Amount = 30 },
            new Budget() { YearMonth = "202312", Amount = 310 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2023, 12, 2));
        Assert.AreEqual(21, actual);
    }

    [Test]
    public void cross_three_month()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget() { YearMonth = "202311", Amount = 30 },
            new Budget() { YearMonth = "202312", Amount = 3100 },
            new Budget() { YearMonth = "202401", Amount = 310 }
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2024, 1, 2));
        Assert.AreEqual(3121, actual);
    }

    [Test]
    public void no_budget()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2024, 1, 2));
        Assert.AreEqual(0, actual);
    }


    [Test]
    public void invalid_period()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget() { YearMonth = "202311", Amount = 30 },
        });

        var actual = _budgetService.Query(new DateTime(2023, 11, 30), new DateTime(2023, 11, 21));
        Assert.AreEqual(0, actual);
    }
}