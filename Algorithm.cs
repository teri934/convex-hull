﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace convex_hull
{

	internal class JarvisAlgorithm
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


		public Tuple<int, int> GetGridBucketIndices(int coordinateY, int coordinateX)
		{
			return grid.GetBucketIndices(coordinateY, coordinateX);
		}

		public List<Point> GetConvexHull => convexHullPoints;
		public List<Point> GetPoints => points;
		public List<Point>[,] GetCustomArray => grid.customArray;
	}
}
