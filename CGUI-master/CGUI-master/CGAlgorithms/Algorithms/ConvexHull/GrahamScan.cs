using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {        

            Stack<Point> l = new Stack<Point>();
            List<KeyValuePair<int, double>> theta = new List<KeyValuePair<int, double>>(); // pointIndex, Angle

            points.Sort((p1, p2) => p1.X.CompareTo(p2.X)); // to get lower X and Y, to avoid points on segments

            Point min_y = this.min_y(points);
            l.Push(min_y);
            points.Remove(min_y);

            Line l1 = new Line(min_y, new Point(min_y.X + 10.0, min_y.Y));

            for (int i = 0; i < points.Count; i++)
            {
                Line l2 = new Line(min_y, points[i]);

                Point vector1 = HelperMethods.GetVector(l1);
                Point vector2 = HelperMethods.GetVector(l2);

                double angle = Math.Atan2(HelperMethods.CrossProduct(vector1, vector2), DotProduct(vector1, vector2)) * (180.0 / Math.PI);
                theta.Add(new KeyValuePair<int, double>(i, angle));
            }

            theta.Sort((a, b) =>
            {
                if (a.Value == b.Value) return a.Key.CompareTo(b.Key);
                return a.Value.CompareTo(b.Value);
            });

            if (theta.Count != 0)
                l.Push(points[theta[0].Key]);

            for (int i = 1; i < theta.Count && l.Count >= 2;)
            {
                Point p = l.Peek();
                Point p_ = l.Pop();
                Point prev = l.Peek();
                l.Push(p_);

                Enums.TurnType turntybe_1 = HelperMethods.CheckTurn(new Line(prev, p), points[theta[i].Key]);

                if (turntybe_1 == Enums.TurnType.Left)
                {
                    l.Push(points[theta[i].Key]);
                    i++;
                }
                else if (turntybe_1 == Enums.TurnType.Colinear)
                {
                    l.Pop();
                    l.Push(points[theta[i].Key]);
                    i++;
                }
                else
                {
                    l.Pop();
                }
            }

            while (l.Count != 0)
            {
                outPoints.Add(l.Pop());
            }
        }

        private Point min_y(List<Point> points)
        {
            int index_min_y = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < points[index_min_y].Y)
                    index_min_y = i;
            }

            return points[index_min_y];
        }
        public static double DotProduct(Point vector1, Point vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
