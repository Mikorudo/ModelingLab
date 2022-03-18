using System;
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
                float y = (float)Selection.Where(p => p < x).Count() / Selection.Count;
                //int count = 0;
				//for (int j = 0; j < selectionSize; j++)
				//{
                //    if (Selection[j] <= x)
                //        count++;
				//}

                //float y = (float)count / selectionSize;
                chart1.Series["Эксперементальная функция"].Points.AddXY(x, y);
			}
            int pocketNum = 7;
            double[] pocketStep = new double[];
            double[] Pi = 1 / pocketNum;
            int[] Mi = new int[7];
            double moveK = F(left, lambda);
            double scaleK =  1 / (F(right, lambda) - F(left, lambda));
			for (int i = 0; i < pocketNum; i++)
			{
                Pi[i] = (F(left + pocketStep * (i + 1), lambda) - moveK) * scaleK - (F(left + pocketStep * i, lambda) - moveK) * scaleK;
                Mi[i] = 0;
				foreach (double item in Selection)
				{
                    if (item > left + pocketStep * i && item <= left + pocketStep * (i + 1))
                        Mi[i]++;
				}
			}


            double hiKv = 0;
			for (int i = 0; i < pocketNum; i++)
			{
                if (Pi[i] == 0)
                    continue;
                hiKv += (Mi[i] - Selection.Count * Pi[i]) * (Mi[i] - Selection.Count * Pi[i]) / (Selection.Count * Pi[i]);
            }
            listBox1.Items.Add(hiKv);
        }
	}
}
