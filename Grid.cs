using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace convex_hull
{
	internal class Grid
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

			customArray = new List<Point>[gridSize + 1, gridSize + 1];
			for (int i = 0; i <= gridSize; i++)
			{
				for (int j = 0; j <= gridSize; j++)
				{
					customArray[i, j] = new List<Point>();
				}
			}
		}

		public void AddPoint(Point point)
		{
			customArray[point.Y / bucketSizeY, point.X / bucketSizeX].Add(point);
		}

		public void RemovePoint(Point point)
		{
			bool value = customArray[point.Y / bucketSizeY, point.X / bucketSizeX].Remove(point);
			if (!value)
				throw new Exception("removing non existing point");
		}

		public Tuple<int, int> GetBucketIndices(int coordinateY, int coordinateX)
		{
			return new Tuple<int, int>(coordinateY / bucketSizeY, coordinateX / bucketSizeX);
		}
	}
}
