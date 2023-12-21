# quadratic-equation
The client asked for a simple implementation of finding the roots of a quadratic equation in C#. Paid in currency for each line of code. So I didn't hold back. 😏😜🤑

## Chapter 1
The application simulates conveyor-belt data processing, where each data object from the input file sequentially goes through a series of processes performed by various threads. The application consists of six threads, each responsible for a specific stage of data processing. The threads operate asynchronously and use accumulators in queue mode to store data objects awaiting processing.

As soon as data enters the queue of the first thread, it gets activated, creates objects from the read data, and places them in a global dictionary for access. Each object receives a unique identifier, which is used for transferring between threads. Then this identifier is passed to the queue of the next thread, which, after a delay, starts its part of the processing.

The threads perform different operations, including calculating the discriminant, checking its value, computing the roots of the equation, and deciding on the format for recording results. If at any stage there is no data to process, the corresponding thread enters a waiting mode. After processing is completed by each thread, the final thread writes the results to an output file and removes the processed data from the global dictionary.

Thus, the application ensures continuous and efficient data processing, where each thread specializes in a certain type of operations and works in coordination with other threads to ensure a smooth and sequential flow of data through the conveyor.


## Chapter 2

In our work, we faced a choice between using the interrupt method and signals like AutoResetEvent for synchronization and thread management in a multi-threaded C# application. After thorough analysis, we concluded that signals are a more preferable option for our purposes.

The interrupt method offers a direct way to interrupt a thread in a waiting, blocking, or sleeping state. However, its use can lead to a ThreadInterruptedException, making it less ideal for complex multi-threaded applications that require precise understanding and control of thread states. Additionally, it has limited flexibility as it only affects threads in certain states.

On the other hand, AutoResetEvent and similar signals provide a high level of abstraction and control, allowing one thread to notify another about the occurrence of specific events. This is particularly useful in scenarios where fine coordination between threads is needed. Such an approach offers better coordination and flexibility, especially when complex synchronization logic and interaction between different data processing stages are required.

In the context of our project, which requires reliable interaction between threads for processing and transferring data through various stages, using AutoResetEvent seems to be a more sensible choice. It not only simplifies thread state management but also reduces the risk of errors associated with thread interruption.

Regarding the question of adding Thread.Abort or Thread.ResetAbort at the end of the code, it’s worth noting that Thread.Abort is considered an outdated and potentially dangerous practice in modern C# programming. Instead, safer and more manageable methods like CancellationToken are recommended for stopping threads. This helps avoid issues related to the safety and stability of the application's execution.


## Chapter 3

In our work, we also considered the use of CancellationToken and Thread.Abort for managing threads in our multi-threaded C# application. After analyzing various approaches, we concluded that CancellationToken is a more preferable and safe solution.

Thread.Abort is a method used for the immediate interruption of a thread. However, its use can be dangerous as it leads to the generation of a ThreadAbortException, potentially interrupting the thread execution at an unpredictable point. This can cause resource leaks, unfinished I/O operations, and other stability issues. Due to these risks, Thread.Abort is considered obsolete and not recommended for use in modern applications.

On the other hand, CancellationToken represents a mechanism for cooperative cancellation of operations, making it safer and more flexible. CancellationToken allows threads to check for a cancellation request at convenient points and properly conclude their work, freeing up resources and completing necessary operations. This ensures a more predictable and controlled termination of threads.

In the context of our project, using CancellationToken provides effective thread management, allowing us to correctly terminate operations when necessary, without the risk of sudden exceptions or system instability. We can pass the cancellation token to each thread and regularly check its state within the threads, which gives us control and flexibility in managing multi-threaded processing.

Thus, opting for CancellationToken over Thread.Abort allows us to ensure more reliable and stable execution of our application, minimizing risks associated with forced thread interruption.

Additionally, in our analysis of using CancellationToken and Thread.Abort in a multi-threaded C# application, we also considered the possibility of using methods like Thread.Suspend() and Thread.Resume(). However, we concluded that these methods also are not suitable for our project for several reasons:

Unpredictability and Risk of Deadlocks: Thread.Suspend() suspends a thread at an arbitrary execution point, which can lead to deadlocks if the thread was stopped in a critical segment of code, for example, while holding a lock. This makes the use of Suspend() and Resume() risky and unpredictable in multi-threaded applications.

Obsolescence of Methods: Similar to Thread.Abort, methods like Thread.Suspend() and Thread.Resume() are considered outdated in modern .NET due to their unreliability and security issues. The use of these methods is not recommended, and they may be completely removed from future versions of the .NET platform.

Lack of Control: These methods do not provide threads the opportunity to properly handle their state before suspension or after resumption. Unlike CancellationToken, which allows threads to cooperatively and safely respond to cancellation requests, Suspend and Resume interrupt thread execution without offering an opportunity for proper handling of the current state.

Complexities in Debugging and Testing: Using Thread.Suspend() and Thread.Resume() can significantly complicate the process of debugging and testing an application, as threads can be suspended at arbitrary points, making it difficult to determine the state of the program and the sources of errors.

Based on these considerations, we decided not to use Thread.Suspend() and Thread.Resume() in our application, and instead rely on safer and more controlled thread management mechanisms like CancellationToken, which provide a more stable and predictable multi-threaded processing.