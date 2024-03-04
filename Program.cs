using System;
using System.Security.Cryptography;
using System.Text;

namespace PaymentSystems
{
    internal class Program
    {
        public const string PAY_URL = "pay.system1.ru/order?amount=12000RUB&hash=";
        public const string ORDER_URL = "order.system2.ru/pay?hash=";
        public const string SYSTEM_URL = "system3.com/pay?amount=12000&curency=RUB&hash=";
        
        private static string _secretKey = "Sd1%h3EsQXfJ5Jt";
        
        public static void Main(string[] args)
        {
            Order order = new Order(12, 35000);

            IPaymentSystem payUrl = new PaymentSystem(order.Id);
            Console.WriteLine(payUrl.GetPayingLink(PAY_URL));

            IPaymentSystem payUrl2 = new PaymentSystem(order.Id, order.Amount);
            Console.WriteLine(payUrl2.GetPayingLink(ORDER_URL));
            
            IPaymentSystem payUrl3 = new PaymentSystem(order.Id, order.Amount, _secretKey);
            Console.WriteLine(payUrl3.GetPayingLink(SYSTEM_URL));
        }
    }
    
    public class Order
    {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    public interface IPaymentSystem
    { 
        string GetPayingLink(string url);
    }

    public class PaymentSystem : IPaymentSystem
    {
        public readonly string Hash;
        
        public PaymentSystem(int id)
        {
            Hash = GetHashMD5(id.ToString());
        }

        public PaymentSystem(int id, int amount)
        {
            Hash = GetHashMD5(id.ToString()+ amount.ToString());
        }

        public PaymentSystem(int id, int amount, string secretKey)
        {
            Hash = GetHashSHA1(id.ToString()+ amount.ToString() + secretKey.ToString());
        }

        public string GetPayingLink(string url)
        {
            return url + Hash;
        }

        private string GetHashMD5(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        private string GetHashSHA1(string input)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}