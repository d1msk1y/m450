using System.Security.Cryptography;

namespace Teamstandard;

public class FingerprintService{
 public string Create(byte[] content){
 using var algorithm=MD5.Create();
 return ToHex(algorithm.ComputeHash(content));
 }
 static string ToHex(byte[] hash){return Convert.ToHexString(hash);}
}
