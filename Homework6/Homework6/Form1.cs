using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace Homework6
{
    public partial class Form1 : Form
    {
        private int populationSize = 100;
        private int samplePopuationSize = 10;
        private int numberOfSamplings = 10;

        private int unitMaxValue = 200;
        private int unitMinValue = 140;

        private List<double> totalPopulation, samplePopulation, varianceSamples, meanSamples;
        private Random random;
        private Bitmap b1,b2;
        private Graphics g1, g2;

        public Form1()
        {
            InitializeComponent();
            random = new Random();
            totalPopulation = new List<double>();
            for (int i = 0; i < populationSize; i++)
            {
                var unit = random.Next(unitMinValue, unitMaxValue);
                totalPopulation.Add(unit);
            }

            b1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g1 = Graphics.FromImage(b1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g1.Clear(Color.White);
            pictureBox1.Image = b1;

            b2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g2 = Graphics.FromImage(b2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g2.Clear(Color.White);
            pictureBox2.Image = b2;
        }

        private List<PointF> histogramFromValues(List<double> samples, Point maxValue, Point minValue, Rectangle rect)
        {
            var samplesArray = samples.ToArray();
            List<PointF> histogram = new List<PointF>();

            for (int x = 0; x < samplesArray.Length; x++)
            {
                var pointF = fromRealToVirtual(new PointF(x, 0), minValue, maxValue, rect);
                pointF.X = pointF.X + 50;
                var pointH = fromRealToVirtual(new PointF(x, (float)samplesArray[x]), minValue, maxValue, rect);
                pointH.X = pointH.X + 50;
                histogram.Add(pointF);
                histogram.Add(pointH);
            }

            return histogram;
        }

        private void drawSamplePopulation(List<double> population)
        {
            var populationArray = totalPopulation.ToArray();

            //number of different sample population that I draw
            for (int t = 0; t < numberOfSamplings; t++)
            {
                samplePopulation = new List<double>();

                //picking our sample from the population
                while (samplePopulation.Count < samplePopuationSize)
                {
                    var draw = random.Next(0, populationSize - 1);
                    samplePopulation.Add(populationArray[draw]);
                }

                var sampleMean = samplePopulation.Mean();
                var sampleVariance = samplePopulation.Variance();
                meanSamples.Add(sampleMean);
                varianceSamples.Add(sampleVariance);
            }
        }

        //generate sample population values
        private void button1_Click(object sender, EventArgs e)
        {
            g1.Clear(Color.White);
            g2.Clear(Color.White);
            Rectangle plottableWindowB1 = new Rectangle(20, 20, b1.Width - 40, b1.Height - 40);
            Rectangle plottableWindowB2 = new Rectangle(20, 20, b2.Width - 40, b2.Height - 40);
            meanSamples = new List<double>();
            varianceSamples = new List<double>();

            drawSamplePopulation(totalPopulation);
            Point minValue = new Point(0, 0);
            Point maxValue = new Point(samplePopuationSize, unitMaxValue*2);
            Point maxVValue = new Point(samplePopuationSize, unitMaxValue*2);

            g1.DrawRectangle(Pens.Black, plottableWindowB1);
            g2.DrawRectangle(Pens.Black, plottableWindowB2);

            //plotting means
            var meanPen = new Pen(Color.OrangeRed, 40);
            List<PointF> meansHistogram = histogramFromValues(meanSamples, minValue, maxValue, plottableWindowB1);
            var histoMeanArray = meansHistogram.ToArray();
            for (int i = 0; i < histoMeanArray.Length - 1; i++)
            {
                g1.DrawLine(meanPen, histoMeanArray[i], histoMeanArray[i + 1]);
                g1.DrawString(Math.Floor(histoMeanArray[i + 1].Y).ToString(), new Font("Ariel", 10.0f), Brushes.Black, histoMeanArray[i + 1]);
                i++;
            }

            var mean = totalPopulation.Mean();
            var meanTPen = new Pen(Color.Red, 2);
            PointF aMPoint = fromRealToVirtual(new PointF(0, (float)mean), minValue, maxValue, plottableWindowB1);
            PointF bMPoint = fromRealToVirtual(new PointF(plottableWindowB1.Width, (float)mean), minValue, maxValue, plottableWindowB1);
            g1.DrawLine(meanTPen, aMPoint, bMPoint);

            pictureBox1.Image = b1;

            //plotting variances
            var variancePen = new Pen(Color.YellowGreen, 40);
            List<PointF> variancesHistogram = histogramFromValues(varianceSamples, minValue, maxVValue, plottableWindowB2);
            var histoVarianceArray = variancesHistogram.ToArray();
            for (int i = 0; i < histoVarianceArray.Length - 1; i++)
            {
                g2.DrawLine(variancePen, histoVarianceArray[i], histoVarianceArray[i + 1]);
                g2.DrawString(Math.Floor(histoVarianceArray[i + 1].Y).ToString(), new Font("Ariel", 10.0f), Brushes.Black, histoVarianceArray[i + 1]);

                i++;
            }

            var variance = totalPopulation.Variance();
            var varianceTPen = new Pen(Color.Purple, 2);
            PointF aVPoint = fromRealToVirtual(new PointF(0, (float)mean), minValue, maxVValue, plottableWindowB2);
            PointF bVPoint = fromRealToVirtual(new PointF(plottableWindowB2.Width, (float)variance), minValue, maxVValue, plottableWindowB2);
            g2.DrawLine(varianceTPen, aVPoint, bVPoint);

            pictureBox2.Image = b2;
        }       

        private PointF fromRealToVirtual(PointF point, Point minValue, Point maxValue, Rectangle rect)
        {
            float newX = maxValue.X - minValue.X == 0 ? 0 : (rect.Left + rect.Width * (point.X - minValue.X) / (maxValue.X - minValue.X));
            float newY = maxValue.Y - minValue.Y == 0 ? 0 : (rect.Top + rect.Height - rect.Height * (point.Y - minValue.Y) / (maxValue.Y - minValue.Y));
            return new PointF(newX, newY);
        }
    }
}