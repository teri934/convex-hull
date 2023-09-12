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
		/// color previous clicked point to the original black color
		/// and store the point to for removal
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PictureBox_Click(object sender, EventArgs e) 
		{
			var array = algorithm.GetCustomArray;
			var (y, x) = algorithm.GetGridBucketIndices(((MouseEventArgs)e).Y, ((MouseEventArgs)e).X);

			for (int i = 0; i < array[y,x].Count; i++)
			{
				if (((MouseEventArgs)e).X >= array[y,x][i].X - DRAWING_SIZE / 2 && ((MouseEventArgs)e).X <= array[y, x][i].X + DRAWING_SIZE / 2
					&& ((MouseEventArgs)e).Y >= array[y,x][i].Y - DRAWING_SIZE / 2 && ((MouseEventArgs)e).Y <= array[y, x][i].Y + DRAWING_SIZE / 2)
				{
					if (pointRemove.X >= 0 && pointRemove.Y >= 0)
						drawPoint(pointRemove, Pens.Black, Brushes.Black);
					pointRemove = array[y, x][i];
					drawPoint(pointRemove, Pens.Blue, Brushes.Blue);
				}
			}
		}

		/// <summary>
		/// remove found point and decide what
		/// to do next with the convex hull
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
			if (pointRemove.X == NEGATIVE)
				return;

			algorithm.RemovePoint(pointRemove);

			if (!convexHull.Contains(pointRemove))
			{
				drawPoint(pointRemove, Pens.White, Brushes.White);
				drawConvexHull();
				algorithm.RemovePointFromGrid(pointRemove);
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
		const int NUMBER_POINTS = 50;

		private List<Point> points = new List<Point>();
		private List<Point> convexHullPoints = new List<Point>();
		Grid grid;

		public JarvisAlgorithm(Point location, Size size, int drawingSize)
		{
			minWindowX = 0 + drawingSize;
			minWindowY = 0 + drawingSize;
			maxWindowX = minWindowX + size.Width - drawingSize;
			maxWindowY = minWindowY + size.Height - drawingSize;
			grid = new Grid(NUMBER_POINTS, size);
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
			for (int i = 0; i < NUMBER_POINTS; i++)
			{
				int x = random.Next(minWindowX, maxWindowX);
				int y = random.Next(minWindowY, maxWindowY);

				Point point = new Point(x, y);
				points.Add(point);
				// add point to the grid
				grid.AddPoint(point);
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

		/// <summary>
		/// perform Jarvis algorithm between two known points
		/// </summary>
		/// <param name="pointRemove"></param>
		public void RemoveRecalculate(Point pointRemove)
		{
			var indexRemove = convexHullPoints.IndexOf(pointRemove);
			int insertIndex = (indexRemove + convexHullPoints.Count - 1) % convexHullPoints.Count;
			Point startPoint = convexHullPoints[insertIndex];
			Point endPoint = convexHullPoints[(insertIndex + 2) % convexHullPoints.Count];

			int leftMostIndex = points.IndexOf(startPoint);
			int lastConvexIndex = leftMostIndex;
			int currentIndex;
			Point lastConvexPoint;

			int num = 0;
			do
			{
				convexHullPoints.Insert(indexRemove + num, points[lastConvexIndex]);
				currentIndex = (lastConvexIndex + 1) % points.Count;
				for (int test_index = 0; test_index < points.Count; test_index++)
				{
					if (orientationTest(points[lastConvexIndex], points[currentIndex], points[test_index]) == Orientation.CCW)
						currentIndex = test_index;
				}
				lastConvexIndex = currentIndex;
				lastConvexPoint = points[lastConvexIndex];
				num++;
			} while (lastConvexPoint != endPoint);

/*			remove point that should be removed (from the grid too) and
			duplicate startPoint */
			convexHullPoints.Remove(pointRemove);
			RemovePointFromGrid(pointRemove);
			convexHullPoints.Remove(startPoint);
		}

		public void RemovePointFromGrid(Point point)
		{
			grid.RemovePoint(point);
		}


		public Tuple<int,int> GetGridBucketIndices(int coordinateY, int coordinateX)
		{
			return grid.GetBucketIndices(coordinateY, coordinateX);
		}

		public List<Point> GetConvexHull => convexHullPoints;
		public List<Point> GetPoints => points;
		public List<Point>[,] GetCustomArray => grid.customArray;
	}
}


public class Grid
{
	readonly int gridSize;
	readonly int bucketSizeY;
	readonly int bucketSizeX;

	public List<Point>[,] customArray;

	public Grid(int num, Size dimen)
	{
		gridSize = (int)(Math.Ceiling(Math.Sqrt(num)));
		bucketSizeY = dimen.Height / gridSize;
		bucketSizeX = dimen.Width / gridSize;

		customArray = new List<Point>[gridSize+1, gridSize+1];
		for (int i = 0; i <= gridSize; i++)
		{
			for (int j = 0;  j <= gridSize;  j++)
			{
				customArray[i,j] = new List<Point>();
			}
		}
	}

	public void AddPoint(Point point)
	{
		customArray[point.Y/bucketSizeY, point.X / bucketSizeX].Add(point);
	}

	public void RemovePoint(Point point)
	{
		bool value = customArray[point.Y/bucketSizeY, point.X/bucketSizeX].Remove(point);
		if (!value)
			throw new Exception("removing non existing point");
	}

	public Tuple<int,int> GetBucketIndices(int coordinateY, int coordinateX)
	{
		return new Tuple<int,int> (coordinateY/bucketSizeY, coordinateX/bucketSizeX);
	}
}

