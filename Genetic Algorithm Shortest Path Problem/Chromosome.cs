using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithm_Shortest_Path_Problem
{
    public class Chromosome : IComparable<Chromosome>
    {
        int[] path;
        int distance;
        public int[] Path { get => path; }
        public int Distance { get => distance; }
        int[] exceptNodes = new int[2]; // вершины, не изменяемые генетическими процессами (начало и конец пути)

        public Chromosome(int[] path, int distance)
        {
            this.path = new int[path.Length];
            Array.Copy(path, this.path, path.Length);
            this.distance = distance;
            exceptNodes[0] = path[0];
            exceptNodes[1] = path[path.Length - 1];
        }

        public void SetPath(int[] path, int distance)
        {
            if (this.path.Length != path.Length)
                this.path = new int[path.Length];
            Array.Copy(path, this.path, path.Length);
            this.distance = distance;
            exceptNodes[0] = path[0];
            exceptNodes[1] = path[path.Length - 1];
        }

        public bool CanBreedWith(Chromosome other)
            // хромосома может скреститься с другой только если
            // в серединах их решений есть одинаковые вершины графа            
            => !this.Equals(other) && other.Path
                                        .Except(exceptNodes)
                                        .Any(node => this.path.Contains(node));

        public int[] FindCrossoverNodes(Chromosome other)
            // возвращает все вершины графа, относительно которых хромосомы могут скреститься
            => other.Path
            .Except(exceptNodes)
            .Where(node => this.path.Contains(node)).ToArray();

        public Chromosome[] BreedWith(Chromosome other, Func<int[], int> distanceFunction)
        {
            List<Chromosome> children = new List<Chromosome>();
            int[] possibleCrossoverNodes = FindCrossoverNodes(other);
            for (int i = 0; i < possibleCrossoverNodes.Length; ++i)
            {
                int crossNode = possibleCrossoverNodes[i];
                int[] childPath1 = this.path.TakeWhile(node => node != crossNode)
                    .Concat(other.path.SkipWhile(node => node != crossNode))
                    .ToArray();
                int[] childPath2 = other.path.TakeWhile(node => node != crossNode)
                    .Concat(this.path.SkipWhile(node => node != crossNode))
                    .ToArray();
                children.Add(new Chromosome(childPath1, distanceFunction.Invoke(childPath1)));
                children.Add(new Chromosome(childPath2, distanceFunction.Invoke(childPath2)));
            }
            return children.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Chromosome other = obj as Chromosome;
            if (other == null) return false;
            return other.path.SequenceEqual(this.path) && other.distance.Equals(this.distance);
        }

        public override string ToString()
        {
            string s = "Path: ";
            if (path == null)
                s += "none";
            else
            {
                for (int i = 0; i < path.Length - 1; ++i)
                    s += path[i].ToString() + "-";
                s += path[path.Length - 1] + ", distance ";
                s += distance.ToString();
            }
            return s;
        }

        public int CompareTo(Chromosome other)
        {
            return this.Distance.CompareTo(other.Distance);
        }

        public override int GetHashCode()
        {
            var hashCode = -974168056;
            hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(path);
            hashCode = hashCode * -1521134295 + distance.GetHashCode();
            return hashCode;
        }
    }
}
