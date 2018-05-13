using App.Polly;
using System;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            CircuitBreaker.TestPolicy();
    
            Console.ReadLine();
        }
    }
}
