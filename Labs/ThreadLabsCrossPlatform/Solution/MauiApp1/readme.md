# MultiThreading Labs

Some of these labs use [MAUI](https://dotnet.microsoft.com/en-us/apps/maui).

To make sure that your machine can create and run MAUI applications, follow this [tutorial](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/intro) first.

If you are on a iOS environment, follow [this tutorial](https://learn.microsoft.com/en-us/dotnet/maui/ios/cli)


Once your machine is correctly setup, you can proceed to focus on the labs.

There are two folders with two Visual Studio solutions: `Starter` and `Solution`.  
- `Starter` is your starting point. Here you can find three incomplete projects that you will enrich with your own code.  
- `Solution` is the end point, with the complete application. You can use it if you get stuck and whenever you want to compare your code with the one provided for you. 

Open the `Starter` solution.
There are three projects: 
- a class library
- a console application
- a MAUI application

The class library contains a `SlowClass` class with four methods, simulating heavy CPU usage. You will use them during the labs.

The Console application 
## Lab 00 - Single Threaded

Let's first have the *Sigle Threaded experience*.  
- Create an instance of the `SlowClass`
- Invoke the `SlowMethod01` 3 times in a synchronous, single threaded way  

You should see the first message after 3 seconds, than a second message after another 3 seconds, then a third message after 3 seconds, having to wait for a total of 9 seconds.

## Lab 01 - Multi Threaded

Now let's try it in parallel. As you have see, the `SolowMethod01` method returns void and accepts no parameter, which means you can use create an instance of the `Thread` class passing the method as `ThreadStart` to the `Start` method of the `Thread` class 
- Create an instance of the `SlowClass`
- Invoke the `SlowMethod01` 3 times in a parallel, multithreaded way, using the `Thread` class.  

You should see the three messages after 3 seconds, having to wait for a total of 3 seconds instead of 9.

## Lab 02 - Passing an object to a Thread

This time you have to invoke `SlowMethod02`. This method accepts an object, which means you can use the `ParameterizedThreadStart` delegate overload and pass the argument to the `Start()` method of the `Thread` class  
- Create an instance of the `SlowClass`
- Invoke the SlowMethod02 3 times in a parallel, multithreaded way, passing a 4, a 3 and a 5 as argument value.

You should see the first message after 3 seconds, than a second message after another second, then a third message after another second, having to wait for a total of 5 seconds.

## Lab 03 - Passing an integer to a Thread

This time you have to invoke `SlowMethod03`. This method accepts an integer, which means you cannot invoke it directly. You can, though, invoke another method of yours which in turn invokes `SlowMethod03`. This new method of yours may be a *normal* method but it could also be a `lambda expression`. Your choice.   
- Create an instance of the `SlowClass`
- Invoke the `SlowMethod03` 3 times in a parallel, multithreaded way, passing a 4, a 3 and a 5 as argument value. Use a method of yours as argument of the constructor of the `Thread` class.  

You should see the `Done after 3 seconds`, than `Done after 4 seconds`, then `Done after 5 seconds`.

## Lab 04 - Race Conditions

`SlowMethod04` returns the square of its input. Your job is to invoke `SlowMethod04` 3 times just like you did in Lab03, but this time you also want to display the sum of the 3 results. So if you pass 4, 3 and 5, you want to see `The square of 3 is 9`, than `The square of 4 is 16`, then `The square of 5 is 25` and `The sum is 50`. Update a `sum` variable in a thread safe way using a `lock` block. Wait the threads to be finished before showing the `sum` by using the `Join` method.

## Lab 05 - Graphical User Interface

Let's move to the `MAUI App` by setting it as *startup project*.  

There is a `MainPage.xaml` and a `MainPage.xaml.cs`. 
- The `xaml` file contains the `View`, where we define the fields where the user can  
  - give her input (`Entry`)
  - and run the features (`Button`)
  - see the output (`Label`) . 
- The `cs` file is the `ViewModel`, where we handle the events (`Click`), read from the `Entry` and write on the `Label`

In this lab, you will focus on the first button `btnSingleThreaded`. 
- Handle its `Click` event by writing code of the `btnSingleThreaded_Click` method.
- Parse the `Text` value of the 3 `Entry` components `txtNumber01`,  `txtNumber02` and `txtNumber03`
- Create an instance of the `SlowClass` class
- Invoke `SlowMethod04` passing the parsed value of `txtNumber01.Text` and write the result on `lblResult01.Text`
- Invoke `SlowMethod04` passing the parsed value of `txtNumber02.Text` and write the result on `lblResult02.Text`
- Invoke `SlowMethod04` passing the parsed value of `txtNumber03.Text` and write the result on `lblResult03.Text`
- Write sum of the results on `lblSum.Text`

When you run the application and click on the first button, you will see that the User Interface hangs and become unresponsive. The UI is updated in one go after 12 seconds instead of after 3, 4 and 5 seconds.

## Lab 06 - Multithreading and SynchronizationContext

Now let's parallelize it in the `btnThreads_Click` handler of the `btnThreads` button.  
Use the strategy of `Lab04` to invoke `SlowMethod04` in parallel using the `Thread` class, but this time :
- Read the values from the `Entry` components
- Write the results to the `Label` components
- Use the `Interlocked` class instead of `lock` to update the `sum`
- Use the `SynchronizationContext` to `Post` the methods that update the UI

## Lab 07 - CountdownEvent

Handle the `btnCountdown_Click` event of the `btnCountdown` button.  
Repeat `Lab06`, but use a `CountdownEvent` to wait for the `sum` instead of `Thread.Join`.

## Lab 08 - ThreadPool.QueueUserWorkItem and Parallel.Invoke

Handle the `btnParallelInvoke_Click` event of the `btnParallelInvoke` button.  
Repeat `Lab06`, but 
- Use the `ThreadPool.QueueUserWorkItem` method to run a method that
  - Uses the `Parallel.Invoke` to invoke the `SlowMethod04` method instead of creating instances of the `Thread` class 
  - Waits the `Parallel.Invoke` and updates the `sum`

## Lab 09 - ThreadPool.QueueUserWorkItem and Parallel.For

Handle the `btnParallelFor_Click` event of the `btnParallelFor` button.  
Repeat `Lab06`, but 
- Use the `ThreadPool.QueueUserWorkItem` method to run a method that
  - Uses the `Parallel.For` to invoke the `SlowMethod04` method instead of creating instances of the `Thread` class 
  - Waits the `Parallel.For` and updates the `sum`

## Lab 10 - ThreadPool.QueueUserWorkItem and Parallel.ForEach

Handle the `btnParallelForEach_Click` event of the `btnParallelForEach` button.  
Repeat `Lab06`, but 
- Use the `ThreadPool.QueueUserWorkItem` method to run a method that
  - Uses the `Parallel.ForEach` to invoke the `SlowMethod04` method instead of creating instances of the `Thread` class 
  - Waits the `Parallel.ForEach` and updates the `sum`

## Lab 11 - New Tasks with Start and Task.WaitAll

Now let's parallelize it in the `btnNewTask_Clicked` handler of the `btnNewTask` button.  
Use the strategy of `Lab06` to invoke `SlowMethod04` in parallel, but this time 
- Create instances of the `Task` class 
- Start them using the `Start` method
- Use `Task.WaitAll` to update the `sum`

## Lab 12 - Task Run

Now let's parallelize it in the `btnTaskRun_Clicked` handler of the `btnTaskRun` button.  
Use the strategy of `Lab 11` to invoke `SlowMethod04` in parallel, but this time use `Task.Run` instead of creating instances of the `Task` class

## Lab 13 - Asynchronous programming with ContinueWith and WhenAll

Let's move to asynchronous programming in the `btnTasksContinueWith_Click` handler of the `btnTasksContinueWith` button.  
We still want to parallelize `SlowMethod04` using `Task.Run`, but we will move the update of the corresponding `Label` in a *callback* that we will pass to the `ContinueWith` of the `Task` class.  
The update of the `sum` will move to the `ContinueWith` of the `Task` returned by a `Task.WhenAll`.
In this lab you still need to `Post` the update to your `SynchronizationContext`.

## Lab 14 - ContinueWith FromCurrentSynchronizationContext

Let's get rid of our own `SynchronizationContext` in the `btnTasksFromSynchronizationContext_Click` handler of the `btnTasksFromSynchronizationContext` button.  

Repeat `Lab14` but use the `FromCurrentSynchronizationContext()` option of the `ContinueWith`

## Lab 15 - `async` and `await`

Instead of the `ContinueWith`, we are going to use the `async` and `await` constructions to update the UI as soon as the `SlowMethod04` is ready with the result.
- Create an `AwaiterMethod` returning `void` and accepting a `Task<int>` and a `Label` as input parameter.
- In the `AwaiterMethod`, use the `await` keyword to retrieve the result if the input `Task`, then update the input `Label` with it
- In the `btnTasksAwait_Click` handler of the `btnTasksAwait` button:
  - Use the `Task.Run` method to schedule the `SlowMethod04` in parallel as you did in the previous lab
  - Pass the resulting `Task` to the `AwaiterMethod` together with which label has to be updated with the result
  - Use the `await` keyword against the `Task` returned by the `Task.WhenAll` method
  - Use the result to update the `sum` 

## Lab 16 - Task FromResult

The goal of this lab is to improve the performances whenever a calculation that has already been calculated before is requested again.   
You're going to implement a `SlowLibraryCacheService` that will store the result of the `SlowMethod` in its own memory data structure, so that whenever the same calculation is invoked again, the result is immediately returned instead of being slowly calculated once more. 
The `btnTaskFromResult_Click` method will make use of this service instead of talking to the `SlowClass` directly.  
- In the `SlowLibrary` project, create a `SlowLibraryCacheService` class
- Add a private dictionary to store the input number as a *key* and the result as a *value*
- Create a `GetSlowSquare` method. Let the method accept an `int` and return a `Task<int>`
    - Check if the input is already in the dictionary. 
    - If so, return a completed Task with the value in the Result
    - If not, Run a Task to invoke the SlowMethod. Schedule the continuation so that the result can be stored in the dictionary. Return the Task.

Don't forget to use a Thread Safe Dictionary to avoid  concurrency problems.

In the `btnTaskFromResult_Click` handler, use the cache service instead of invoking the SlowMethod directly. If you click on the button multiple times using the same values, you should see that the UI is immediately updated instead of having to wait many seconds.

## Lab 17 - Task When Any

On Lab 16 you used one strategy to continue as soon as Tasks complete, which was to `await` for them in a separate method.  
In this lab, you will use a different strategy, which consists in 
- putting the `Task`s in a collection
- `await`ing on the fastest `Task`, returned by the `Task.WhenAny` 
- removing the fastest task from the collection
- repeating the `await`ing and removing until the collection is empty

Implement the `btnAwaitWhenAny_Click` handler of the `btnAwaitWhenAny` button.
You will need two different collections: one for the `WaitAll`, to update the `sum` as you did in the preovious labs, and the other for the `WaitAny`, to update the corresponding `Label`. To know which `Label` to update when the `Task` is complete, you can use the `Task` as a key in a `Dictionary` and link it to a `Label` as a value, so that it's easy to retrieve the `Label` once you have the fastest `Task`.

## Lab 18 - Interleaved

Yet another strategy to process `Task`s as the complete is to create a new *combinator*. As explained by Steven Toub in is article [Processing Tasks as they complete](https://devblogs.microsoft.com/pfxteam/processing-tasks-as-they-complete/):

> When working with a collection of `Task<T>` instances, `WhenAny` returns a `Task<Task<T>>`; this is a task that will complete when the first supplied task has completed, and that `Task<Task<T>>`’s `Result` will be that first completed task.  In our case, we don't just want the first task, but rather we want all of the tasks, ordered in how they'll complete.  We can represent this with a `Task<Task<T>>[]`.  Think of this as an array of buckets where we'll place the input tasks as they complete, one task per bucket. So, if you want to get the first task to complete, you can `await` the first bucket of this array, and if you want to get the sixth task to complete, you can `await` the sixth bucket of this array.

So this is how it works:  
Let's say someone passes you a collection of 5 running Tasks that will complete in this order: Task3, Task2, Task4, Task1, Task5.   
What you do is the following.  
You create 5 Tasks of yours, put them in a collection and return it to your caller. Those Tasks are not the original ones, they are Tasks that you have created using 5 TaskCompletionSources. The Tasks that you return are not completed yet, but your caller can start waiting for the first one in the collection.  
In the meantime, you schedule the `ContinueWith` of each of the original five Tasks you got as an input.   
When the first `ContinueWith` runs (so in our example it's the one of Task3), you set the result of first of the tasks you returned, which means that your caller, which was waiting for it, now gets a result and can wait on the second Task you returned.  
When the `ContinueWith` of Task2 runs, you set the result of the second Task you returned.  
On the ContinueWith of Task4, you set the result of your third Task, on the one of Task1 you set the result of your fourth, and on Task5 you set the result of your fifth. You do that by keeping an index that you increment on each `ContinueWith`.


- Create an `Interleaved` method which takes an `IEnumerable<Task<int>>` and returns an `IEnumerable<Task<int>>`.
- Create a collection of `TaskCompletionSource<int>` with as many `TaskCompletionSource<int>` as the count of the `Task`s in the input collection
- Create an `int index` variable
- Schedule the `ContinueWith` of each of the original tasks in the input collection
- The `ContinueWith` sets the result of the Task at position `index` and increment the `index`
- Return a collection with the `Task`s property of each of the `TaskCompletionSource` items in the collection you created

Now that your method is complete, you can implement the `btnInterleaved_Click` handler of the `btnInterleaved` button.  
Invoke the `SlowMethod04` as you did previously, pass the tasks to the `Interleaved` method and collect the results by using a `foreach` in which you `await` the returned Tasks.   
`await` the `WhenAll` to update the `sum`.

## Lab 19 - Interleaved with IAsyncEnumerable

You will now make use of the `IAsyncEnumerable` interface and the `await foreach` construct.  
Create  a new `InterleavedStream` method that accepts an `IEnumerable<Task<int>>` and returns an `IAsyncEnumerable<int>`.  
 Invoke the `Interleaved` method passing your input as an argument.  
 For each item in the result of the `Interleaved` method, 
 `await` the item and `yield return` the result.  

 Now implement the `btnInterleavedStream_Clicked` handler of the `btnInterleavedStream` button.  
 Invoke the `SlowMethod04` as you did previously, pass the tasks to the `InterleavedStream` method and collect the results by using an `await foreach`.   
`await` the `WhenAll` to update the `sum`.





