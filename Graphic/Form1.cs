using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Graphic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void GenerateAndPrint()
		{
            if (numericUpDown3.Value == 0)
                throw new Exception("Лямбда равна 0!");
            if (numericUpDown2.Value <= numericUpDown1.Value)
                throw new Exception("Правая граница меньше левой!");

            double left = Convert.ToDouble(numericUpDown1.Value);
            double right = Convert.ToDouble(numericUpDown2.Value);
            double lambda = Convert.ToDouble(numericUpDown3.Value);
            int selectionSize = Convert.ToInt32(numericUpDown4.Value);

            List<double> Selection = LambdaRandVarGenerator.GenerateSelection(selectionSize, left, right, lambda);

            #region Сохранение выборки
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = filePath + @"\Numbers.txt";
            StreamWriter file = new StreamWriter(filePath, false);
            foreach (double d in Selection)
                file.WriteLine(d);
            file.Close();
            Process.Start(filePath);
            #endregion

            #region Параметры графика
            chart1.Series.Clear();
            chart1.Series.Add("Эксперементальная функция");
            chart1.Series["Эксперементальная функция"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Эксперементальная функция"].Color = Color.Red;
            chart1.Series.Add("Теоритическая функция");
            chart1.Series["Теоритическая функция"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Теоритическая функция"].Color = Color.Blue;
            chart1.Series.Add("Критерий Колмогорова");
            chart1.Series["Критерий Колмогорова"].BorderWidth = 2;
            chart1.Series["Критерий Колмогорова"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Критерий Колмогорова"].Color = Color.Green;
            chart1.ChartAreas[0].AxisX.Minimum = left;
            chart1.ChartAreas[0].AxisX.Maximum = right;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 1;
            chart1.ChartAreas[0].BorderWidth = 4;
            #endregion

            #region Построение графиков функций
            int chartSize = 100; //Количество точек для построения графика
            for (int i = 0; i < chartSize; i++)
            {
                float step = Convert.ToSingle((right - left) / (chartSize));
                float x = Convert.ToSingle(left + step * i);

                float y = Convert.ToSingle(LambdaRandVarGenerator.FinRange(x, lambda, left, right));
                chart1.Series["Теоритическая функция"].Points.AddXY(x, y);

                y = (float)Selection.Where(p => p <= x).Count() / Selection.Count;
                chart1.Series["Эксперементальная функция"].Points.AddXY(x, y);
            }
            #endregion

            #region Критерий Коломогорова
            //1. Сортируем
            Selection.Sort();
            //2. Перебираем элементы в порядке возрастания и сравниваем |Fэкспер. - Fтеор.|, находим максимум этой разности
            double xMaxDif = Selection[0];
            double yMaxDif = 0;
            double difMax = -1;
            for (int i = 0; i < Selection.Count; i++)
            {
                double Fteor = LambdaRandVarGenerator.FinRange(Selection[i], lambda, left, right);
                double Fexp = (double)(i + 1) / Selection.Count;
                double dif = Math.Abs(Fteor - Fexp);
                if (dif >= difMax)
                {
                    difMax = dif;
                    xMaxDif = Selection[i];
                    yMaxDif = Fexp;
                }
            }
            //3. Выводим результат
            chart1.Series["Критерий Колмогорова"].Points.AddXY(xMaxDif, yMaxDif);
            chart1.Series["Критерий Колмогорова"].Points.AddXY(xMaxDif, LambdaRandVarGenerator.FinRange(xMaxDif, lambda, left, right));
            listBox1.Items.Clear();
            listBox1.Items.Add("Критерий Колмогорова:");
            listBox1.Items.Add(Math.Round(difMax, 5));
            #endregion

            #region Параметры выборки
            listBox1.Items.Add("Среднее выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetAverage(Selection), 5));
            listBox1.Items.Add("Медиана выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetMedian(Selection), 5));
            listBox1.Items.Add("Мода выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetMode(Selection), 5));
            listBox1.Items.Add("Дисперсия выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetDispersion(Selection), 5));
            listBox1.Items.Add("Среднеквадратичное отклонение выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetStandardDeviation(Selection), 5));
            #endregion
            
        }
        private void Start(object sender, EventArgs e)
        {
			try
			{
                GenerateAndPrint();
			}
			catch (Exception exception)
			{
                MessageBox.Show(exception.Message);
			}
        }
    }
}
