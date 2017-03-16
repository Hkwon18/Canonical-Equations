using System;
using System.IO;
using Canonical_Equations.Classes;

namespace Canonical_Equations
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Title = "Canonical Equations Interactive Mode";

                while (true)
                {
                    Console.WriteLine("Enter An equation of the form: " +
                                      "P1 + P2 + ... = ... + PN");
                    Console.WriteLine("Press Ctrl + C to close the program");
                    string equation = Console.ReadLine();

                    if (String.IsNullOrEmpty(equation))
                    {
                        continue;
                    }

                    Canonizer canonizer = new Canonizer();

                    try
                    {
                        string result = canonizer.ParseEquation(equation);
                        Console.WriteLine(result);
                        Console.WriteLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Entry was invalid. try again");
                        Console.WriteLine();  
                    }
                }
            }
            else
            {
                string fileName = args[0];


                StreamReader input = new StreamReader(fileName);
                StreamWriter output = new StreamWriter(fileName + ".out");

                Console.WriteLine("Reading input file");

                string result;
                string line;
                while ((line = input.ReadLine()) != null)
                {
                    Canonizer canonizer = new Canonizer();
                    result = canonizer.ParseEquation(line);

                    output.WriteLine(result);
                }

                input.Close();
                output.Flush();
                output.Close();;

                Console.WriteLine("Finished");
            }
        }
    }
}
