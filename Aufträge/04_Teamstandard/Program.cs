using System.Text;
using Teamstandard;

Console.WriteLine(new FingerprintService().Create(Encoding.UTF8.GetBytes("abc")));
