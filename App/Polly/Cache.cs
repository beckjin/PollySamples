using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.MemoryCache;
using System;

namespace App.Polly
{
    public class Cache
    {
        public static void TestPolicy()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
            var cachePolicy = Policy.Cache(memoryCacheProvider, TimeSpan.FromSeconds(5));

            var resultFrist = cachePolicy.Execute(() => Test(), new Context("testKey"));
            var resultSecond = cachePolicy.Execute(() => Test(), new Context("testKey"));
            Console.WriteLine(resultFrist + "---" + resultSecond);
        }

        private static string Test()
        {
            return "success";
        }
    }
}
