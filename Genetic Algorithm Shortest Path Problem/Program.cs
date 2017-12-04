using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithm_Shortest_Path_Problem
{
    class Program
    {
        static void Main(string[] args)
        {
            int pathStart = 0, pathEnd = 0;
            EdgeList graph = LoadGraph("d:\\dump\\graph4.txt", ref pathStart, ref pathEnd);
            Console.WriteLine("Loaded graph:\n");
            Console.WriteLine(graph.ToString());

            Random rand = new Random();
            int maxGenerations = 100;
            int minPopulationSize = 10, maxPopulationSize = 30;
            float mutationRate = rand.Next(5, 10) / 100.0f;
            int reportEvery = 10;

            GeneticSolver solver = new GeneticSolver(maxGenerations, minPopulationSize,
                maxPopulationSize, mutationRate, graph, pathStart, pathEnd, reportEvery);
            solver.Solve();

            Console.ReadLine();
        }

        private static EdgeList LoadGraph(string filePath, ref int pathStart, ref int pathEnd)
        {
            Random rand = new Random(0);
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line = sr.ReadLine();
                string[] parameters = line.Split(' ');
                int vCount = Int32.Parse(parameters[0]);
                pathStart = 1;
                pathEnd = Int32.Parse(parameters[1]);
                EdgeList result = new EdgeList(vCount);
                while ((line = sr.ReadLine()) != null)
                {
                    parameters = line.Split('-');
                    int from = Int32.Parse(parameters[0]);
                    int to = Int32.Parse(parameters[1]);
                    int weight = rand.Next(1, 10);
                    result.AddEdge(new Edge(from, to, weight));
                }
                return result;
            }
        }
    }
}
