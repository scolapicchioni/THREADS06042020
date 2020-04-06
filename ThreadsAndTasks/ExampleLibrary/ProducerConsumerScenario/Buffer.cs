using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExampleLibrary.ProducerConsumerScenario
{
    public class Buffer<T> {
        //we have a limited amount of room (10 items max)
        private const int size = 10;

        //this is the actual data structure where we put our stuff
        private Queue<T> buffer = new Queue<T>(size);
        
        //we use this to guard our code
        private object _lock = new object();
        
        //the producer must set this to true to signal that it's done
        private bool sendCompleteInvoked = false;
        
        //the consumer can stop consuming when the producer is done and we're empty
        public bool IsCompleted => sendCompleteInvoked && buffer.Count == 0;

        /// <summary>
        /// Inserts an item in a Thread Safe way (if there's room, otherwise it waits)
        /// </summary>
        /// <param name="item">The item to push in the buffer</param>
        public void Push(T item) {
            //we enter only if no other producer 
            //or consumer is pushing or pulling
            lock (_lock) {
                //if we're here we are safe
                while (buffer.Count == size) {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Queue full. waiting");
                    //uh oh, it looks like we're full....
                    //Let's wait and give the chance 
                    //to somebody else to push or pull
                    Monitor.Wait(_lock);
                }
                //if we're here we have the lock and there's room,
                //so we write our data
                buffer.Enqueue(item);
                //now we can signal another thread to stop waiting
                Monitor.Pulse(_lock);
            }
            //lock released, others may continue
        }

        /// <summary>
        /// Pulls an item from the queue (if not empty, otherwise it waits)
        /// </summary>
        /// <returns>The next item in the buffer</returns>
        public T Pull() {
            T result = default;
            //we enter only if we're the only producer or consumer
            //trying to work 
            lock (_lock) {
                //if we're here we're alone
                while (buffer.Count == 0) {
                    Console.WriteLine($"\t{Thread.CurrentThread.ManagedThreadId} Queue empty. waiting");
                    //uh oh... it looks like our queue is empty...
                    //better wait and give the chance to someone else
                    //to produce or consume data
                    Monitor.Wait(_lock);
                }
                //if we're here we have the lock
                //and there's data in the queue
                result = buffer.Dequeue();
                //now we signal another thread so that
                //it stops waiting
                Monitor.Pulse(_lock);
            }
            //lock released, other threads may continue
            return result;
        }

        /// <summary>
        /// Invoked by the Producer to signal that the data is over
        /// </summary>
        public void SendComplete() {
            sendCompleteInvoked = true;
        }
    }
}
