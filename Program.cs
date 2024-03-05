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
            Console.WriteLine(PAY_URL + payUrl.GetPayingLink(order));

            IPaymentSystem payUrl2 = new PaymentSystem(order.Id, order.Amount);
            Console.WriteLine(ORDER_URL + payUrl2.GetPayingLink(order));
            
            IPaymentSystem payUrl3 = new PaymentSystem(order.Id, order.Amount, _secretKey);
            Console.WriteLine(SYSTEM_URL + payUrl3.GetPayingLink(order));
        }
    }
    

    public interface IPaymentSystem
    { 
        string GetPayingLink(Order url);
    }

    public interface IHashService
    {
        string GetHashMD5(string input);
        string GetHashSHA1(string input);
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

    public class HashService : IHashService
    {
        public string GetHashMD5(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        public string GetHashSHA1(string input)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
    
    public class PaymentSystem : IPaymentSystem
    {
        public readonly string Hash;
        
        public PaymentSystem(int id)
        {
            IHashService hash = new HashService();
            Hash = hash.GetHashMD5(id.ToString());
        }

        public PaymentSystem(int id, int amount)
        {
            IHashService hash = new HashService();
            Hash = hash.GetHashMD5(id + amount.ToString());
        }

        public PaymentSystem(int id, int amount, string secretKey)
        {
            IHashService hash = new HashService();
            Hash = hash.GetHashSHA1(id + amount.ToString() + secretKey);
        }

        public string GetPayingLink(Order order)
        {
            return Hash;
        }
    }
}