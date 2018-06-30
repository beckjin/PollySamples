using Polly;
using Polly.Wrap;
using System;
using System.Threading;

namespace App.Polly
{
    public class CircuitBreaker
    {
        private static int times = 0;
        public static void TestPolicy()
        {
            var fallbackPolicy = Policy<string>.Handle<Exception>().Fallback("失败了");

            var circuitBreakerPolicy = Policy
                    .Handle<Exception>()
                    .CircuitBreaker(
                        exceptionsAllowedBeforeBreaking: 4,             // 连续4次异常
                        durationOfBreak: TimeSpan.FromMinutes(1),       // 断开1分钟
                        onBreak: (exception, breakDelay) =>             // 断路器打开时
                            Console.WriteLine($"熔断: {breakDelay.TotalMilliseconds } ms, 异常: " + exception.Message),
                        onReset: () =>                                  // 熔断器关闭时
                            Console.WriteLine("熔断器关闭了"),
                        onHalfOpen: () =>                               // 熔断时间结束时，从断开状态进入半开状态
                            Console.WriteLine("熔断时间到，进入半开状态")
                    );


            var advancedCircuitBreakerPolicy = Policy
                    .Handle<Exception>()
                    .AdvancedCircuitBreaker(
                        failureThreshold: 0.5,                          // 至少50%有异常则熔断
                        samplingDuration: TimeSpan.FromSeconds(10),     // 10秒内
                        minimumThroughput: 8,                           // 最少共有多少次调用
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (exception, breakDelay) =>             // 断路器打开时
                            Console.WriteLine($"熔断: {breakDelay.TotalMilliseconds } ms, 异常: " + exception.Message),  
                        onReset: () =>                                  // 熔断器关闭时
                            Console.WriteLine("熔断器关闭了"),                                                            
                        onHalfOpen: () =>                               // 熔断时间结束时，从断开状态进入半开状态
                            Console.WriteLine("熔断时间到，进入半开状态")                                                   
                    );

            var warp = fallbackPolicy.Wrap(circuitBreakerPolicy);

            for (int i = 0; i < 12; i++)  // 模拟多次调用，触发熔断
            {
                try
                {
                    var result = circuitBreakerPolicy.Execute(Test);
                    //var result = warp.Execute(Test);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("try-catch:" + ex.Message);
                }
                Thread.Sleep(500);
            }
        }

        private static string Test()
        {
            times++;

            if (times % 5 != 0) // 模仿某些错误情况下抛异常
            {
                throw new Exception("exception message");
            }
            return "success";
        }
    }
}
