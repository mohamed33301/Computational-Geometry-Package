using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    public class EventPoint
    {
        public Point point;
        public int index;
        public string event_type;
        public Point edge;

        public EventPoint(Point p, int index, string type, Point e)
        {
            this.point = p;
            this.index = index;
            this.event_type = type;
            this.edge = e;
        }
    }


    class MonotonePartitioning : Algorithm
    {
        public double Y = 0.0;

        public static double CalcAngle(Point p1, Point p, Point p2)
        {
            double angle = 0.0;

            Line l1 = new Line(p1, p);
            Line l2 = new Line(p, p2);

            Point a = HelperMethods.GetVector(l1);
            Point b = HelperMethods.GetVector(l2);
            double dotProduct = (a.X * b.X + a.Y * b.Y);
            double crossProduct = HelperMethods.CrossProduct(a, b);

            angle = Math.Atan2(crossProduct, dotProduct) * 180 / Math.PI;
            if (angle < 0)
                angle += 360;

            return angle;
        }

        public int CompareYX(EventPoint E1, EventPoint E2)
        {
            if (E1.point.Y < E2.point.Y) return -1;
            if (E1.point.Y > E2.point.Y) return 1;

            if (E1.point.X < E2.point.X) return -1;
            if (E1.point.X > E2.point.X) return 1;

            return 0;
        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<EventPoint> Q = new List<EventPoint>();
            Dictionary<int, EventPoint> prev_event = new Dictionary<int, EventPoint>();
            Dictionary<int, EventPoint> helper = new Dictionary<int, EventPoint>();
            List<EventPoint> T = new List<EventPoint>();
            string type;

            for (int i = 0; i < polygons[0].lines.Count; ++i)
            {
                Point n1 = polygons[0].lines[(polygons[0].lines.Count + i - 1) % polygons[0].lines.Count].Start;
                Point p = polygons[0].lines[i].Start;
                Point n2 = polygons[0].lines[(i + 1) % polygons[0].lines.Count].Start;
                double angle = CalcAngle(n1, p, n2);

                if (((n1.Y < p.Y) || ((n1.Y == p.Y) && (n1.X > p.Y))) && ((n2.Y < p.Y) || ((n2.Y == p.Y) && (n2.X > p.X))))
                {
                    if (angle < 180)
                    {
                        type = "Start";
                    }
                    else
                    {
                        type = "Split";
                    }
                }
                else if (((p.Y < n1.Y) || ((p.Y == n1.Y) && (p.X > n1.Y))) && ((p.Y < n2.Y) || ((p.Y == n2.Y) && (p.X > n2.X))))
                {
                    if (angle < 180)
                    {
                        type = "End";
                    }
                    else
                    {
                        type = "Merge";
                    }
                }
                else
                {
                    type = "Regular";
                }

                EventPoint ev = new EventPoint(polygons[0].lines[i].Start, i, type, polygons[0].lines[i].End);
                Q.Add(ev);
                EventPoint prev;
                if (i != (polygons[0].lines.Count - 1))
                {
                    prev = new EventPoint(polygons[0].lines[i].Start, i, type, polygons[0].lines[i].End);
                    prev_event.Add(i + 1, prev);
                }
                else
                {
                    prev = new EventPoint(polygons[0].lines[i].Start, i, type, polygons[0].lines[i].End);
                    prev_event.Add(0, prev);
                }
            }

            Q.Sort(CompareYX);

            while (Q.Count != 0)
            {
                EventPoint ev = Q[0];
                Y = ev.point.Y;

                Q.RemoveAt(0);

                if (ev.event_type == "Start")
                {
                    helper.Add(ev.index, ev);
                    T.Add(ev);
                }
                else if (ev.event_type == "End")
                {
                    if (helper.TryGetValue(ev.index - 1, out EventPoint evMinus1))
                    {
                        if (evMinus1.event_type == "Merge")
                        {
                            outLines.Add(new Line(ev.point, evMinus1.point));
                        }
                        T.Remove(evMinus1);
                    }
                }
                else if (ev.event_type == "Merge")
                {
                    if (helper.TryGetValue(ev.index - 1, out EventPoint ev2))
                    {
                        if (ev2.event_type == "Merge")
                        {
                            outLines.Add(new Line(ev.point, ev2.point));
                        }
                    }

                    if (prev_event.TryGetValue(ev.index, out EventPoint prev))
                    {
                        T.Remove(prev);
                    }

                    if (T.Count >= 2)
                    {
                        KeyValuePair<EventPoint, EventPoint> temp = new KeyValuePair<EventPoint, EventPoint>(T[T.Count - 1], T[T.Count - 2]);
                        if (temp.Value != null && helper.TryGetValue(temp.Value.index, out EventPoint tempValue))
                        {
                            if (tempValue.event_type == "Merge")
                            {
                                outLines.Add(new Line(ev.point, tempValue.point));
                            }
                            helper[temp.Value.index] = ev;
                        }
                    }
                }
                else if (ev.event_type == "Split")
                {
                    if (T.Count >= 2)
                    {
                        KeyValuePair<EventPoint, EventPoint> temp = new KeyValuePair<EventPoint, EventPoint>(T[T.Count - 1], T[T.Count - 2]);
                        if (temp.Value != null)
                        {
                            outLines.Add(new Line(ev.point, helper[temp.Value.index].point));
                            helper[temp.Value.index] = ev;
                        }
                    }

                    helper[ev.index] = ev;
                    T.Add(ev);
                }
                else
                {
                    var y = ev.edge.Y;
                    if (ev.point.Y > y)
                    {
                        if (helper.TryGetValue(ev.index - 1, out EventPoint evMinus1))
                        {
                            if (evMinus1.event_type == "Merge")
                            {
                                outLines.Add(new Line(ev.point, evMinus1.point));
                            }

                            T.Remove(prev_event[ev.index]);
                        }

                        helper.Add(ev.index, ev);
                        T.Add(ev);
                    }
                    else
                    {
                        if (T.Count >= 2)
                        {
                            KeyValuePair<EventPoint, EventPoint> temp = new KeyValuePair<EventPoint, EventPoint>(T[T.Count - 1], T[T.Count - 2]);
                            if (temp.Value != null)
                            {
                                if (helper[temp.Value.index].event_type == "Merge")
                                {
                                    outLines.Add(new Line(ev.point, helper[temp.Value.index].point));
                                    helper[temp.Value.index] = ev;
                                }
                            }
                        }
                    }
                }
            }
        }
        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}
