namespace Adamijak.CosmosTest;

[TestClass]
public class Smoke
{
    [TestMethod]
    public void Pass(){}

    [TestMethod]
    public void Throw()
    {
        throw new Exception("Smoke.Throw exception");
    }
}