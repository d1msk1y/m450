using Rabatt;
Console.WriteLine("Analyse: dotnet build --no-incremental");
Console.WriteLine(new PriceCalculator().CalculateTotal(100m));
Console.WriteLine(CustomerGreeting.Format("Ada"));
InputGuard.Check(100m);
