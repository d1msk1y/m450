namespace Rabatt;

public static class InputGuard
{
    public static void Check(decimal subtotal)
    {
        if (subtotal < 0m)
        {
            throw new ArgumentOutOfRangeException("total");
        }
    }
}
