using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExampleLibrary.BankScenario.Timeout
{
    public class Bank
    {
        public bool Transfer(BankAccount from, BankAccount to, double amount) {
            bool lockTakenFrom = false;
            bool lockTakenTo = false;
            bool retval = false;
            try {
                //let's try to take the lock on "from"
                Monitor.TryEnter(from, 10, ref lockTakenFrom);
                //we could be here for two reasons:
                //1-we have the lock on "from", or
                //2-timeout has passed and somebody else has the lock
                if (lockTakenFrom) {
                    //we have the lock on from
                    //now let's try to take the lock on "to"
                    Monitor.TryEnter(to, 10, ref lockTakenTo);
                    //we could be here for two reasons:
                    //1-we have the lock on "to", or
                    //2-timeout has passed and somebody else has the lock
                    if (lockTakenTo) {
                        //if we're here, we have the lock on "to" and "from"
                        //so we finally can transfer the money
                        amount = from.Withdraw(amount);
                        to.Deposit(amount);
                        retval = true;
                    }
                }
            } finally {
                //we can now release the lock on "from"
                if (lockTakenFrom)
                    Monitor.Exit(from);
                //we can now release the lock on "to"
                if(lockTakenTo)
                    Monitor.Exit(to);
            }
            
            return retval;
        }
    }
}
