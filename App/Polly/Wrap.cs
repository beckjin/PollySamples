using Polly;
using System;
using System.Threading;

namespace App.Polly
{
    public class Wrap
    {
        private static int times = 0;

        public static void TestPolicy()
        {
            var retryPolicy = Policy
                    .Handle<Exception>()
                    .Retry(6);

            var circuitBreakerPolicy = Policy
                     .Handle<Exception>()
                     .CircuitBreaker(
                         exceptionsAllowedBeforeBreaking: 4, // 连续4次异常
                         durationOfBreak: TimeSpan.FromSeconds(1), // 断开1秒
                         onBreak: (exception, breakDelay) => // 断路器被打开的时
                        {
                            Console.WriteLine($"durationOfBreak: {breakDelay.TotalMilliseconds } ms, exception: " + exception.Message);
                        },
                         onReset: () => Console.WriteLine("Closed the circuit again"), // 重新关闭断路器时
                         onHalfOpen: () => Console.WriteLine("Half-open: Next call is a trial") // 在自动断路时间到时，从断开的状态复原
                     );

            var policy = Policy.Wrap(retryPolicy, circuitBreakerPolicy);

            try
            {
                var result = policy.Execute(Test);
                Console.WriteLine($"result:{result}");
            }
            catch (Exception)
            { }

            Thread.Sleep(2000);
            var result1 = policy.Execute(Test);
            Console.WriteLine($"result1:{result1}");
        }

        private static string Test()
        {
            // 每执行一次加1
            times++;

            if (times != 5)
            {
                throw new Exception("exception message");
            }
            return "success";
        }
    }
}
