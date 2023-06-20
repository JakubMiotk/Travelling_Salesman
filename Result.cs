using System;
using System.Collections.Generic;
using System.Linq;

namespace MINSIProjekt
{
    public class Result
    {
        private static Random rand = new Random();
        List<int> sequence;
        int cycleLength;
        List<int> distances;
        List<int> distancesAsc;
        public Result(List<int> sequenceTemp, int cycleLengthTemp, List<int> distancesTemp, List<int> distancesAscTemp) 
        { 
            sequence = sequenceTemp;
            cycleLength = cycleLengthTemp;
            distances = distancesTemp;             
            distancesAsc = distancesAscTemp;     
        }
        public Result() 
        {
            sequence = new List<int>();
            cycleLength = 0;
            distances = new List<int>();
            distancesAsc = new List<int>();
        }

        public static void GeneticAlgorithm(List<City> mainlist, string startCity, int popsize, int iterations, double mutProb, double hybProb)
        {
            List<Result> results = new List<Result>();
            results = CreatePopulation(mainlist, startCity, popsize);
            Console.WriteLine("Startowa populacja: \n");
            results.ForEach(result => { Console.Write(result.ToString() + "\n"); });
            for (int i  = 0; i < iterations; i++)
            {
                List<Result> tempResults = results;
                Console.WriteLine("|||| Iteracja algorytmu: " + (i+1).ToString() + " ||||\n");
                for(int j = 0; j < popsize; j++)
                {
                    double prob = rand.NextDouble();
                    int z = rand.Next(0, popsize);
                    while (z == j) { z = rand.Next(0, popsize); }
                    if (prob <= hybProb) { tempResults.Add(Hybrydization(mainlist, results[j], results[z])); }
                }
                for (int j = 0; j < tempResults.Count-1; j++)
                {
                    double x = rand.NextDouble();
                    if(x <= mutProb) 
                    { 
                        Result mutRes = Mutation(mainlist, tempResults[j]);
                        tempResults[j] = mutRes;
                    }
                }
                List<Result> sortedList = Selection(tempResults, popsize);
                results = sortedList;
                results.ForEach(result => { Console.Write(result.ToString() + "\n"); });
            }
        }
        public static List<Result> CreatePopulation(List<City> list, string startCity, int popsize)
        {
            List<Result> results = new List<Result>();
            for(int i = 0; i < popsize; i++)
            {
                results.Add(CreateUnit(list, startCity));
            }
            return results;
        }

        public static Result CreateUnit(List<City> list, string startCity)
        {         
                int startsize = list.Count;
                string nextCity = startCity;
                int startCityId = list.FindIndex(match: x => x.name.Equals(startCity));
                List<City> listTemp = new List<City>(list);
                Result result = new Result();
                int cycleLength = 0;
                for (int z = 0; z < startsize; z++)
                {
                    for(int c = 0; c< listTemp.Count; c++)
                    {
                        if (listTemp[c].name.Equals(nextCity))
                        {
                            result.sequence.Add(int.Parse(listTemp[c].id));
                            listTemp.RemoveAt(c);
                        }
                    }
                    int i = listTemp.Count;
                    if (i == 0) { break; }
                    int x = rand.Next(0, i);
                result.distances.Add(int.Parse(listTemp[x].roads[nextCity]));
                    nextCity = listTemp[x].name;

                }
                result.sequence.Add(int.Parse(list[startCityId].id));
                result.distances.Add(int.Parse(list[startCityId].roads[nextCity]));
                for(int i = 0; i < result.distances.Count; i++)
                {
                    cycleLength += result.distances[i];
                    result.distancesAsc.Add(cycleLength);
                }
                result.cycleLength = cycleLength;
                return result;
        }

