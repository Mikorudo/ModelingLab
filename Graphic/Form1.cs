﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphic
{
    public partial class Form1 : Form
    {
        static Random rnd = new Random();
        static double c; //левая граница
        static double d; //правая граница
        static double lambda; //лямбда
        List<double> Selection;
        public Form1()
        {
            InitializeComponent();
        }
        double F(double x, double lambda)
        {
            return lambda * x - Math.Pow(lambda, 2) * Math.Pow(x, 2) / 4;
        }
        double GenerateRandomVariable(double lambda, double cy, double dy)
        {
            //double y = cy + rnd.NextDouble() * (dy - cy);
            double y = rnd.NextDouble();
            return 2 * (1 - Math.Sqrt(1 - y)) / lambda;
        }
        List<double> GenerateSelection(double amount, double left, double right, double lambda)
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
            

            if (right > 2/lambda)
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
        private void button1_Click(object sender, EventArgs e)
        {
            c = 0; //c = numericUpDown...
            d = 2;
            lambda = 1;
            int selectionSize = 1000;
            Selection = GenerateSelection(selectionSize, c, d, lambda);
            foreach (double d in Selection)
            {
                listBox1.Items.Add(d);
            }
            //GraphicPoints = new List<PointF>();
            chart1.Series.Add("Эксперементальная функция");
            chart1.Series["Эксперементальная функция"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 2;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 1;
            chart1.ChartAreas[0].BorderWidth = 4;
            chart1.Series["Эксперементальная функция"].Color = Color.Black;
            List<PointF> points = new List<PointF>();
            int chartSize = 100; 
			for (int i = 0; i < chartSize; i++)
			{
                float step = Convert.ToSingle((d - c)/(chartSize));
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
                chart1.Series["Эксперементальная функция"].Points.AddXY(points[i].X, points[i].Y);
			}
        }
    }
}
