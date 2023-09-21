using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace convex_hull
{
	public partial class FormConvexHull : Form
	{
		JarvisAlgorithm algorithm;
		Graphics graphics;
		const int DRAWING_SIZE = 10;
		const int NEGATIVE = -1;
		List<Point> points, convexHull;
		Point pointRemove;
		int indexRemove;

		public FormConvexHull()
		{
			InitializeComponent();
			graphics = PictureBox.CreateGraphics();
		}

		/// <summary>
		/// clear canvas and delete data from previous computations
		/// </summary>
		private void clearCanvas()
		{
			graphics.Clear(Color.White);
			pointRemove = new Point(NEGATIVE, NEGATIVE);
		}

		/// <summary>
		/// drawing ellipse and filling it to draw a point
		/// </summary>
		/// <param name="point"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		private void drawPoint(Point point, Pen pen, Brush brush)
		{
			var rect = new Rectangle(point.X - DRAWING_SIZE / 2, point.Y - DRAWING_SIZE / 2, DRAWING_SIZE, DRAWING_SIZE);
			graphics.DrawEllipse(pen, rect);
			graphics.FillEllipse(brush, rect);
		}

		/// <summary>
		/// draw all points and their convex hull
		/// </summary>
		private void drawConvexHull()
		{
			convexHull = algorithm.GetConvexHull;
			points = algorithm.GetPoints;

			for (int i = 0; i < convexHull.Count; i++)
				graphics.DrawLine(Pens.Red, convexHull[i].X, convexHull[i].Y, convexHull[(i + 1) % convexHull.Count].X, convexHull[(i + 1) % convexHull.Count].Y);
			for (int i = 0; i < points.Count; i++)
			{
				drawPoint(points[i], Pens.Black, Brushes.Black);
			}
		}

		/// <summary>
		/// generate random points, calculate the convex hull
		/// and draw the result
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonGenerateRandom_Click(object sender, EventArgs e)
		{
			clearCanvas();
			algorithm = new JarvisAlgorithm(PictureBox.Location, PictureBox.Size, DRAWING_SIZE, 50);
			algorithm.CreateRandomPoints();
			algorithm.CalculateFromPoints();

			drawConvexHull();
		}

		/// <summary>
		/// if clicking on a point, recolor it and
		/// color previous clicked pint to original black color
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PictureBox_Click(object sender, EventArgs e)
		{
			Console.WriteLine(string.Format("X: {0} Y: {1}", ((MouseEventArgs)e).X, ((MouseEventArgs)e).Y));
			for (int i = 0; i < points.Count; i++)
			{
				if (((MouseEventArgs)e).X >= points[i].X - DRAWING_SIZE / 2 && ((MouseEventArgs)e).X <= points[i].X + DRAWING_SIZE / 2
					&& ((MouseEventArgs)e).Y >= points[i].Y - DRAWING_SIZE / 2 && ((MouseEventArgs)e).Y <= points[i].Y + DRAWING_SIZE / 2)
				{
					if (pointRemove.X >= 0 && pointRemove.Y >= 0)
						drawPoint(pointRemove, Pens.Black, Brushes.Black);
					pointRemove = points[i];
					indexRemove = i;
					drawPoint(pointRemove, Pens.Blue, Brushes.Blue);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonClear_Click(object sender, EventArgs e)
		{
			clearCanvas();
			algorithm = new JarvisAlgorithm(PictureBox.Location, PictureBox.Size, DRAWING_SIZE, 50);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void generate_benchmarks_Click(object sender, EventArgs e)
		{
			Random rand = new Random(42);
			string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "basic_version.txt"), false))
			{
				Stopwatch sw = new Stopwatch();
				for (int numPoints = 1000; numPoints <= 60_000; numPoints += 1000)
				{
					double suma = 0;
					for (int i = 0; i < 10; i++)
					{
						sw.Reset();
						JarvisAlgorithm benchmarkAlgorithm = new JarvisAlgorithm(PictureBox.Location, PictureBox.Size, DRAWING_SIZE, numPoints);
						benchmarkAlgorithm.CreateRandomPoints();
						benchmarkAlgorithm.CalculateFromPoints();


						var convexPoints = benchmarkAlgorithm.GetConvexHull;
						int randomIndex = rand.Next(convexPoints.Count);

						sw.Start();
						benchmarkAlgorithm.RemoveRecalculate(convexPoints[randomIndex]);
						sw.Stop();
						suma += sw.Elapsed.TotalSeconds;
					}

					outputFile.WriteLine($"{numPoints}	{suma/10}");
					Console.WriteLine($"{numPoints}	{suma / 10}");
				}
			}

		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (pointRemove.X == NEGATIVE)
				return;

			algorithm.RemovePoint(pointRemove);

			if (!convexHull.Contains(pointRemove))
			{
				drawPoint(pointRemove, Pens.White, Brushes.White);
				drawConvexHull();
			}
			else
			{
				algorithm.RemoveRecalculate(pointRemove);
				clearCanvas();
				drawConvexHull();
			}

			pointRemove = new Point(NEGATIVE, NEGATIVE);
		}
	}
}