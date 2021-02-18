using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace KangaCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();

            PrivateKey key2 = new PrivateKey();
            PublicKey wallet2 = key2.publicKey();

            
            Blockchain kangacoin = new Blockchain(6, 100);

            Console.WriteLine("Start the miner!");
            kangacoin.MinePendingTransactions(wallet1);
            Transaction tx1 = new Transaction(wallet1, wallet2, 10);
            tx1.SignTransaction(key1);
            kangacoin.addPendingTransaction(tx1);
            Console.WriteLine("Start the miner!");
            kangacoin.MinePendingTransactions(wallet2);
            Console.WriteLine("\nBalance of Wallet1 is: $" + kangacoin.GetBalanceOfWallet(wallet1).ToString());
            Console.WriteLine("\nBalance of Wallet2 is: $" + kangacoin.GetBalanceOfWallet(wallet2).ToString());


            // kangacoin.AddBlock(new Block(1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "amount: 50"));
            // kangacoin.AddBlock(new Block(2, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "amount: 200"));
            //
            //string blockJSON = JsonConvert.SerializeObject(kangacoin, Formatting.Indented);
            //Console.WriteLine(blockJSON);


            if (kangacoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is valid!");
            }
            else
            {
                Console.WriteLine("Blockchain is not valid!");
            }
        }
    }
}
