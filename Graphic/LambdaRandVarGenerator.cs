using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphic
{
	static class LambdaRandVarGenerator
	{
        static public double F(double x, double lambda)
        {
            if (lambda >= 0)
            {
                if (x <= 0)
                    return 0;
                if (x >= 2 / lambda)
                    return 1;
            }
            else
            {
                if (x <= 4 / lambda)
                    return 0;
                if (x >= 2 / lambda)
                    return 1;
            }
            return lambda * x - Math.Pow(lambda, 2) * Math.Pow(x, 2) / 4;
        }
        static public double FinRange(double x, double lambda, double left, double right)
		{
            return (F(x, lambda) - F(left, lambda)) / (F(right, lambda) - F(left, lambda));
        }
        static public double GenerateRandomVariable(double lambda, double leftProbability, double rightProbability, double rand0_1)
        {
            if (rand0_1 > 1 || rand0_1 < 0)
                throw new Exception("Параметр rand0_1 передан с недопустимым значением");
            double r = leftProbability + rand0_1 * (rightProbability - leftProbability);
            return 2 * (lambda - Math.Abs(lambda) * Math.Sqrt(1 - r)) / Math.Pow(lambda, 2);
        }
        static public List<double> GenerateSelection(int amount, double left, double right, double lambda)
        {
            double leftProbability, rightProbability;
            if (right <= left)
                throw new Exception("Правая граница меньше либо равна левой");

            if (lambda > 0)
            {
                if (left < 0)
                    leftProbability = 0;
                else
                    leftProbability = F(left, lambda);
            }
            else if (lambda < 0)
            {
                if (left < 4 / lambda)
                    leftProbability = 0;
                else
                    leftProbability = F(left, lambda);
            }
            else
                throw new Exception("lambda = 0");


            if (right > 2 / lambda)
                rightProbability = 1;
            else
                rightProbability = F(right, lambda);

            if (leftProbability == rightProbability)
                throw new Exception("На этом интервале не генерируется СВ");

            List<double> Selection = new List<double>();
            Random rnd = new Random();

            for (int i = 0; i < amount; i++)
            {
                Selection.Add(GenerateRandomVariable(lambda, leftProbability, rightProbability, rnd.NextDouble()));
            }
            return Selection;
        }
        static public double GetMedian(List<double> selection)
        {
            if (selection.Count == 0)
            {
                throw new Exception("Пустая выборка!");
            }
            List<double> copiedSelection = selection;
            copiedSelection.Sort();
            if (selection.Count % 2 == 1)
            {
                return copiedSelection[(copiedSelection.Count + 1) / 2];
            }
            else
            {
                return (copiedSelection[copiedSelection.Count / 2] + copiedSelection[(copiedSelection.Count + 1) / 2]) / 2;
            }
        }
        static public double GetMode(List<double> selection)
        {
            if (selection.Count == 0)
                throw new Exception("Пустая выборка!");

            Dictionary<double, int> dict = new Dictionary<double, int>();
            foreach (double elem in selection)
            {
                if (dict.ContainsKey(elem))
                    dict[elem]++;
                else
                    dict[elem] = 1;
            }

            int maxCount = 1;
            double mode = Double.NaN;
            foreach (double elem in dict.Keys)
            {
                if (dict[elem] > maxCount)
                {
                    maxCount = dict[elem];
                    mode = elem;
                }
            }
            return mode;
        }
        static public double GetDispersion(List<double> selection)
        {
            double m = selection.Average();
            return selection.Sum(a => (a - m) * (a - m)) / (selection.Count - 1);
        }
        static public double GetStandardDeviation(List<double> selection)
        {
            return Math.Sqrt(GetDispersion(selection));
        }
    }
}
