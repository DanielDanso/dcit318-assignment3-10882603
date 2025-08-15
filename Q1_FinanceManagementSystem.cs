
using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // ================================
    // a. Define core models using records
    // ================================
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // ================================
    // b. Interface for processing transactions
    // ================================
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // ================================
    // c. Concrete processors
    // ================================
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
        }
    }

    // ================================
    // d. Base Account class
    // ================================
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }

    // ================================
    // e. Sealed SavingsAccount class
    // ================================
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
            }
        }
    }

    // ================================
    // f. FinanceApp
    // ================================
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Create SavingsAccount
            var account = new SavingsAccount("SA-001", 1000m);

            // ii. Create sample transactions
            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 300m, "Entertainment");

            // iii. Process each transaction
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            mobileMoney.Process(t1);

            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            bankTransfer.Process(t2);

            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();
            cryptoWallet.Process(t3);

            // iv. Apply each transaction to the account
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            // v. Add all transactions to _transactions
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("\nAll transactions recorded successfully.");
        }
    }

    // ================================
    // Main Program
    // ================================
    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
