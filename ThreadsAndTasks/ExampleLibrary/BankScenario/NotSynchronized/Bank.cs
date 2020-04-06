using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExampleLibrary.BankScenario.NotSynchronized
{
    public class Bank
    {
        public void Transfer(BankAccount from, BankAccount to, double amount) {
            amount = from.Withdraw(amount);
            to.Deposit(amount);
        }
    }
}