        public static Result Hybrydization(List<City> list, Result result1, Result result2)
        {
            int citiesNumber = result1.sequence.Count;
            int breakpoint = rand.Next(1, citiesNumber);
            List<int> newSequence = new List<int>();
            List<int> missing = new List<int>();
            for(int i = 0; i < citiesNumber; i++)
            {
                if (i < breakpoint) { newSequence.Add(result1.sequence[i]); }
                else 
                { 
                    if (result1.sequence[i] != result1.sequence[0]) { missing.Add(result1.sequence[i]); } 
                }
            }
            for(int i = 0; i < citiesNumber; i++)
            {
                for(int  j = 0; j < missing.Count; j++) 
                { 
                    if (result2.sequence[i] == missing[j])
                    {
                        newSequence.Add(result2.sequence[i]);
                        missing.Remove(missing[j]);
                        break;
                    }
                }
            }
            newSequence.Add(result1.sequence[0]);
            Result returnResult = new Result();
            List<int> tempDistances = new List<int>();
            for (int i = 0; i < citiesNumber-1; i++)
            {
                string firstCity = "";
                string secondCity = "";
                
                for(int j = 0; j< citiesNumber-1; j++)
                {
                    if(int.Parse(list[j].id) == newSequence[i]) { firstCity = list[j].name; }
                }
                for (int j = 0; j < citiesNumber-1; j++)
                {
                    if(i+1 >= newSequence.Count) { break; }
                    if (int.Parse(list[j].id) == newSequence[i+1]) { secondCity = list[j].name; }
                }
                if (secondCity != "")
                {
                    int index = list.FindIndex(match: x => x.name.Equals(firstCity));
                    tempDistances.Add(int.Parse(list[index].roads[secondCity]));
                }
            }
            returnResult.sequence = newSequence;
            returnResult.distances = tempDistances;
            int cycleLength = 0;
            for (int i = 0; i < returnResult.distances.Count; i++)
            {
                cycleLength += returnResult.distances[i];
                returnResult.distancesAsc.Add(cycleLength);
            }
            returnResult.cycleLength = cycleLength;
            return returnResult;
        }

        public static Result Mutation(List<City> list, Result result)
        {
            Result returnResult = new Result();
            List<int> tempSeq = new List<int>();
            for(int i = 0; i<result.sequence.Count; i++)
            {
                if (result.sequence[i] != result.sequence[0]) { tempSeq.Add(result.sequence[i]); }
            }
            int x = rand.Next(0, list.Count-1);
            int y = rand.Next(0, list.Count-1);
            int temp = tempSeq[x];
            tempSeq[x] = tempSeq[y];
            tempSeq[y] = temp;
            List<int> returnSeq = new List<int>
            {
                result.sequence[0]
            };
            for (int i = 0; i<tempSeq.Count; i++)
            {
                returnSeq.Add(tempSeq[i]);
            }
            returnSeq.Add(result.sequence[0]);
            returnResult.sequence = returnSeq;
            List<int> tempDistances = new List<int>();
            for (int i = 0; i < result.sequence.Count; i++)
            {
                string firstCity = "";
                string secondCity = "";

                for (int j = 0; j < result.sequence.Count-1; j++)
                {
                    if (int.Parse(list[j].id) == returnSeq[i]) { firstCity = list[j].name; }
                }
                for (int j = 0; j < result.sequence.Count-1; j++)
                {
                    if (i + 1 >= returnSeq.Count) { break; }
                    if (int.Parse(list[j].id) == returnSeq[i + 1]) { secondCity = list[j].name; }
                }
                if (secondCity != "")
                {
                    int index = list.FindIndex(match: c => c.name.Equals(firstCity));
                    tempDistances.Add(int.Parse(list[index].roads[secondCity]));
                }
            }
            returnResult.distances = tempDistances;
            int cycleLength = 0;
            for (int i = 0; i < returnResult.distances.Count; i++)
            {
                cycleLength += returnResult.distances[i];
                returnResult.distancesAsc.Add(cycleLength);
            }
            returnResult.cycleLength = cycleLength;
            return returnResult;
        }

        public static List<Result> Selection(List<Result> resultList, int popsize)
        {
            List<Result> tempList = resultList.OrderBy(result => result.cycleLength).ToList();      
            resultList = tempList.Take(popsize).ToList();
            return resultList;
        }

        public override string ToString()
        {
            string outString = "";
            outString += "Sequence: ";
            for (int i = 0; i < sequence.Count ; i++)
            {
                outString += sequence[i].ToString() + "; ";
            }
            outString += "\n Length of cycle: " + cycleLength.ToString() + "\n" + "Distances: ";
            for (int i = 0; i < distances.Count; i++)
            {
                outString += distances[i].ToString() + "; ";
            }
            outString += "\n Distances ascending: ";
            for(int i = 0; i < distancesAsc.Count; i++)
            {
                outString += distancesAsc[i].ToString() + "; ";
            }
            return outString;
        }
        public string LengthsToString()
        {
            string outString = cycleLength.ToString();
            return outString;
        }
    }
}
