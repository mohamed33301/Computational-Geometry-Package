using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    public class Event
    {
        public Point Current_Point;
        public int status; // 0 : start , 1 : End ,  2 : intersection 
        public int L1_index;
        public int L2_index;

        public Event(Point p, int stat, int ind1, int ind2)
        {
            Current_Point = p;
            status = stat;
            L1_index = ind1;
            L2_index = ind2;
        }
    }
    class MonotoneTriangulation  :Algorithm
    {
        double SW_line = 3; 
        List<Line> copy_OF_input_lines = new List<Line>();

        List<Event> L_SweepLine; 
        List<Event> Q_EventPoints;

        List<Point> intersection_points_list = new List<Point>();


        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            // Copy input lines
            copy_OF_input_lines.AddRange(lines);

            // Initialize event points
            Q_EventPoints = new List<Event>();
            foreach (var line in lines)
            {
                Q_EventPoints.Add(new Event(line.Start, 0, lines.IndexOf(line), 0));
                Q_EventPoints.Add(new Event(line.End, 1, lines.IndexOf(line), 0));
            }

           
            Q_EventPoints.Sort(Event_Points_Sort);

           
            L_SweepLine = new List<Event>();

            foreach (var current_Event in Q_EventPoints)
            {
                HandleEvent(current_Event);
            }

            intersection_points_list.Sort(my_sort);

            
            for (int i = 0; i < intersection_points_list.Count - 1; i++)
            {
                if (intersection_points_list[i].Equals(intersection_points_list[i + 1]))
                {
                    intersection_points_list.RemoveAt(i);
                    i--;
                }
            }

            outPoints = intersection_points_list;
        }

        public int my_sort(Point p1, Point p2)
        {
            if (p1.X < p2.X)
            {
                return -1;
            }
            else if (p1.X > p2.X)
            {
                return 1;
            }
            else return 0;
        }

        public void HandleEvent(Event current_Event)
        {
            if (current_Event.status == 0)
            {
                // Handle start event
                L_SweepLine.Add(current_Event);
                CheckIntersections(current_Event, GetPrevious(current_Event), GetNext(current_Event));
            }
            else if (current_Event.status == 1)
            {
                // Handle end event
                CheckIntersections(GetPrevious(current_Event), GetNext(current_Event));
                L_SweepLine.Remove(current_Event);
            }
            else if (current_Event.status == 2)
            {
                // Handle intersection event
                Line S1 = copy_OF_input_lines[current_Event.L1_index];
                Line S2 = copy_OF_input_lines[current_Event.L2_index];

                L_SweepLine.Remove(current_Event);

                SwapS1S2InL(current_Event.L1_index, current_Event.L2_index);

                CheckIntersections(GetPrevious(current_Event), GetNext(current_Event));
            }
        }

        public void CheckIntersections(Event e1, Event e2)
        {
            if (e1 == null || e2 == null)
                return;

            int index1 = e1.L1_index;
            int index2 = e2.L1_index;

            Line line1 = copy_OF_input_lines[index1];
            Line line2 = copy_OF_input_lines[index2];

            if (doIntersect_between_2_Segments(line1, line2))
            {
                bool infinity_intersection_point = true;
                Point intersection_point = Get_intersection_point_cordinates(line1, line2, ref infinity_intersection_point);

                if (!infinity_intersection_point)
                {
                    intersection_points_list.Add(intersection_point);
                    Event intersection_Event = new Event(intersection_point, 2, index1, index2);
                    Q_EventPoints.Add(intersection_Event);
                }
            }
        }

        public void CheckIntersections(Event current_Event, Event prev_Event, Event next_Event)
        {
            CheckIntersections(prev_Event, current_Event);
            CheckIntersections(current_Event, next_Event);
        }

        public Event GetPrevious(Event current_Event)
        {
            int currentIndex = L_SweepLine.IndexOf(current_Event);
            if (currentIndex > 0)
                return L_SweepLine[currentIndex - 1];
            else
                return null;
        }

        public Event GetNext(Event current_Event)
        {
            int currentIndex = L_SweepLine.IndexOf(current_Event);
            if (currentIndex < L_SweepLine.Count - 1)
                return L_SweepLine[currentIndex + 1];
            else
                return null;
        }

        public void SwapS1S2InL(int index_S1, int index_S2)
        {
            Event S1 = L_SweepLine.Find(e => e.L1_index == index_S1);
            Event S2 = L_SweepLine.Find(e => e.L1_index == index_S2);

            L_SweepLine.Remove(S1);
            L_SweepLine.Remove(S2);

            Point temp = S1.Current_Point;
            S1.Current_Point = S2.Current_Point;
            S2.Current_Point = temp;

            L_SweepLine.Add(S1);
            L_SweepLine.Add(S2);
        }

        int Event_Points_Sort(Event e1, Event e2)
        {
            if (e1.Current_Point.X < e2.Current_Point.X)
            {
                return -1;
            }
            else if (e1.Current_Point.X > e2.Current_Point.X)
            {
                return 1;
            }
            else
            {
                if (e1.Current_Point.Y < e2.Current_Point.Y)
                {
                    return -1;
                }
                else if (e1.Current_Point.Y > e2.Current_Point.Y)
                {
                    return 1;
                }
                else
                    return 0;
            }
        }
        public static bool doIntersect_between_2_Segments(Line l1, Line l2)
        {
            

            if (l1.Start.X == l2.Start.X &&
                l1.Start.Y == l2.Start.Y &&
                l1.End.X == l2.End.X &&
                l1.End.Y == l2.End.Y)
            {
                return false;
            }
        
            if (HelperMethods.CheckTurn(l1, l2.Start) != HelperMethods.CheckTurn(l1, l2.End)
                && HelperMethods.CheckTurn(l2, l1.Start) != HelperMethods.CheckTurn(l2, l1.End))
            {
                return true;
            }
           
            if (HelperMethods.CheckTurn(l1, l2.Start) == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(l2.Start, l1.Start, l1.End))
            {
                return true;
            }

            
            if (HelperMethods.CheckTurn(l1, l2.End) == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(l2.End, l1.Start, l1.End))
            {
                return true;
            }
            
            if (HelperMethods.CheckTurn(l2, l1.Start) == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(l1.Start, l2.Start, l2.End))
            {
                return true;
            }
            if (HelperMethods.CheckTurn(l2, l1.End) == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(l1.End, l2.Start, l2.End))
            {
                return true;
            }

            return false; 
        }
        public static Point Get_intersection_point_cordinates(Line l1, Line l2, ref bool infinty_intersection_point)
        {
           
            infinty_intersection_point = true;
            double X1, Y1, X2, Y2, X3, Y3, X4, Y4;

            //l1.start
            X1 = l1.Start.X;
            Y1 = l1.Start.Y;

            //l1.end
            X2 = l1.End.X;
            Y2 = l1.End.Y;

            //l2.start
            X3 = l2.Start.X;
            Y3 = l2.Start.Y;

            //l2.end
            X4 = l2.End.X;
            Y4 = l2.End.Y;

            double part_1 = ((X4 - X3) * (Y3 - Y1)) - ((X3 - X1) * (Y4 - Y3));
            double part_2 = ((X4 - X3) * (Y2 - Y1)) - ((X2 - X1) * (Y4 - Y3));

            double S = 0;

            if (part_2 != 0) 
            {
                S = part_1 / part_2;
                infinty_intersection_point = false;
            }

          
            part_1 = ((X2 - X1) * (Y3 - Y1)) - ((X3 - X1) * (Y2 - Y1));
            part_2 = ((X4 - X3) * (Y2 - Y1)) - ((X2 - X1) * (Y4 - Y3));
            double t = 0;
            if (part_2 != 0)
            {
                t = part_1 / part_2;
                infinty_intersection_point = false;
            }


            Point intersection_point = new Point(0, 0);

            intersection_point.X = X1 + ((X2 - X1) * S);
            intersection_point.Y = Y1 + ((Y2 - Y1) * S);

            return intersection_point;

        }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}
