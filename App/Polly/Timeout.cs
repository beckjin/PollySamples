using Polly;
using Polly.Timeout;
using System;
using System.Threading;

namespace App.Polly
{
    public class Timeout
    {
        public static void TestPolicy()
        {
            var policy = Policy.Handle<TimeoutRejectedException>()
                      .Retry(3)
                      .Wrap(Policy.Timeout(3, TimeoutStrategy.Pessimistic));

            try
            {
                var result = policy.Execute(Test);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string Test()
        {
            Thread.Sleep(2000);
            return "success";
        }
    }
}
