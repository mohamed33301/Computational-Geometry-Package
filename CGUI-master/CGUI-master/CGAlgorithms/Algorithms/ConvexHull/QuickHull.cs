using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point min_x = FindMinXPoint(points);
            Point max_x = FindMaxXPoint(points);

            List<Point> right = ComputeQuickHull(points, min_x, max_x, "Right");
            List<Point> left = ComputeQuickHull(points, min_x, max_x, "Left");

            List<Point> convexHull = MergePointLists(right, left);

            AddToOutPoints(outPoints, convexHull);

        }
        public List<Point> ComputeQuickHull(List<Point> points, Point min_x, Point max_x, string direction)
        {
            Enums.TurnType segment = GetTurnType(direction);
            int index;
            double max = FindMaxDistance(points, min_x, max_x, segment, out index);

            if (index == -1)
            {
                return new List<Point> { min_x, max_x };
            }

            List<Point> p1 = ComputeQuickHull(points, points[index], min_x, GetOppositeDirection(direction));
            List<Point> p2 = ComputeQuickHull(points, points[index], max_x, direction);

            return MergePointLists(p1, p2);
        }

        public Enums.TurnType GetTurnType(string direction)
        {
            return direction == "Right" ? Enums.TurnType.Right : Enums.TurnType.Left;
        }

        public string GetOppositeDirection(string direction)
        {
            return direction == "Right" ? "Left" : "Right";
        }

        public double FindMaxDistance(List<Point> points, Point min_x, Point max_x, Enums.TurnType segment, out int index)
        {
            index = -1;
            double max = -1;

            for (int i = 0; i < points.Count; i++)
            {
                double x = ComputeDistance(min_x, max_x, points[i]);
                Enums.TurnType turn = DetermineTurn(min_x, max_x, points[i]);

                if (turn == segment && x > max)
                {
                    index = i;
                    max = x;
                }
            }

            return max;
        }

        public void AddToOutPoints(List<Point> outPoints, List<Point> pointsToAdd)
        {
            foreach (Point point in pointsToAdd)
            {
                if (!outPoints.Contains(point))
                    outPoints.Add(point);
            }
        }

        public double ComputeDistance(Point first, Point second, Point third)
        {
            return Math.Abs((third.Y - first.Y) * (second.X - first.X) - (second.Y - first.Y) * (third.X - first.X));
        }
        public Enums.TurnType DetermineTurn(Point p1, Point p2, Point p3)
        {
            return CGUtilities.HelperMethods.CheckTurn(new Line(p1.X, p1.Y, p2.X, p2.Y), p3);
        }

        public Point FindMaxXPoint(List<Point> points)
        {
            Point maxX = new Point(double.MinValue, 0);
            foreach (Point point in points)
            {
                if (point.X > maxX.X)
                    maxX = point;
            }
            return maxX;
        }

        public Point FindMinXPoint(List<Point> points)
        {
            Point minX = new Point(double.MaxValue, 0);
            foreach (Point point in points)
            {
                if (point.X < minX.X)
                    minX = point;
            }
            return minX;
        }

        public List<Point> MergePointLists(List<Point> list1, List<Point> list2)
        {
            List<Point> mergedList = new List<Point>(list1);
            mergedList.AddRange(list2);
            return mergedList;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
