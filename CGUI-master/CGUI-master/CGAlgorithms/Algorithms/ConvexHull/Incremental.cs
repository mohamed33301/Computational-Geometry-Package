using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                outPoints = points;
                return;
            }

            SortedSet<Tuple<double, int>> all = new SortedSet<Tuple<double, int>>(); // all points  
            Point midpoints = GetMidPoint(points[0], points[1]);
            midpoints = GetMidPoint(midpoints, points[2]);

            Point nextpoint = new Point(midpoints.X, midpoints.Y);
            Line supportingLine = new Line(midpoints, nextpoint);

            for (int i = 0; i < 3; i++)
            {
                double angle = CalculateAngle(supportingLine.Start, supportingLine.End, supportingLine.Start, points[i]);
                all.Add(new Tuple<double, int>(angle, i)); // add point (angle, index)
            }

            for (int i = 3; i < points.Count; i++)
            {
                ProcessPoints(points, all, supportingLine, midpoints, i);
            }

            foreach (var tuple in all)
            {
                outPoints.Add(points[tuple.Item2]);
            }
        }
        private void ProcessPoints(List<Point> points, SortedSet<Tuple<double, int>> allpoints, Line supportingLine, Point mid, int index)
        {
            Tuple<double, int> next = null;
            Tuple<double, int> previous = null;
            KeyValuePair<Tuple<double, int>, Tuple<double, int>> data = new KeyValuePair<Tuple<double, int>, Tuple<double, int>>();
            double angle = CalculateAngle(supportingLine.Start, supportingLine.End, mid, points[index]);

            data = GetUpperAndLowerPoints(allpoints, new Tuple<double, int>(angle, index));
            next = data.Key; // next point
            previous = data.Value; // previous point

            if (previous == null)
            {
                previous = allpoints.Last();
            }
            if (next == null)
            {
                next = allpoints.First();
            }
            Line line = new Line(points[previous.Item2], points[next.Item2]);
            Enums.TurnType type = HelperMethods.CheckTurn(line, points[index]);

            if (type == Enums.TurnType.Right)
            {
                Tuple<double, int> previous1 = null;
                data = GetUpperAndLowerPoints(allpoints, previous);
                previous1 = data.Value;

                if (previous1 == null)
                {
                    previous1 = allpoints.Last();
                }

                line = new Line(points[index], points[previous.Item2]);
                type = HelperMethods.CheckTurn(line, points[previous1.Item2]);

                while (type == Enums.TurnType.Left || type == Enums.TurnType.Colinear) // No supporting line
                {
                    allpoints.Remove(previous);
                    previous = previous1;
                    data = GetUpperAndLowerPoints(allpoints, previous);
                    previous1 = data.Value;

                    if (previous1 == null)
                    {
                        previous1 = allpoints.Last();
                    }

                    line = new Line(points[index], points[previous.Item2]);
                    type = HelperMethods.CheckTurn(line, points[previous1.Item2]);
                }

                Tuple<double, int> next1 = null;
                data = GetUpperAndLowerPoints(allpoints, next);
                next1 = data.Key;

                if (next1 == null)
                {
                    next1 = allpoints.First();
                }

                line = new Line(points[index], points[next.Item2]);
                type = HelperMethods.CheckTurn(line, points[next1.Item2]);

                while (type == Enums.TurnType.Right || type == Enums.TurnType.Colinear)
                {
                    allpoints.Remove(next);
                    next = next1;
                    data = GetUpperAndLowerPoints(allpoints, next);
                    next1 = data.Key;

                    if (next1 == null)
                    {
                        next1 = allpoints.First();
                    }

                    line = new Line(points[index], points[next.Item2]);
                    type = HelperMethods.CheckTurn(line, points[next1.Item2]);
                }

                allpoints.Add(new Tuple<double, int>(angle, index));
            }
        }
        private KeyValuePair<Tuple<double, int>, Tuple<double, int>> GetUpperAndLowerPoints(SortedSet<Tuple<double, int>> set, Tuple<double, int> point)
        {
            var upper = set.GetViewBetween(
                Tuple.Create(point.Item1, point.Item2 + 1),
                Tuple.Create(double.PositiveInfinity, int.MaxValue))
                .FirstOrDefault();

            var lower = set.GetViewBetween(
                Tuple.Create(double.NegativeInfinity, int.MinValue),
                Tuple.Create(point.Item1, point.Item2 - 1))
                .LastOrDefault();

            return new KeyValuePair<Tuple<double, int>, Tuple<double, int>>(upper, lower);
        }
        private Point GetMidPoint(Point point1, Point point2)
        {
            double midpoint_x = (point1.X + point2.X) / 2;
            double midpoint_y = (point1.Y + point2.Y) / 2;
            return new Point(midpoint_x, midpoint_y);
        }
        private double CalculateAngle(Point point1, Point point2, Point point3, Point point4)
        {
            double theta = Math.Atan2(point2.Y - point1.Y, point1.X - point2.X) - Math.Atan2(point4.Y - point3.Y, point3.X - point4.X);
            theta = theta * (180 / Math.PI);
            return theta;
        }
        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}