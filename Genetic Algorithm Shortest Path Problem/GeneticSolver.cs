using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithm_Shortest_Path_Problem
{
    public class GeneticSolver
    {
        Random random;

        int maxGenerations;
        int minPopulationSize, maxPopulationSize;
        float mutationRate;
        int reportFrequency;

        EdgeList graph;
        int pathStart, pathEnd;
        int[] bestPath;
        int bestDistance;

        LinkedList<Chromosome> population;
        LinkedList<Chromosome> parentPool;

        public GeneticSolver(int maxGenerations, int minPopulationSize, int maxPopulationSize,
            float mutationRate, EdgeList graph, int pathStart, int pathEnd, int reportFrequency)
        {
            this.random = new Random();
            this.maxGenerations = maxGenerations;
            this.mutationRate = mutationRate;
            this.population = new LinkedList<Chromosome>();
            this.parentPool = new LinkedList<Chromosome>();
            this.graph = graph;
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.reportFrequency = reportFrequency;
            this.minPopulationSize = minPopulationSize;
            this.maxPopulationSize = maxPopulationSize;
        }

        public void Solve()
        {
            CreateFirstGeneration();
            int generation = 0;
            while (generation < maxGenerations)
            {
                KeepBestResult();
                if (generation % reportFrequency == 0)
                {
                    Console.WriteLine("Generation " + generation.ToString());
                    Console.WriteLine(ToString() + "\n");
                }
                ProcessSelection();
                ProcessCrossover();
                ProcessMutation();                
                ++generation;
            }
            Console.WriteLine("Generation " + generation.ToString());
            Console.WriteLine(ToString() + "\n");
        }

        private void KeepBestResult()
        {   // запоминает лучшее решение поколения (если оно лучше решений предыдущего)
            foreach (Chromosome chromosome in population)
            {
                if (bestPath == null || chromosome.Distance < bestDistance)
                {
                    bestPath = new int[chromosome.Path.Length];
                    Array.Copy(chromosome.Path, bestPath, chromosome.Path.Length);
                    bestDistance = chromosome.Distance;
                }
            }
        }

        private void CreateFirstGeneration()
        {   // генерирует изначальную популяцию со случайными решениями
            population.Clear();
            int initialPopulationSize = random.Next(minPopulationSize, maxPopulationSize);
            for (int i = 0; i < initialPopulationSize; ++i)
                AddRandomIndividual();
        }

        private void AddRandomIndividual()
        {
            int[] randomPath = graph.RandomPath(pathStart, pathEnd);
            int distance = graph.MeasureDistance(randomPath);
            Chromosome individual = new Chromosome(randomPath, distance);
            population.AddLast(individual);
        }

        private void ProcessSelection()
        {   // производит турнирный отбор среди особей
            // вся популяция разбивается на группы по 2-3 особи
            // лучшая особь группы попадает в пул родителей, остальные вымирают
            parentPool.Clear();
            List<Chromosome> competitionPool = new List<Chromosome>(3);
            while (population.Count > 0)
            {
                competitionPool.Clear();
                int poolSize = random.Next(2, 4);
                for (int i = 0; i < poolSize && population.Count > 0; ++i)
                {
                    competitionPool.Add(population.First.Value);
                    population.RemoveFirst();
                }
                competitionPool.Sort();
                parentPool.AddLast(competitionPool.First());
            }
        }

        private void ProcessCrossover()
        {   // прозводит попарное скрещивание особей, прошедших отбор
            // на выходе популяция состоит из родителей и получившихся потомков
            int nextGenerationSize = random.Next(minPopulationSize, maxPopulationSize);
            int requiredChildrenCount = nextGenerationSize - parentPool.Count;
            while (requiredChildrenCount > 0 && parentPool.Count > 1)
            {
                Chromosome parent = parentPool.ElementAt(random.Next(parentPool.Count));
                parentPool.Remove(parent);
                population.AddLast(parent);

                IEnumerable<Chromosome> partners = parentPool.Where(chromosome => chromosome.CanBreedWith(parent));
                if (partners.Count() > 0)
                {
                    Chromosome partner = partners.ElementAt(random.Next(partners.Count()));
                    parentPool.Remove(partner);
                    population.AddLast(partner);

                    Chromosome[] children = parent.BreedWith(partner, graph.MeasureDistance);
                    requiredChildrenCount -= children.Length;
                    foreach (Chromosome child in children)
                        population.AddLast(child);                                        
                }
            }
            // принудительное пополнение новыми особями, если популяция вымирает
            while (requiredChildrenCount-- > 0)   
                AddRandomIndividual();
        }

        private void ProcessMutation()
        {   // производит мутацию в некоторой доле популяции
            int mutantsCount = Convert.ToInt32(Math.Round(population.Count * mutationRate));
            for (int i = 0; i < mutantsCount; ++i)
            {
                int mutantIndex = random.Next(population.Count);
                Chromosome mutant = population.ElementAt(mutantIndex);
                int[] mutantPath = graph.ModifyRandomPath(mutant.Path);
                int distance = graph.MeasureDistance(mutantPath);
                mutant.SetPath(mutantPath, distance);
            }
        }

        public override string ToString()
        {
            string s = "Population size: " + population.Count.ToString() + " chromosomes\n";
            s += "Best path: ";
            if (bestPath == null)
                s += "none";
            else
            {
                for (int i = 0; i < bestPath.Length - 1; ++i)
                    s += bestPath[i].ToString() + "-";
                s += bestPath[bestPath.Length - 1] + ", ";
                s += "distance: " + bestDistance.ToString();
            }
            return s;
        }
    }
}
