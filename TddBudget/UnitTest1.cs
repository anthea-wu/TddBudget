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
            new Budget() { YearMonth = "202311", Amount = 31 }
        });

        var actual = _budgetService.Query(new DateTime(2023,11,25), new DateTime(2023,11,25));
        Assert.AreEqual(1,actual);
            
    }
    
    [Test]
    public void samemonth()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget() { YearMonth = "202311", Amount = 31 }
        });

        var actual = _budgetService.Query(new DateTime(2023,11,25), new DateTime(2023,11,27));
        Assert.AreEqual(3,actual);
            
    }
}