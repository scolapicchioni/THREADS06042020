using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleLibrary.BankScenario.Deadlocked
{
    public class Bank
    {
        public void Transfer(BankAccount from, BankAccount to, double amount) {
            lock (from) {
                lock (to) {
                    amount = from.Withdraw(amount);
                    to.Deposit(amount);
                }
            }
        }
    }
}
