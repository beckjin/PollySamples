using Polly;
using System;
using System.Threading;

namespace App.Polly
{
    public class CircuitBreaker
    {
        private static int times = 0;
        public static void TestPolicy()
        {
            var policy = Policy
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

            for (int i = 0; i < 12; i++)
            {
                try
                {
                    var result = policy.Execute(Test);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(500);
            }
        }

        private static string Test()
        {
            times++;

            if (times % 5 != 0)
            {
                throw new Exception("exception message");
            }
            return "success";
        }
    }
}
