using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp1
{
	class Program
	{
        static Random rnd = new Random();
        static double c; //левая граница
        static double d; //правая граница
        static double lambda; //лямбда
        static List<double> Selection;
        static double F(double x, double lambda)
        {
            return lambda * x - Math.Pow(lambda, 2) * Math.Pow(x, 2) / 4;
        }
        static double GenerateRandomVariable(double lambda, double cy, double dy)
        {
            double y = cy + rnd.NextDouble() * (dy - cy);
            //double y = rnd.NextDouble();
            return 2 * (1 - Math.Sqrt(1 - y)) / lambda;
        }
        static List<double> GenerateSelection(double amount, double left, double right, double lambda)
        {
            double cy, dy;
            if (right < left)
                (right, left) = (left, right); //смена границ
            if (lambda > 0)
            {
                if (left < 0)
                    cy = 0;
                else
                    cy = F(left, lambda);
            }
            else if (lambda < 0)
            {
                if (left < -lambda)
                    cy = 0;
                else
                    cy = F(left, lambda);
            }
            else
                throw new Exception("lambda = 0");


            if (right > 2 / lambda)
                dy = 1;
            else
                dy = F(right, lambda);

            List<double> Selection = new List<double>();
            for (int i = 0; i < amount; i++)
            {
                Selection.Add(GenerateRandomVariable(lambda, cy, dy));
            }
            return Selection;
        }
        static void Main(string[] args)
		{
            c = 0; //c = numericUpDown...
            d = 2;
            lambda = 1;
            int selectionSize = 1000;
            Selection = GenerateSelection(selectionSize, c, d, lambda);




            List<PointF> points = new List<PointF>();
            int chartSize = 20;
            for (int i = 0; i < chartSize; i++)
            {
                float step = Convert.ToSingle((d - c) / (chartSize));
                float x = Convert.ToSingle(c + step * i);
                int count = 0;
                for (int j = 0; j < selectionSize; j++)
                {
                    if (Selection[j] <= x)
                        count++;
                }
                float y = (float)count / selectionSize;
                points.Add(new PointF(x, y));
            }
			for (int i = 0; i < chartSize; i++)
			{
                Console.WriteLine("x = " + points[i].X + " y = " + points[i].Y);
			}
            //for (int i = 0; i < chartSize; i++)
            //{
            //    int n = (int)(points[i].Y * 10);
            //    for (int j = 0; j < n; j++)
            //    {
            //        Console.Write("|");
            //    }
            //    Console.WriteLine();
            //}
        }
	}
}
