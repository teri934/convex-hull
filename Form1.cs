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
			algorithm = new JarvisAlgorithm(PictureBox.Location, PictureBox.Size, DRAWING_SIZE);
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
			algorithm = new JarvisAlgorithm(PictureBox.Location, PictureBox.Size, DRAWING_SIZE);
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{

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

	public class JarvisAlgorithm
	{
		Random random = new Random();
		private readonly int minWindowX, minWindowY, maxWindowX, maxWindowY;
		const int NEGATIVE = -1;

		private List<Point> points = new List<Point>();
		private List<Point> convexHullPoints = new List<Point>();

		public JarvisAlgorithm(Point location, Size size, int drawingSize)
		{
			minWindowX = 0 + drawingSize;
			minWindowY = 0 + drawingSize;
			maxWindowX = minWindowX + size.Width - drawingSize;
			maxWindowY = minWindowY + size.Height - drawingSize;
		}

		enum Orientation
		{
			CW,
			CCW,
			collinear
		}

		public bool RemovePoint(Point pointRemove)
		{
			return points.Remove(pointRemove);
		}

		public void CreateRandomPoints()
		{
			for (int i = 0; i < 50; i++)
			{
				int x = random.Next(minWindowX, maxWindowX);
				int y = random.Next(minWindowY, maxWindowY);

				points.Add(new Point(x, y));
			}
		}

		/// <summary>
		/// performs orientation test for ordered triplet of points in the order like in the function parameters
		/// </summary>
		/// <param name="lastConvexPoint"></param>
		/// <param name="currentPoint"></param>
		/// <param name="testPoint"></param>
		/// <returns>value of orientation test: 0 ~ collinear, -1 ~ CCW, 1 ~ CW</returns>
		private Orientation orientationTest(Point lastConvexPoint, Point currentPoint, Point testPoint)
		{
			int value = (currentPoint.Y - lastConvexPoint.Y) * (testPoint.X - currentPoint.X) - 
				(currentPoint.X - lastConvexPoint.X) * (testPoint.Y - currentPoint.Y);

			if (value == 0)
				return Orientation.collinear;
			else if (value < 0)
				return Orientation.CCW;
			else
				return Orientation.CW;
		}

		private double getDistance(Point point1, Point point2)
		{
			return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
		}

		/// <summary>
		/// finding the point from the set
		/// with the lowest X coordinate
		/// </summary>
		/// <returns></returns>
		private int findLeftMostPoint()
		{
			int leftMostIndex = 0;
			for (int i = 1; i < points.Count; i++)
			{
				if (points[i].X < points[leftMostIndex].X)
					leftMostIndex = i;
			}

			return leftMostIndex;
		}

		/// <summary>
		/// perform Jarvis March algorithm
		/// </summary>
		public void CalculateFromPoints()
		{
			int leftMostIndex = findLeftMostPoint();
			int lastConvexIndex = leftMostIndex;
			int currentIndex;

			do
			{
				convexHullPoints.Add(points[lastConvexIndex]);
				currentIndex = (lastConvexIndex + 1) % points.Count;
				for (int test_index = 0; test_index < points.Count; test_index++)
				{
					if (orientationTest(points[lastConvexIndex], points[currentIndex], points[test_index]) == Orientation.CCW)
						currentIndex = test_index;
				}
				lastConvexIndex = currentIndex;
			} while (leftMostIndex != lastConvexIndex);
		}


		public void RemoveRecalculate(Point pointRemove)
		{
			var indexRemove = convexHullPoints.IndexOf(pointRemove);
			Point newVertex = new Point(NEGATIVE, NEGATIVE);

			double min_distance = maxWindowX + maxWindowY;
			double distance;
			for (int i = 0; i < points.Count; i++)
			{
				distance = getDistance(pointRemove, points[i]);
				if (distance > min_distance) continue;

				if (orientationTest(
					convexHullPoints[(indexRemove + convexHullPoints.Count - 1) % convexHullPoints.Count],
					convexHullPoints[(indexRemove + 1) % convexHullPoints.Count],
					points[i]) == Orientation.CCW)
				{
					min_distance = distance;
					newVertex = points[i];
				}
			}

			// no new vertex to add, no update of newVertex
			if (newVertex.X < 0 || newVertex.Y < 0)
				convexHullPoints.RemoveAt(indexRemove);
			else
				convexHullPoints[indexRemove] = newVertex;
		}

		public List<Point> GetConvexHull => convexHullPoints;
		public List<Point> GetPoints => points;
	}
}

