using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    public class EventPoint
    {
        public static List<Line> lines;
        public Point point;
        public int index;
        public int event_type;
        public EventPoint intersection_segment_1;
        public EventPoint intersection_segment_2;

        // 0 intersection, 1 start, -1 end
        public EventPoint(Point p, int index, int type)
        {
            this.point = p;
            this.index = index;
            this.event_type = type;
            this.intersection_segment_1 = null;
            this.intersection_segment_2 = null;
        }

        public EventPoint(Point p, EventPoint s1, EventPoint s2, int type)
        {
            this.point = p;
            this.event_type = type;
            this.intersection_segment_1 = s1;
            this.intersection_segment_2 = s2;
        }
    }

    class SweepLine :Algorithm
    {
        public int Compare_X(EventPoint E1, EventPoint E2)
        {
            if (E2.point.X < E1.point.X) return 1;
            else if (E2.point.X > E1.point.X) return -1;

            if (E2.point.Y < E1.point.Y) return 1;
            else if (E2.point.Y > E1.point.Y) return -1;

            if (EventPoint.lines[E2.index].End.X < EventPoint.lines[E1.index].End.X) return 1;
            else if (EventPoint.lines[E2.index].End.X > EventPoint.lines[E1.index].End.X) return -1;

            return 0;
        }

        public int Compare_Y(EventPoint E1, EventPoint E2)
        {
            if (E2.point.Y < E1.point.Y) return 1;
            if (E2.point.Y > E1.point.Y) return -1;

            if (E2.point.X < E1.point.X) return 1;
            if (E2.point.X > E1.point.X) return -1;

            return 0;
        }

        public bool ComparePoints(Point P1, Point P2)
        {
            if (P1.X < P2.X || (P1.X == P2.X && P1.Y < P2.Y)) return true;
            else return false;
        }


        public Point twoLinesIntersectionPoint(Line L1, Line L2)
        {
            double L1_slope = (L1.Start.Y - L1.End.Y) / (L1.Start.X - L1.End.X);
            double L2_slope = (L2.Start.Y - L2.End.Y) / (L2.Start.X - L2.End.X);
            double L1_intercept = L1.Start.Y - (L1_slope * L1.Start.X);
            double L2_intercept = L2.Start.Y - (L2_slope * L2.Start.X);
            double X, Y;

            if (L1_slope == Double.PositiveInfinity)
            {
                X = L1.Start.X;
                Y = (L2_slope * X) + L2_intercept;
            }
            else if (L2_slope == Double.PositiveInfinity)
            {
                X = L2.Start.X;
                Y = (L1_slope * X) + L1_intercept;
            }
            else
            {
                X = (L2_intercept - L1_intercept) / (L1_slope - L2_slope);
                Y = L1_slope * X + L1_intercept;
            }
            return new Point(X, Y);
        }
        public bool TwoSegmentsIntersectionTest(Line L1, Line L2)
        {
            Enums.TurnType first_orientation = HelperMethods.CheckTurn(L1, L2.Start);
            Enums.TurnType second_orientation = HelperMethods.CheckTurn(L1, L2.End);
            Enums.TurnType third_orientation = HelperMethods.CheckTurn(L2, L1.Start);
            Enums.TurnType fourth_orientation = HelperMethods.CheckTurn(L2, L1.End);

            if (first_orientation != second_orientation && third_orientation != fourth_orientation) return true;
            if (first_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L2.Start, L1.Start, L1.End)) return true;
            if (second_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L2.End, L1.Start, L1.End)) return true;
            if (third_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L1.Start, L2.Start, L2.End)) return true;
            if (fourth_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L1.End, L2.Start, L2.End)) return true;

            return false;
        }
        public bool HasValue(double value)
        {
            return !Double.IsNaN(value) && !Double.IsInfinity(value);
        }


        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<EventPoint> eventPoints = new List<EventPoint>();
            HashSet<Tuple<int, int>> intersectedLines = new HashSet<Tuple<int, int>>();

            EventPoint.lines = lines;

            foreach (var line in lines)
            {
                if (!ComparePoints(line.Start, line.End))
                    lines[lines.IndexOf(line)] = new Line(line.End, line.Start);

                eventPoints.Add(new EventPoint(line.Start, lines.IndexOf(line), 1));
                eventPoints.Add(new EventPoint(line.End, lines.IndexOf(line), -1));
            }

            eventPoints.Sort((e1, e2) => Compare_X(e1, e2));

            List<EventPoint> activeSegments = new List<EventPoint>();

            for (int i = 0; i < eventPoints.Count; i++)
            {
                var currentEvent = eventPoints[i];

                if (currentEvent.event_type == 1)
                {
                    activeSegments.Add(currentEvent);

                    if (lines[currentEvent.index].Start.X == lines[currentEvent.index].End.Y)
                    {
                        foreach (var e in activeSegments)
                        {
                            if (TwoSegmentsIntersectionTest(lines[e.index], lines[currentEvent.index]))
                            {
                                Point pointOfIntersection = twoLinesIntersectionPoint(lines[e.index], lines[currentEvent.index]);

                                if (HasValue(pointOfIntersection.X) && HasValue(pointOfIntersection.Y) &&
                                    !intersectedLines.Contains(new Tuple<int, int>(currentEvent.index, e.index)))
                                {
                                    outPoints.Add(pointOfIntersection);
                                    intersectedLines.Add(new Tuple<int, int>(currentEvent.index, e.index));
                                    intersectedLines.Add(new Tuple<int, int>(currentEvent.index, e.index));
                                }
                            }
                        }
                        continue;
                    }

                    var pre = activeSegments.Find(e => e.index == currentEvent.index - 1);
                    var next = activeSegments.Find(e => e.index == currentEvent.index + 1);

                    if (pre != null && TwoSegmentsIntersectionTest(lines[pre.index], lines[currentEvent.index]))
                    {
                        Point pointOfIntersection = twoLinesIntersectionPoint(lines[pre.index], lines[currentEvent.index]);
                        if (!eventPoints.Contains(new EventPoint(pointOfIntersection, pre, currentEvent, 0)) &&
                            !intersectedLines.Contains(new Tuple<int, int>(pre.index, currentEvent.index)))
                            eventPoints.Add(new EventPoint(pointOfIntersection, pre, currentEvent, 0));
                    }

                    if (next != null && TwoSegmentsIntersectionTest(lines[currentEvent.index], lines[next.index]))
                    {
                        Point pointOfIntersection = twoLinesIntersectionPoint(lines[currentEvent.index], lines[next.index]);
                        if (!eventPoints.Contains(new EventPoint(pointOfIntersection, currentEvent, next, 0)) &&
                            !intersectedLines.Contains(new Tuple<int, int>(currentEvent.index, next.index)))
                            eventPoints.Add(new EventPoint(pointOfIntersection, currentEvent, next, 0));
                    }
                }
                else if (currentEvent.event_type == -1)
                {
                    var pre = activeSegments.Find(e => e.index == currentEvent.index - 1);
                    var next = activeSegments.Find(e => e.index == currentEvent.index + 1);

                    if (pre != null && next != null && TwoSegmentsIntersectionTest(lines[pre.index], lines[next.index]))
                    {
                        Point pointOfIntersection = twoLinesIntersectionPoint(lines[pre.index], lines[next.index]);
                        if (!eventPoints.Contains(new EventPoint(pointOfIntersection, pre, next, 0)) &&
                            !intersectedLines.Contains(new Tuple<int, int>(pre.index, next.index)))
                            eventPoints.Add(new EventPoint(pointOfIntersection, pre, next, 0));
                    }

                    activeSegments.RemoveAll(e => e.index == currentEvent.index);
                }
                else
                {
                    var s1 = currentEvent.intersection_segment_1;
                    var s2 = currentEvent.intersection_segment_2;

                    if (HasValue(currentEvent.point.X) && HasValue(currentEvent.point.Y) &&
                        !intersectedLines.Contains(new Tuple<int, int>(s1.index, s2.index)))
                    {
                        outPoints.Add(currentEvent.point);
                        intersectedLines.Add(new Tuple<int, int>(s1.index, s2.index));
                        intersectedLines.Add(new Tuple<int, int>(s2.index, s1.index));
                    }

                    var s1Prev = activeSegments.Find(e => e.index == s1.index - 1);
                    var s2Next = activeSegments.Find(e => e.index == s2.index + 1);

                    if (s1Prev != null && TwoSegmentsIntersectionTest(lines[s1Prev.index], lines[s2.index]))
                    {
                        Point pointOfIntersection = twoLinesIntersectionPoint(lines[s1Prev.index], lines[s2.index]);
                        if (!eventPoints.Contains(new EventPoint(pointOfIntersection, s1Prev, s2, 0)) &&
                            !intersectedLines.Contains(new Tuple<int, int>(s1Prev.index, s2.index)))
                            eventPoints.Add(new EventPoint(pointOfIntersection, s1Prev, s2, 0));
                    }

                    if (s2Next != null && TwoSegmentsIntersectionTest(lines[s1.index], lines[s2Next.index]))
                    {
                        Point pointOfIntersection = twoLinesIntersectionPoint(lines[s1.index], lines[s2Next.index]);
                        if (!eventPoints.Contains(new EventPoint(pointOfIntersection, s1, s2Next, 0)) &&
                            !intersectedLines.Contains(new Tuple<int, int>(s1.index, s2Next.index)))
                            eventPoints.Add(new EventPoint(pointOfIntersection, s1, s2Next, 0));
                    }

                    activeSegments.RemoveAll(e => e.index == s1.index);
                    activeSegments.RemoveAll(e => e.index == s2.index);

                    double px = currentEvent.point.X + 0.3;
                    double py1 = currentEvent.point.Y + 10000;
                    double py2 = currentEvent.point.Y - 10000;
                    Line sweepL = new Line(new Point(px, py1), new Point(px, py2));
                    Point p1 = twoLinesIntersectionPoint(sweepL, lines[s1.index]);
                    Point p2 = twoLinesIntersectionPoint(sweepL, lines[s2.index]);
                    s1.point = p1;
                    s2.point = p2;
                    activeSegments.Add(s1);
                    activeSegments.Add(s2);
                }
            }
        }
        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
