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
		/// 
		/// find point faster with the help of the grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PictureBox_Click(object sender, EventArgs e) 
		{
			var array = algorithm.GetCustomArray;
			var (y,x) = algorithm.GetGridBucketIndices(((MouseEventArgs)e).Y, ((MouseEventArgs)e).X);

			for (int i = 0; i < array[y,x].Count; i++)
			{
				if (((MouseEventArgs)e).X >= array[y,x][i].X - DRAWING_SIZE / 2 && ((MouseEventArgs)e).X <= array[y,x][i].X + DRAWING_SIZE / 2
					&& ((MouseEventArgs)e).Y >= array[y,x][i].Y - DRAWING_SIZE / 2 && ((MouseEventArgs)e).Y <= array[y,x][i].Y + DRAWING_SIZE / 2)
				{
					if (pointRemove.X >= 0 && pointRemove.Y >= 0)
						drawPoint(pointRemove, Pens.Black, Brushes.Black);
					pointRemove = array[y,x][i];
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
}



