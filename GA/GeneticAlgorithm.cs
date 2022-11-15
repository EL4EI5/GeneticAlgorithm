using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;

namespace GA
{
    class RandomGenerator
    {
        static Random RND = new Random(DateTime.Now.Millisecond);

        public static int RandomNumber()
        {
            return RND.Next(32);
        }

        public static byte RandomBit()
        {
            return Convert.ToByte(Math.Abs(RND.Next() % 2));
        }
    }

    public class Demo
    {
        Individual Highest1, Highest2;
        Population Populations = new Population();

        public byte Score = 0;

        public void Selection()
        {
            Highest1 = Populations.Individuals[Populations.First].Copy();
            Highest2 = Populations.Individuals[Populations.Second].Copy();
        }

        public void Crossover()
        {
            int CrossOverPoint = RandomGenerator.RandomNumber();
            for (int i = 0; i < CrossOverPoint; i++)
            {
                byte Temp = Highest1.Genes[i];
                Highest1.Genes[i] = Highest2.Genes[i];
                Highest2.Genes[i] = Temp;
            }
        }

        public void Mutation()
        {
            int MutationPoint = RandomGenerator.RandomNumber();
            if (Highest1.Genes[MutationPoint] == 0) Highest1.Genes[MutationPoint] = 1;
            else Highest1.Genes[MutationPoint] = 0;

            MutationPoint = RandomGenerator.RandomNumber();
            if (Highest2.Genes[MutationPoint] == 0) Highest2.Genes[MutationPoint] = 1;
            else Highest2.Genes[MutationPoint] = 0;
        }

        public void FittestOffspring()
        {
            Highest1.CalculateScore();
            Highest2.CalculateScore();
            if (Highest1.Score > Highest2.Score)
            {
                Populations.Individuals[Populations.Last] = Highest1;
                Score = Highest1.Score;
            }
            else
            {
                Populations.Individuals[Populations.Last] = Highest2;
                Score = Highest2.Score;
            }
            Populations.HighestScore();
        }
    }


    class Individual
    {
        static int Zeros = 9;
        static bool Initiated = false;
        static SHA256 Sha256 = SHA256.Create();
        static Random RND = new Random();
        public static byte[] Data = new byte[80];

        public byte Score = 0;
        public byte[] Genes = new byte[32];

        public Individual()
        {
            if (!Initiated)
            {
                HeaderData();
                Initiated = true;
            }
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = RandomGenerator.RandomBit();
            }
            CalculateScore(); 
        }

        private Individual(Individual Old)
        {
            Score = Old.Score;
            Old.Genes.CopyTo(Genes, 0);
        }

        public static string GetHash()
        {
            var Hash = Sha256.ComputeHash(Sha256.ComputeHash(Data));
            StringBuilder hex = new StringBuilder(Hash.Length * 2);
            foreach (byte b in Hash)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public Individual Copy()
        {
            return new Individual(this);
        }

        public void CalculateScore()
        {
            Score = 0;
            double Number = 0;
            for (int i = 0; i < Genes.Length; i++)
            {
                if (Genes[i] == 1) Number += Math.Pow(2, i);
            }
            var Bytes = BitConverter.GetBytes(Convert.ToUInt32(Number));
            Data[76] = Bytes[0];
            Data[77] = Bytes[1];
            Data[78] = Bytes[2];
            Data[79] = Bytes[3];
            var Hash = Sha256.ComputeHash(Sha256.ComputeHash(Data));
            int Limit = Hash.Length - Zeros;
            for (int i = Hash.Length - 1; i > Limit; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    var Bit = (Hash[i] >> j) % 2;
                    if (Bit == 0) Score++;
                }
            }
        }

        public static void HeaderData()
        {
            string Header = "000000205873757623f61cf57d122d3c18a877c8628f3193d2f9060000000000000000005c6b6c678a85005e91647f022798a27fd4bf5e07a877115ae7691373de4f9e912b80475fea0710179052dc97";
            for (int i = 0; i < Header.Length; i += 2)
            {
                string Number = "0x" + Header.Substring(i, 2);
                Data[i / 2] = Convert.ToByte(Number, 16);
            }
        }

        public override string ToString()
        {
            var SB = new StringBuilder();
            for (int i = 0; i < Genes.Length; i++)
            {
                SB.Append(Genes[i]);
            }
            return SB.ToString();
        }
    }

    class Population
    {
        public Individual[] Individuals = new Individual[1000];
        public int First = 0;
        public int Second = 0;
        public int Last = 0;

        public Population()
        {
            for (int i = 0; i < Individuals.Length; i++)
            {
                Individuals[i] = new Individual();
            }
            HighestScore();
        }

        public void HighestScore()
        {
            int MaxFit = -1;
            int MinFit = -1;
            for (int i = 0; i < Individuals.Length; i++)
            {
                if (Individuals[i].Score >= MaxFit)
                {
                    MaxFit = Individuals[i].Score;
                    Second = First;
                    First = i;
                }
                if (Individuals[i].Score <= MinFit)
                {
                    MinFit = Individuals[i].Score;
                    Last = i;
                }
            }
        }
    }
}
