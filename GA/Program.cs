using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Demo demo = new Demo();
            int Count = 0;
            while (demo.Score < 50)
            {
                demo.Selection();
                demo.Crossover();
                //Do mutation under a random probability
                if (RandomGenerator.RandomNumber() % 7 < 5) demo.Mutation();
                demo.FittestOffspring();
                Count++;
            }

            Console.WriteLine("\nSolution found in generation " + Count);
            Console.WriteLine("Hash: " + Individual.GetHash());
            //Console.WriteLine("Genes: ");
            Console.WriteLine(""); 
            Console.ReadLine();
        }


    }
}
