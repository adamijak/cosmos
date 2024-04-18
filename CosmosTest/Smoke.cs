namespace Adamijak.CosmosTest;

[TestClass]
public class Smoke
{
    [TestMethod]
    public void Pass(){}

    [TestMethod]
    public void Throw()
    {
        Assert.ThrowsException<Exception>(() => throw new Exception());
    }
}