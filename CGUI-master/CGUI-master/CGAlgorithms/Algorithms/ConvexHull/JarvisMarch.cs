using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int numOfPoints = points.Count;

            if (numOfPoints < 3)
            {
                outPoints = points;
                return;
            }

            Point minPoint = FindLowestLeftmostPoint(points);
            outPoints.Add(minPoint);
            Point start = minPoint;

            Point extraPoint = new Point(minPoint.X - 20, minPoint.Y);

            while (true)
            {
                Point nextPoint = GetNextPoint(points, minPoint, extraPoint);

                if (start.X == nextPoint.X && start.Y == nextPoint.Y)
                    break;

                outPoints.Add(nextPoint);
                extraPoint = minPoint;
                minPoint = nextPoint;
            }
        }

        private Point FindLowestLeftmostPoint(List<Point> points)
        {
            Point lowestLeftmost = points[0];

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].Y < lowestLeftmost.Y || (points[i].Y == lowestLeftmost.Y && points[i].X < lowestLeftmost.X))
                {
                    lowestLeftmost = points[i];
                }
            }

            return lowestLeftmost;
        }

        private Point GetNextPoint(List<Point> points, Point minPoint, Point extraPoint)
        {
            double largestTheta = 0;
            double largestDistance = 0;
            Point nextPoint = minPoint;

            foreach (Point currentPoint in points)
            {
                Point minPointExtraPoint = new Point(minPoint.X - extraPoint.X, minPoint.Y - extraPoint.Y);
                Point minPointNextPoint = new Point(currentPoint.X - minPoint.X, currentPoint.Y - minPoint.Y);

                double dotProduct = DotProduct(minPointExtraPoint, minPointNextPoint);
                double crossProduct = HelperMethods.CrossProduct(minPointExtraPoint, minPointNextPoint);

                double distance = Distance(minPoint, currentPoint);
                double theta = CalculateTheta(crossProduct, dotProduct);

                if (theta < 0)
                    theta += (2 * Math.PI);

                if (theta > largestTheta || (theta == largestTheta && distance > largestDistance))
                {
                    largestTheta = theta;
                    largestDistance = distance;
                    nextPoint = currentPoint;
                }
            }

            return nextPoint;
        }

        public double DotProduct(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private double CalculateTheta(double cross, double dot)
        {
            return Math.Atan2(cross, dot);
        }



        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
