using Polly;
using System;
using System.Collections.Generic;

namespace App.Polly
{
    public class Retry
    {
        private static int times = 0;

        public static void TestPolicy()
        {
            var policy = Policy
                .Handle<Exception>()
                .Retry(3, (exception, retryCount, context) => // 出异常会执行以下代码
                {
                    Console.WriteLine($"exception:{ exception.Message}, retryCount:{retryCount}, id:{context["id"]}, name:{context["name"]}");
                });

            try
            {
                // 通过 new Context 传递上下文信息
                var result = policy.Execute(Test, new Context("data", new Dictionary<string, object>() { { "id", "1" }, { "name", "beck" } }));
                Console.WriteLine($"result:{result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string Test()
        {
            // 每执行一次加1
            times++;

            // 前2次都抛异常
            if (times < 3)
            {
                throw new Exception("exception message");
            }
            return "success";
        }
    }
}
