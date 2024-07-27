using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    public class smart_point
    {
        public Point Current_Point;
        public bool is_Ear;
        public Point Prev;
        public Point Next;

        public smart_point(Point p, bool status, Point p_prev, Point p_next)
        {
            Current_Point = p;
            is_Ear = status;
            Prev = p_prev;
            Next = p_next;
        }
        public smart_point()
        {
            Current_Point = new Point(0, 0);
            is_Ear = false;
            Prev = new Point(0, 0);
            Next = new Point(0, 0);

        }

    }
    class SubtractingEars : Algorithm
    {
        List<Line> interor_diagonals = new List<Line>();
        Point mini_x = new Point(double.MaxValue, double.MaxValue);
        int index_of_mini_x;
        List<smart_point> input_points_list = new List<smart_point>();
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<Point> input = new List<Point>(); 
           
            for (int i = 0; i < polygons[0].lines.Count(); i++)
            {
                Point p = polygons[0].lines[i].Start;
                if (p.X <= mini_x.X)
                {
                    if (p.X < mini_x.X)
                    {
                        mini_x = p;
                        index_of_mini_x = i;
                    }

                    else if (p.Y < mini_x.Y)
                    {
                        mini_x = p;
                        index_of_mini_x = i;
                    }
                }
                input.Add(p);
            }

            bool Points_sorted_CCW = IS_points_Sorted_CCW(mini_x, index_of_mini_x, input);
        
            if (Points_sorted_CCW == false)
            {
                for (int i = 0; i < polygons[0].lines.Count; i++)
                {
                    Point temp = polygons[0].lines[i].Start;

                    polygons[0].lines[i].Start = polygons[0].lines[i].End;
                    polygons[0].lines[i].End = temp;

                }
                polygons[0].lines.Reverse();
            }

          
            int size = polygons[0].lines.Count();
            for (int i = 0; i < size; i++)
            {
              

                Point p = polygons[0].lines[i].Start;
                bool Ear_point = false;
                Point Prev = polygons[0].lines[((i - 1) + size) % size].Start;
                Point next = polygons[0].lines[(i + 1) % size].Start;

                input_points_list.Add(new smart_point(p, Ear_point, Prev, next));
            }

            int current = 0;
            List<smart_point> List_of_Ear_points = new List<smart_point>();
            input_points_list[0].is_Ear = false;
            for (int i = 0; i < input_points_list.Count(); i++)
            {

                input_points_list[i].is_Ear = false;
                bool check = check_point_is_Ear(input_points_list[i], input_points_list);

                if (check == true)
                {
                    List_of_Ear_points.Add(input_points_list[i]);
                    List_of_Ear_points[current].is_Ear = true;
                     current++;

                }
            }

            while (List_of_Ear_points.Count() != 0)
            {
                if (input_points_list.Count() <= 3)  
                {
                    break;
                }             

                smart_point p = List_of_Ear_points[0];
                Line l1 = subtractEar(p, ref List_of_Ear_points);
                outLines.Add(l1);
            }

        } 

        public Line subtractEar(smart_point Ear, ref List<smart_point> E)
        {
            
            Line interor_line = new Line(Ear.Prev, Ear.Next);
            E.Remove(Ear);
            
            bool Status_of_prev_Before = false, Status_of_Next_Before = false;
            int ind_OF_Ear_in_polygon = 0; 

            for (int i = 0; i < input_points_list.Count; i++)
            {
                if (input_points_list[i].Current_Point == Ear.Prev)
                {
                    input_points_list[i].Next = Ear.Next;
                    Status_of_prev_Before = input_points_list[i].is_Ear;
                    input_points_list[i].is_Ear = check_point_is_Ear(input_points_list[i], input_points_list);
                    if (Status_of_prev_Before == false && input_points_list[i].is_Ear == true) 
                    {
                        E.Add(input_points_list[i]);
                    }

                    ind_OF_Ear_in_polygon = (i + 1) % input_points_list.Count; 
                    input_points_list.RemoveAt(ind_OF_Ear_in_polygon);
                }
                else if (input_points_list[i].Current_Point == Ear.Next)
                {

                    input_points_list[i].Prev = Ear.Prev;
                    Status_of_Next_Before = input_points_list[i].is_Ear;
                    input_points_list[i].is_Ear = check_point_is_Ear(input_points_list[i], input_points_list);
                    if (Status_of_Next_Before == false && input_points_list[i].is_Ear == true) 
                    {
                        E.Add(input_points_list[i]);
                    }

                }
            }           
            for (int i = 0; i < E.Count(); i++)
            {
                if (E[i].is_Ear == false)
                {
                    E.RemoveAt(i);
                }
            }

            return interor_line;
        } 
        bool check_point_is_Ear(smart_point p, List<smart_point> points)
        {
            Line l1 = new Line(p.Prev, p.Current_Point);
            bool point_is_Convex = false;
            bool points_inside_triangle = false;
            if (HelperMethods.CheckTurn(l1, p.Next) == Enums.TurnType.Left) 
            {
                point_is_Convex = true;

                for (int j = 0; j < points.Count(); j++)
                {
                    if (HelperMethods.PointInTriangle(points[j].Current_Point, p.Prev, p.Current_Point, p.Next) == Enums.PointInPolygon.Inside)
                    {
                        points_inside_triangle = true;

                        break;
                    }
                }
            }
            if (point_is_Convex == true && points_inside_triangle == false) 
            {
                return true;
                p.is_Ear = true;
            }
            else
                p.is_Ear = false;
            return false;
        }

        public static bool IS_points_Sorted_CCW(Point Min_X, int ind_OF_Min_X, List<Point> points)
        {
            Point prev = points[(ind_OF_Min_X - 1 + points.Count()) % points.Count()];
            Point next = points[(ind_OF_Min_X + 1) % points.Count()];

            Line l1 = new Line(prev, next);
            if (HelperMethods.CheckTurn(l1, Min_X) == Enums.TurnType.Right)
            {
                return true;
            }
            else
                return false; 
        }

        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}
