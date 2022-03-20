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
            int selectionSize = Convert.ToInt32(numericUpDown4.Value); ;
            List<double> Selection = LambdaRandVarGenerator.GenerateSelection(selectionSize, left, right, lambda);
            listBox1.Items.Clear();
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
            listBox1.Items.Add("Критерий Колмогорова:");
            listBox1.Items.Add(Math.Round(difMax, 5));
            #endregion

            #region Параметры выборки
            listBox1.Items.Add("Медиана выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetMedian(Selection), 5));
            listBox1.Items.Add("Мода выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetMode(Selection), 5));
            listBox1.Items.Add("Дисперсия выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetDispersion(Selection), 5));
            listBox1.Items.Add("Среднеквадратичное отклонение выборки");
            listBox1.Items.Add(Math.Round(LambdaRandVarGenerator.GetStandardDeviation(Selection), 5));
            #endregion
            #region Критерий Хи квадрат
            /*int pocketNum = 1 + Convert.ToInt32(Math.Log(Selection.Count) / Math.Log(2));
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
            listBox1.Items.Add("Должно быть ~" + Math.Round(Selection.Count * P, 0));
            int index;
            for (index = 0; index < pocketNum - 1; index++)
                listBox1.Items.Add("[" + Math.Round(pocketBorder[index], 3) + "; " + Math.Round(pocketBorder[index + 1], 3) + ") - " + Mi[index]);

            listBox1.Items.Add("[" + Math.Round(pocketBorder[index], 3) + "; " + Math.Round(pocketBorder[index + 1], 3) + "] - " + Mi[index]);
            listBox1.Items.Add("Итого: " + Mi.Sum());
            */
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
