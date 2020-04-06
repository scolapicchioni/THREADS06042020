using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExampleLibrary.BankScenario
{
    public class BankAccount
    {
        public int Id { get; set; }
        public double Saldo { get; private set; }
        public double Withdraw(double amount) {
            if (amount < 0) {
                amount = 0;
            }
            //this line of code 
            //Saldo -= amount;

            //is actually split into 3 operations
            //at runtime: 
            //double temp = Saldo;
            //temp = temp - amount;
            //Saldo = temp;

            //if we slow them down we can increase
            //the chances of a concurrency problem
            //in order to demonstrate
            //how it could go wrong
            double temp = Saldo;
            Thread.Sleep(1);
            temp = temp - amount;
            Thread.Sleep(1);
            Saldo = temp;
            
            return amount;
        }

        public double Deposit(double amount) {
            if (amount < 0) {
                amount = 0;
            }
            //Saldo += amount;

            double temp = Saldo;
            Thread.Sleep(1);
            temp = temp + amount;
            Thread.Sleep(1);
            Saldo = temp;


            return amount;
        }
    }
}
