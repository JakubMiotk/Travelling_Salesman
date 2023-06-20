using System;
using System.Collections.Generic;
using System.IO;

namespace MINSIProjekt
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            List<City> mainList = new List<City>();
            City.ReadJSONFile(AskForFile(), mainList);

            Result.GeneticAlgorithm(mainList, "Warszawa", 100, 10000, 0.1, 0.2);
        }

        private static string AskForFile()
        {
            BEGIN:
            Console.Write("Podaj nazwę pliku: ");
            var response = Console.ReadLine();
            if (File.Exists(response)) { return response; }
            else
            {
                Console.Write("File doesnt exist\n");
                goto BEGIN;  
            }
        }
    }

}
