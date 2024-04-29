namespace CosmosBenchmark;

public class Item(int size)
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();

    public string? Data { get; set; } = new('x', size);
}