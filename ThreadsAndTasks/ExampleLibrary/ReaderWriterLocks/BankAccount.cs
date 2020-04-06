using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExampleLibrary.BankScenario.ReaderWriterLocks
{
    public class BankAccount {
        ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
        private double _saldo;

        public int Id { get; set; }
        public double Saldo {
            get {
                try {
                    _rw.EnterReadLock();
                    return _saldo;
                } finally {
                    _rw.ExitReadLock();
                }
            }
            private set {
                try {
                    _rw.EnterWriteLock();
                    _saldo = value;
                } finally {
                    _rw.ExitWriteLock();
                }
            }
        }
        public double Withdraw(double amount) {
            if (amount < 0) {
                amount = 0;
            } else {
                try {
                    _rw.EnterWriteLock();
                    _saldo -= amount;
                } finally {
                    _rw.ExitWriteLock();
                }
            }
            return amount;
        }

        public double Deposit(double amount) {
            if (amount < 0) {
                amount = 0;
            } else {
                try {
                    _rw.EnterWriteLock();
                    _saldo += amount;
                } finally {
                    _rw.ExitWriteLock();
                }
            }
            return amount;
        }
    }
}
