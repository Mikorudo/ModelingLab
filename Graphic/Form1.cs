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
    //Левая граница
    //Условие Пирсона
    //Пара

    public partial class Form1 : Form
    {
        static Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
        }
        //F(x) = Интеграл всех значение от -бесконечности до x 
        double F(double x, double lambda)
        {
            if (x >= 2 / lambda)
                return 1;
            if (x <= 0) //поменять условие для lamda < 0
                return 0;
            return lambda * x - Math.Pow(lambda, 2) * Math.Pow(x, 2) / 4; // -l^2*x^2 / 4 + l*x - r = 0;
            // D = l^2 - 4 * (-l^2 / 4) * (-r)
            //x1 = ((-l) - Корень(D)) / 2 -l^2
        }
        double GenerateRandomVariable(double lambda, double cy, double dy, double rand0_1)
        {
            double y = cy + rand0_1 * (dy - cy);
            return 2 * (1 - Math.Sqrt(1 - y)) / lambda;
        }
        List<double> GenerateSelection(int amount, double left, double right, double lambda)
        {
            double leftProbability, rightProbability;
            if (right < left)
                throw new Exception("right border less than left border");

            if (lambda > 0)
            {
                if (left < 0)
                    leftProbability = 0;
                else
                    leftProbability = F(left, lambda);
            }
            else if (lambda < 0)
            {
                if (left < 4/lambda)
                    leftProbability = 0;
                else
                    leftProbability = F(left, lambda);
            }
            else
                throw new Exception("lambda = 0");
            

            if (right > 2/lambda)
                rightProbability = 1;
            else
                rightProbability = F(right, lambda);

            if (leftProbability == rightProbability)
                throw new Exception("На этом интервале не генерируется СВ");

            List<double> Selection = new List<double>();
            for (int i = 0; i < amount; i++)
            {
                Selection.Add(GenerateRandomVariable(lambda, leftProbability, rightProbability, rnd.NextDouble()));
            }
            return Selection;
        }
        private void Start(object sender, EventArgs e)
        {
            if (numericUpDown3.Value == 0)
			{
                MessageBox.Show("Лямбда равна 0!");
                return;
			}
			if (numericUpDown2.Value <= numericUpDown1.Value)
			{
                MessageBox.Show("Правая граница меньше левой!");
                return;
            }

            double left = Convert.ToDouble(numericUpDown1.Value);
            double right = Convert.ToDouble(numericUpDown2.Value);
            double lambda = Convert.ToDouble(numericUpDown3.Value);
            int selectionSize = Convert.ToInt32(numericUpDown4.Value); ;
            List<double> Selection = GenerateSelection(selectionSize, left, right, lambda);
            listBox1.Items.Clear();
            foreach (double d in Selection)
                listBox1.Items.Add(d);
            #region Параметры графика
            chart1.Series.Clear();
            chart1.Series.Add("Эксперементальная функция");
            chart1.Series["Эксперементальная функция"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.Minimum = Convert.ToDouble(numericUpDown1.Value);
            chart1.ChartAreas[0].AxisX.Maximum = Convert.ToDouble(numericUpDown2.Value);
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 1;
            chart1.ChartAreas[0].BorderWidth = 4;
            chart1.Series["Эксперементальная функция"].Color = Color.Black;
            #endregion

            int chartSize = 100; 
			for (int i = 0; i < chartSize; i++)
			{
                float step = Convert.ToSingle((right - left)/(chartSize));
                float x = Convert.ToSingle(left + step * i);
                float y = (float)Selection.Where(p => p <= x).Count() / Selection.Count;
                
                chart1.Series["Эксперементальная функция"].Points.AddXY(x, y);
			}
            int pocketNum = 1 + Convert.ToInt32(Math.Log(Selection.Count)/Math.Log(2));
            double[] pocketBorder = new double[pocketNum + 1];
            double P = 1.0 / pocketNum;
            int[] Mi = new int[pocketNum];

            double Fa = F(left, lambda);
            double Fb = F(right, lambda);

            
			for (int i = 1; i < pocketNum; i++)
                pocketBorder[i] = GenerateRandomVariable(lambda, Fa, Fb, P * i);
            pocketBorder[0] = left;
            pocketBorder[pocketNum] = right;

			for (int i = 0; i < pocketNum; i++)
                Mi[i] = Selection.Where(x => x >= pocketBorder[i] && x < pocketBorder[i + 1]).Count();

            double hiKv = 0;
			for (int i = 0; i < pocketNum; i++)
			{
                hiKv += (Mi[i] - Selection.Count * P) * (Mi[i] - Selection.Count * P) / (Selection.Count * P);
            }
            listBox1.Items.Add("Хи квадрат: " + hiKv);
            int index;
            for (index = 0; index < pocketNum - 1; index++)
                listBox1.Items.Add("[" + Math.Round(pocketBorder[index], 3) + "; " + Math.Round(pocketBorder[index + 1], 3) + ") - " + Mi[index]);

            listBox1.Items.Add("[" + Math.Round(pocketBorder[index], 3) + "; " + Math.Round(pocketBorder[index + 1], 3) + "] - " + Mi[index]);
            listBox1.Items.Add("Итого: " + Mi.Sum());
        }
	}
    
}
