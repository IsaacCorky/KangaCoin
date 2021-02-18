using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace KangaCoin
{
    // make blackchain class
    class Blockchain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> pendingTransactions { get; set; }
        public decimal MiningReward { get; set; }


        public Blockchain(int difficulty, decimal miningreward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenisisBlock());
            this.Difficulty = difficulty;
            this.MiningReward = miningreward;
            this.pendingTransactions = new List<Transaction>();
        }

        public Block CreateGenisisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHmmssffff"), new List<Transaction>());
        }

        public Block GetLatestBlock()
        {
            return this.Chain.Last();
        }

        public void AddBlock(Block newBlock)
        {
            // add block and link to last block
            newBlock.PreviousHash = this.GetLatestBlock().Hash; // last block hash becomes the current block's "PreviousHash"
            newBlock.Hash = newBlock.CalculateHash(); // Calculate this block's Hash
            this.Chain.Add(newBlock); // add to chain
        }

        public void addPendingTransaction(Transaction transaction)
        {
            if (transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transaction must include a to and from address.");
            }
            if (transaction.Amount > this.GetBalanceOfWallet(transaction.FromAddress))
            {
                throw new Exception("There must be sufficient funds in the walet!");
            }
            if (transaction.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block.");
            }

            this.pendingTransactions.Add(transaction);
        }
        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");
            

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (!(transaction.FromAddress is null))
                    {
                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");

                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }

                    string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");
                    if (toDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }

        public void MinePendingTransactions(PublicKey miningRewardWallet)
        {
            Transaction rewardtx = new Transaction(null, miningRewardWallet, MiningReward);
            this.pendingTransactions.Add(rewardtx);

            Block newBlock = new Block(GetLatestBlock().Index + 1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), this.pendingTransactions, GetLatestBlock().Hash);
            newBlock.Mine(this.Difficulty);

            Console.WriteLine("Block successfully added!");
            this.Chain.Add(newBlock);
            this.pendingTransactions = new List<Transaction>();
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block currentBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                // hash match against it's self
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                // hash match against previous block
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }

            return true;
        }

        

    }
}
