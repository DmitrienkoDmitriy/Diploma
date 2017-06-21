using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System_signal;

namespace Diploma
{
    public partial class MainForm : Form
    {
        double megatmp = 0;
        public int maxCountOfPacks = 60;
        public int minCountOfPacks = 45;
        public int countOfPeriodicsEvents = 1;
        bool open = false;
        Signal signal = new Signal();
        List<double> ResultSignal = new List<double>();
        public MainForm()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(chData_MouseWheel);
        }
        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open = true;
            signal.Dispose();
            chart1.Series["Сигнал"].Points.Clear();
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                Array.Clear(lines, 0, 1);
                foreach (string line in lines)
                {
                    if (line != null)
                    {
                        signal.inputData.Add(new PointF((float)Convert.ToDouble(line.Split('\t')[0]), (float)Convert.ToDouble(line.Split('\t')[1].Replace('.', ','))));
                    }
                }
                for (int i = 0; i < signal.inputData.Count; i++)
                {
                    chart1.Series["Сигнал"].Points.AddXY(signal.inputData[i].Y, signal.inputData[i].X);
                    chart1.Series["Сигнал"].Points[i].Color = Color.Azure;
                }
                menuStrip1.Items[1].Visible = true;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            menuStrip1.Items[1].Visible = false;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].BackColor = Color.Gray;
        }
        private void chData_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0 && open == true)
                {
                    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) * 2;
                    double posXFinish = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) * 2;
                    double posYStart = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) *4;
                    double posYFinish = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) * 4;

                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                   
                    chart1.ChartAreas[0].AxisX.Interval = megatmp;
                    megatmp /= 2;

                }

                if (e.Delta > 0 && open == true)
                {
                    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

                    double posXStart = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;

                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    chart1.ChartAreas[0].AxisX.Interval = megatmp;

                    megatmp *= 2;
                    
                }
            }
            catch { }
        }//zoom chart
        public List<double> CosTransform(List<double> input, int numOfPacks)
        {
            int N = input.Count / numOfPacks;
            List<List<double>>output = new List<List<double>>();
            List<double> resultSum = new List<double>();
            double phi = Math.PI / 8.0f;
            double sumOfC = 0;
            
            for (int j = 0; j < numOfPacks; j++)
            {
                output.Add(new List<double>());
                for (int i = 0; i < N; i++)
                {
                     for (int x = j * N; x < j * N + N; x++)
                     {
                         sumOfC += input[x] * Math.Cos((2 * Math.PI * x * i) / N - phi);
                     }
                     output[j].Add(sumOfC);
                     sumOfC = 0;
                }
            }
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < output.Count; j++)
                {
                    sumOfC += output[j][i];
                }
                resultSum.Add(sumOfC / numOfPacks);
                sumOfC = 0;
            }
            return resultSum;
        }
        #region
        //public List<double> CosTransform1(List<double> input, int numOfPacks)
        //{
        //    int N = input.Count / numOfPacks;
        //    List<double> output = new List<double>();
        //    double phi = Math.PI / 8.0f;
        //    double sumOfC = 0;
        //    for (int i = 0; i < N; i++)
        //    {
        //        for (int x = 0; x < N; x++)
        //        {
        //            sumOfC += input[x] * Math.Cos((2 * Math.PI * x * i) / N - phi);
        //        }
        //        output.Add(sumOfC);
        //        sumOfC = 0;
        //    }
        //    return output;
        //}
        //public double CosTransform(List<double> _input, int startIndex, int N)
        //{
        //    List<double> output = new List<double>();
        //    List<double> input = new List<double>();
        //    for (int i = startIndex; i < startIndex+N; i++)
        //    {
        //        input.Add(_input[i]);
        //    }
        //    double phi = Math.PI / 8.0f;
        //    double sumOfC = 0;
        //    for (int i = 0; i < N; i++)
        //    {
        //        for (int x = 0; x < N; x++)
        //        {
        //            sumOfC += input[x] * Math.Cos((2 * Math.PI * x * i)/N - phi);
        //        }
        //        output.Add(sumOfC);
        //        sumOfC = 0;
        //    }
        //    for (int i = 0; i < N; i++)
        //    {
        //        sumOfC += output[i];
        //    }
        //    return sumOfC/N;
        //}
        #endregion
        public List<double> TransformFromCos(List<double> input)
        {
            List<double> output = new List<double>();
            int N = input.Count;
            double phi = Math.PI / 8.0f;
            double sumOfC = 0;
            for (int x = 0; x < N; x++)
            {
                for (int i = 0; i < N; i++)
                {
                    sumOfC += 2 / (N * Math.Sin(2 * phi)) * input[i] * Math.Sin(2 * Math.PI * x * i / N + phi);
                }
                output.Add(sumOfC);
                sumOfC = 0;
            }
            return output;
        }

        public double findMiddleSquaredError(List<double> OriginalData, List<double> transformedAndComebackAgainData, int numOfPacks)
        {
            double error = 0;
            double sumForError = 0;
            List<double> errors = new List<double>();
            for (int i = 0; i < numOfPacks; i++)
            {
                for (int j = 0; j < transformedAndComebackAgainData.Count; j++)
                {
                    errors.Add(OriginalData[i*transformedAndComebackAgainData.Count + j] - transformedAndComebackAgainData[j]);
                }
            }
            for (int i = 0; i < errors.Count; i++)
            {
                sumForError += Math.Pow(errors[i], 2);
            }
            error = Math.Sqrt(sumForError) / errors.Count;
            return error;
        }

        public void findPeriod(List<double> inputData, int countOftimes)
        {
            List<double> output = inputData;
            if (countOftimes < countOfPeriodicsEvents)
            {
                output = new List<double>();
                double error;
                double min = double.MaxValue;
                List<double> resultDataTmp = new List<double>();
                int tmpNumOfPacks = 0;
                for (int j = maxCountOfPacks; j > minCountOfPacks; j--)
                {
                    int numOfPacks = j;

                    List<double> middledTransformedData = new List<double>();
                    List<double> resultData = new List<double>();
                    middledTransformedData = CosTransform(inputData, numOfPacks);
                    resultData = TransformFromCos(middledTransformedData);
                    error = findMiddleSquaredError(inputData, resultData, numOfPacks);
                    if (error < min)
                    {
                        resultDataTmp = resultData;
                        min = error;
                        tmpNumOfPacks = numOfPacks;
                    }
                    progressBar1.Invoke(new Action(() => { progressBar1.Value++; }));
                }
                for (int i = 0; i < tmpNumOfPacks; i++)
                {
                    for (int j = i * resultDataTmp.Count; j < i * resultDataTmp.Count + resultDataTmp.Count; j++)
                    {
                        output.Add(inputData[j] - resultDataTmp[j - resultDataTmp.Count * i]);
                    }
                }
                open = true;
                chart1.Invoke(new Action(() =>
                {
                    chart1.Series["Сигнал"].Points.Clear();
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                    for (int j = 0; j < output.Count; j++)
                    {
                        chart1.Series["Сигнал"].Points.AddXY(j, output[j]);
                        chart1.Series["Сигнал"].Points[j].Color = Color.Azure;
                    }
                }));
                countOftimes++;
                findPeriod(output, countOftimes);
            }
            else
            {
                ResultSignal = output;
                return;
            }
        }
        public void func()
        {
            List<double> inputData = new List<double>();
            for (int i = 0; i < signal.inputData.Count; i++)
            {
                inputData.Add(signal.inputData[i].X);
            }
            findPeriod(inputData, 0);
        }
        private void періодикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = (maxCountOfPacks - minCountOfPacks) * countOfPeriodicsEvents;
            Thread myThread = new Thread(func);
            myThread.Start();
        }

        private void пошукСигналуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            signal.Dispose();
            for (int i = 0; i < ResultSignal.Count; i++)
            {
               signal.inputData.Add(new PointF((float)ResultSignal[i],i));
            }
            foreach (string line in signal.scalarMultiply(signal.inputData))
            {
                listBox1.Items.Add(line);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            double timeForChart = Convert.ToDouble(listBox1.SelectedItem.ToString().Split('\t')[1]);

            double maxAmplituda = double.MinValue;
            double minAmplituda = double.MaxValue;
            for (int i = 0; i < signal.inputData .Count; i++)
            {
                if (signal.inputData[i].Y >= timeForChart - 9 && signal.inputData[i].Y <= timeForChart + 9)
                {
                    chart1.Series["Сигнал"].Points[i].Color = Color.Red;
                    if (signal.inputData[i].X < minAmplituda)
                        minAmplituda = signal.inputData[i].X;
                    if (signal.inputData[i].X > maxAmplituda)
                        maxAmplituda = signal.inputData[i].X;
                }
            }
            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(Math.Round(minAmplituda,1), Math.Round(maxAmplituda, 1));
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(timeForChart - 20, timeForChart + 20);
        }
    } 
}









     







 
