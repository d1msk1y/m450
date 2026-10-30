namespace Rabatt;

public static class CustomerGreeting
{
    public static string Format(string? name)
    {
        return "Hallo " + name.Trim();
    }
}
