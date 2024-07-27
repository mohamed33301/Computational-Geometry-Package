using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

            bool[] vis = new bool[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vis[i] = false;
            }

            BuildSegmentsAndFindExtremePoints(points, vis, outPoints, outLines);
            if (points.Count > 0 && outPoints.Count == 0)
            {
                outPoints.Add(points[0]);
            }
            return;



        }
        private void BuildSegmentsAndFindExtremePoints(List<Point> points, bool[] vis, List<Point> outPoints, List<Line> outLines)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (i > 0 && ArePointsEqual(points[i], points[i - 1]))
                    continue;

                for (int j = i + 1; j < points.Count; j++)
                {
                    if (ArePointsEqual(points[i], points[j]))
                        continue;
                    if (j > 0 && ArePointsEqual(points[j], points[j - 1]))
                        continue;

                    Line seg = new Line(points[i], points[j]);

                    int right = 0, left = 0, co = 0;
                    CheckPointsRelationships(points, i, j, seg, ref right, ref left, ref co, vis, outLines);

                    if (left == 0 || right == 0)
                    {
                        AddUniquePoints(vis, outPoints, points, i, j);
                        outLines.Add(seg);
                    }
                }
            }
        }

        private void CheckPointsRelationships(List<Point> points, int i, int j, Line seg, ref int right, ref int left, ref int co, bool[] vis, List<Line> outLines)
        {
            for (int k = 0; k < points.Count; k++)
            {
                if (k == i || k == j)
                    continue;

                Point p = points[k];
                Enums.TurnType t = HelperMethods.CheckTurn(seg, p);
                if (t == Enums.TurnType.Right)
                    right += 1;
                else if (t == Enums.TurnType.Left)
                    left += 1;
                else if (t == Enums.TurnType.Colinear)
                {
                    co += 1;
                    if (!IsPointInsideSegment(seg, p))
                    {
                        left += 1;
                        right += 1;
                        break;
                    }
                }
            }
        }
        private bool ArePointsEqual(Point a, Point b)
        {
            // collinear ?
            double epsilon = Constants.Epsilon;
            return Math.Abs(a.X - b.X) <= epsilon && Math.Abs(a.Y - b.Y) <= epsilon;
        }
        private void AddUniquePoints(bool[] vis, List<Point> outPoints, List<Point> points, int i, int j)
        {
            if (!vis[i])
            {
                vis[i] = true;
                outPoints.Add(points[i]);
            }
            if (!vis[j])
            {
                vis[j] = true;
                outPoints.Add(points[j]);
            }
        }
        public bool IsPointInsideSegment(Line l, Point p)
        {
            double minX = Math.Min(l.Start.X, l.End.X);
            double maxX = Math.Max(l.Start.X, l.End.X);
            double minY = Math.Min(l.Start.Y, l.End.Y);
            double maxY = Math.Max(l.Start.Y, l.End.Y);

            return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY;
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
