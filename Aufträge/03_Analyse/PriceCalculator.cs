namespace Rabatt;

public class PriceCalculator
{
    public decimal CalculateTotal(decimal subtotal)
    {
        if (subtotal > 100m)
        {
            return subtotal * 0.10m;
        }

        return subtotal;
    }
}
