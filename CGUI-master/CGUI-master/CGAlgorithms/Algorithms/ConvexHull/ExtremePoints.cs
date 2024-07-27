using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
           
            for (int i = 0; i < points.Count; i++)
            {
                bool deleted = false;
                for (int j = 0; j < points.Count; j++)
                {

                    for (int l = 0; l < points.Count; l++)
                    {
                        for (int k = 0; k < points.Count; k++)
                        {
                            if (j != i && k != i && l != i)
                            {
                                Enums.PointInPolygon o = HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]);
                                if (o.Equals(Enums.PointInPolygon.Inside) || o.Equals(Enums.PointInPolygon.OnEdge))
                                {
                                    points.RemoveAt(i);
                                    deleted = true;
                                    break;
                                }
                            }
                        }
                        if (deleted)
                            break;
                    }
                    if (deleted)
                        break;
                }
                if (deleted)
                    i--;

            }
            outPoints = points;

        }
        



        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
