using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExampleLibrary.BankScenario.EventWaitHandles
{
    public class Bank
    {
        //Think about an AutoResetEvent like a "door" guarding a 
        //piece of code.
        //The door of an AutoResetEvent automatically closes as soon
        //as one thread goes through it.
        //In our case, the door is open at the beginning.
        private AutoResetEvent transferredEvent = new AutoResetEvent(true);
        public void Transfer(BankAccount from, BankAccount to, double amount) {
            //The first Transfer can proceed 
            //because the door is open,
            //but it closes the
            //door behind itself.
            //This means any other Transfer will have to wait
            //for the door to open in order to proceed.
            transferredEvent.WaitOne();

            amount = from.Withdraw(amount);
            to.Deposit(amount);

            //When the Transfer is done, it opens the door with
            //the Set so that another waiting Transfer can continue
            transferredEvent.Set();
        }
    }
}
