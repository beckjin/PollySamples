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
                        exceptionsAllowedBeforeBreaking: 4,             // 连续4次异常
                        durationOfBreak: TimeSpan.FromSeconds(1),       // 断开1秒
                        onBreak: (exception, breakDelay) =>             // 断路器打开时
                            Console.WriteLine($"熔断: {breakDelay.TotalMilliseconds } ms, 异常: " + exception.Message),
                        onReset: () =>                                  // 熔断器关闭时
                            Console.WriteLine("熔断器关闭了"),
                        onHalfOpen: () =>                               // 熔断时间结束时，从断开状态进入半开状态
                            Console.WriteLine("熔断时间到，进入半开状态")
                    );

            var wrap = Policy.Wrap(retryPolicy, circuitBreakerPolicy);

            try
            {
                var result = wrap.Execute(Test);
                Console.WriteLine($"result:{result}");
            }
            catch (Exception)
            { }

            Thread.Sleep(2000);
            var result1 = wrap.Execute(Test);
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
