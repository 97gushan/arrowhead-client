using System;
using ArrowHead;

namespace arrowhead_client
{
    class Program
    {
        static void Main(string[] args)
        {
            ArrowHead.Test t = new ArrowHead.Test();
            Console.WriteLine(t.GetHelloWorld());
        }
    }
}
