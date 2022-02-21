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
            double y = cy + rnd.NextDouble() * (dy - cy);
            return 2 * (1 - Math.Sqrt(1 - y)) / lambda;
        }
        List<double> GenerateSelection(double amount, double c, double d, double lambda)
        {
            double cy, dy;
            if (d < c)
                (d, c) = (c, d); //смена границ если d < c

            if (c < 0)
                cy = F(0, lambda);
            else
                cy = F(c, lambda);

            if (d < 0)
                dy = F(0, lambda);
            else
                dy = F(d, lambda);

            List<double> Selection = new List<double>();
            for (int i = 0; i < amount; i++)
            {
                Selection.Add(GenerateRandomVariable(lambda, cy, dy));
            }
            return Selection;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            c = 0.5; //c = numericUpDown...
            d = 1.5;
            lambda = 1;
            Selection = GenerateSelection(1000, c, d, lambda);
            foreach (double d in Selection)
            {
                listBox1.Items.Add(d);
            }




            //GraphicPoints = new List<PointF>();
            chart1.Series.Add("Эксперементальная функция");
            chart1.Series["Эксперементальная функция"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            int size = 1000;
			for (int i = 0; i < size; i++)
			{
                float step = Convert.ToSingle((d - c)/(size/*-1*/));
                float x = Convert.ToSingle(c + step * i);
                float y = Selection.Where(p => p < x).Count() / Selection.Count;
                //GraphicPoints.Add(new PointF(x, y));
                chart1.Series["Эксперементальная функция"].Points.AddXY(x, y);
			}

        }
    }
}
