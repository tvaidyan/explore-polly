using Polly;

Console.WriteLine("Which option do you want to try? Press");
Console.WriteLine("1. Retry 3 times");
Console.WriteLine("2. Retry 3 times, with a timeout of 2 seconds in between");
Console.WriteLine("3. Retry forever, with timeout");
Console.WriteLine("4. Exponential Back-off");
Console.WriteLine("5. Circuit Breaker");

var answer = Console.ReadLine();

switch (answer)
{
    case "1":
        // Retry three times
        var mySimpleRetryPolicy = Policy.Handle<ApplicationException>().Retry(3);
        mySimpleRetryPolicy.Execute(() =>
        {
            MethodWithException();
        });
        break;
    case "2":
        // Retry 3 times with a timeout of 2 seconds in between
        var retryThriceWithTimeout = Policy.Handle<ApplicationException>().WaitAndRetry(3, x => TimeSpan.FromSeconds(2));
        retryThriceWithTimeout.Execute(() =>
         {
             MethodWithException();
         });
        break;
    case "3":
        // Retry forever with a timeout
        var retryForeverWithTimeout = Policy.Handle<ApplicationException>()
    .WaitAndRetryForever(x => TimeSpan.FromSeconds(2));

        retryForeverWithTimeout.Execute(() =>
        {
            MethodWithException();
        });
        break;
    case "4":
        // Exponential back-off
        var exponentialBackoff = Policy.Handle<ApplicationException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        exponentialBackoff.Execute(() =>
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            MethodWithException();
        });
        break;
    case "5":
        // Circuit Breaker
        var circuitBreaker = Policy
        .Handle<ApplicationException>()
        .CircuitBreaker(3, TimeSpan.FromSeconds(5));
        var i = 1;
        while (i < 51)
        {
            Console.WriteLine($"Attempt {i}");
            i++;

            try
            {
                circuitBreaker.Execute(() =>
                {
                    Console.WriteLine($"Calling MethodWithException at {DateTime.Now.ToLongTimeString()}");
                    MethodWithException();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        break;
    default:
        Console.WriteLine("Sorry, invalid selection.");
        break;
}

static string MethodWithException()
{
    Console.WriteLine("MethodWithException running.");
    throw new ApplicationException("Uh-oh");
}