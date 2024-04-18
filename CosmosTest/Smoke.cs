namespace Adamijak.CosmosTest;

[TestClass]
public class Smoke
{
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context){}
    
    [TestMethod]
    public void Pass(){}
    
    [ClassCleanup]
    public static async Task ClassCleanup(){}
}