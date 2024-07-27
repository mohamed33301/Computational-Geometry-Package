using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points = points.OrderBy(point => point.X).ThenBy(point => point.Y).ToList();
            outPoints = Divide(points);
        }

        public List<Point> Divide(List<Point> div_p)
        {
            if (div_p.Count <= 1)
            {
                return div_p;
            }

            List<Point> left_p = GetLeftHalf(div_p);
            List<Point> right_p = GetRightHalf(div_p);

            List<Point> convexHull = Combine(Divide(left_p), Divide(right_p));

            return convexHull;
        }

        public List<Point> GetLeftHalf(List<Point> points)
        {
            List<Point> left_p = new List<Point>();
            int half_number_of_points = points.Count / 2;

            for (int i = 0; i < half_number_of_points; i++)
            {
                left_p.Add(points[i]);
            }

            return left_p;
        }

        public List<Point> GetRightHalf(List<Point> points)
        {
            List<Point> right_p = new List<Point>();
            int half_number_of_points = points.Count / 2;

            for (int i = half_number_of_points; i < points.Count; i++)
            {
                right_p.Add(points[i]);
            }

            return right_p;
        }
        public List<Point> Combine(List<Point> left_p, List<Point> right_p)
        {

            int index_of_most_left = 0, index_of_most_right = 0, Lcount = left_p.Count, Rcount = right_p.Count;

            for (int i = 1; i < Lcount; i++)
            {
                if (left_p[i].X > left_p[index_of_most_right].X ||
                    left_p[i].X == left_p[index_of_most_right].X && left_p[i].Y > left_p[index_of_most_right].Y)

                    index_of_most_right = i;
            }

            for (int i = 1; i < Rcount; i++)
            {
                if (right_p[i].X < right_p[index_of_most_left].X ||
                    right_p[i].X == right_p[index_of_most_left].X && right_p[i].Y < right_p[index_of_most_left].Y)

                    index_of_most_left = i;
            }

    
            int change_point_A_up = index_of_most_right;
            int change_point_B_up = index_of_most_left;
            bool found = false;

            while (!found)
            {
                found = true;

                while (CGUtilities.HelperMethods.CheckTurn(new Line(right_p[change_point_B_up].X, right_p[change_point_B_up].Y, left_p[change_point_A_up].X, left_p[change_point_A_up].Y),
                          left_p[(change_point_A_up + 1) % Lcount]) == Enums.TurnType.Right)
                {
                    change_point_A_up = (change_point_A_up + 1) % Lcount;
                    found = false;
                }
          
                if ((CGUtilities.HelperMethods.CheckTurn(new Line(right_p[change_point_B_up].X, right_p[change_point_B_up].Y, left_p[change_point_A_up].X, left_p[change_point_A_up].Y),
                             left_p[(change_point_A_up + 1) % Lcount]) == Enums.TurnType.Colinear))
                {
                    change_point_A_up = (change_point_A_up + 1) % Lcount;
                }

                while (CGUtilities.HelperMethods.CheckTurn(new Line(left_p[change_point_A_up].X, left_p[change_point_A_up].Y, right_p[change_point_B_up].X, right_p[change_point_B_up].Y),
                    right_p[(Rcount + change_point_B_up - 1) % Rcount]) == Enums.TurnType.Left)
                {
                    change_point_B_up = (Rcount + change_point_B_up - 1) % Rcount;
                    found = false;

                }

                if ((CGUtilities.HelperMethods.CheckTurn(new Line(left_p[change_point_A_up].X, left_p[change_point_A_up].Y, right_p[change_point_B_up].X, right_p[change_point_B_up].Y),
                    right_p[(change_point_B_up - 1 + Rcount) % Rcount]) == Enums.TurnType.Colinear))
                {
                    change_point_B_up = (change_point_B_up - 1 + Rcount) % Rcount;
                }
            }

        
            int change_point_A_low = index_of_most_right;
            int change_point_B_low = index_of_most_left;
            found = false;

   
            while (!found)
            {
                found = true;
                while (CGUtilities.HelperMethods.CheckTurn(new Line(right_p[change_point_B_low].X, right_p[change_point_B_low].Y, left_p[change_point_A_low].X, left_p[change_point_A_low].Y)
                    , left_p[(change_point_A_low + Lcount - 1) % Lcount]) == Enums.TurnType.Left)
                {
                    change_point_A_low = (change_point_A_low + Lcount - 1) % Lcount;
                    found = false;
                }

                if ((CGUtilities.HelperMethods.CheckTurn(new Line(right_p[change_point_B_low].X, right_p[change_point_B_low].Y, left_p[change_point_A_low].X, left_p[change_point_A_low].Y),
                                left_p[(change_point_A_low + Lcount - 1) % Lcount]) == Enums.TurnType.Colinear))
                {
                    change_point_A_low = (change_point_A_low + Lcount - 1) % Lcount;
                }

                while (CGUtilities.HelperMethods.CheckTurn(new Line(left_p[change_point_A_low].X, left_p[change_point_A_low].Y, right_p[change_point_B_low].X, right_p[change_point_B_low].Y),
                    right_p[(change_point_B_low + 1) % Rcount]) == Enums.TurnType.Right)
                {
                    change_point_B_low = (change_point_B_low + 1) % Rcount;
                    found = false;

                }

                if ((CGUtilities.HelperMethods.CheckTurn(new Line(left_p[change_point_A_low].X, left_p[change_point_A_low].Y, right_p[change_point_B_low].X, right_p[change_point_B_low].Y),
                    right_p[(change_point_B_low + 1) % Rcount]) == Enums.TurnType.Colinear))
                {
                    change_point_B_low = (change_point_B_low + 1) % Rcount;
                }
            }

            List<Point> convexHull = new List<Point>();

       

            convexHull.Add(left_p[change_point_A_up]);

            while (change_point_A_up != change_point_A_low)
            {
                change_point_A_up = (change_point_A_up + 1) % Lcount;


                if (!convexHull.Contains(left_p[change_point_A_up]))

                    convexHull.Add(left_p[change_point_A_up]);

            }


            convexHull.Add(right_p[change_point_B_low]);

            while (change_point_B_low != change_point_B_up)
            {
                change_point_B_low = (change_point_B_low + 1) % Rcount;

                if (!convexHull.Contains(right_p[change_point_B_low]))

                    convexHull.Add(right_p[change_point_B_low]);

            }
            return convexHull;
        
    }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
