using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace KangaCoin
{
    class Transaction
    {
        public PublicKey FromAddress { get; set; }
        public PublicKey ToAddress { get; set; }
        public decimal Amount { get; set; }
        public Signature Signature { get; set; }
        public Transaction(PublicKey fromaddress, PublicKey toaddress, decimal amount)
        {
            this.FromAddress = fromaddress;
            this.ToAddress = toaddress;
            this.Amount = amount;
        }

        public void SignTransaction(PrivateKey signingkey)
        {
            string fromAddressDER = BitConverter.ToString(FromAddress.toDer()).Replace("-", "");
            string signingDER = BitConverter.ToString(signingkey.publicKey().toDer()).Replace("-", "");
            
            if (fromAddressDER != signingDER)
            {
                throw new Exception("You cannot sign transactions for other wallets!");
            }

            string txHash = this.CalculateHash();
            this.Signature = Ecdsa.sign(txHash, signingkey);
        }

        public string CalculateHash()
        {
            string fromAddressDER = BitConverter.ToString(FromAddress.toDer()).Replace("-", "");
            string toAddressDER = BitConverter.ToString(ToAddress.toDer()).Replace("-", "");
            string transactionData = fromAddressDER + toAddressDER + Amount;
            byte[] tdBytes = Encoding.ASCII.GetBytes(transactionData);
            return BitConverter.ToString(SHA256.Create().ComputeHash(tdBytes)).Replace("-", "");
        }

        public bool IsValid()
        {

            if (this.FromAddress is null) return true;
            if(this.Signature is null)
            {
                throw new Exception("No signature in this transaction!");
            }
            return Ecdsa.verify(this.CalculateHash(), this.Signature, this.FromAddress);
        }
    }
}
