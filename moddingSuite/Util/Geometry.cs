using moddingSuite.Model.Common;
using moddingSuite.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace moddingSuite.Geometry
{
    public static class Geometry
    {
        public const int scaleFactor = 1280;
        public const double groundLevel = 32946.4921875;
        public static List<Point> getOutline(AreaContent content){
            
            var outline=content.Vertices.GetRange(content.BorderVertex.StartVertex,content.BorderVertex.VertexCount);
            return outline.ConvertAll(x => new Point((int)x.X / scaleFactor, (int)x.Y / scaleFactor));
        }
        public static AreaContent getFromOutline(List<AreaVertex> outline)
        {
            


            var content = new AreaContent();
            
            if (!isRightHanded(outline))
            {
                outline.Reverse();
            }
            outline=outline.Select(x => { x.Center = 1; return x; }).ToList();
            content.Vertices.AddRange(outline);

            //var bottomRing = new List<AreaVertex>(outline);
            var bottomRing = outline.Select(x => {
                var y = new AreaVertex();
                y.X = x.X;
                y.Y = x.Y;
                y.Z = x.Z;
                y.W = x.W;
                y.Center = 0; 
                return y; }).ToList();
            content.Vertices.AddRange(bottomRing);

            var innerRing = getInnerRing(outline);
            content.Vertices.AddRange(innerRing);

            var triangles = getTriangles(outline, 0, outline.Count - 1);
            content.Triangles.AddRange(triangles);
            var borderTriangles = getBorderTriangles(outline.Count);
            content.Triangles.AddRange(borderTriangles);

            AreaClipped ac = new AreaClipped();
            ac.StartTriangle = 0;
            ac.StartVertex = 0;
            ac.TriangleCount = triangles.Count;
            ac.VertexCount = outline.Count;
            //ac.TriangleCount = content.Triangles.Count;
            //ac.VertexCount = content.Vertices.Count;
            content.ClippedAreas.Add(ac);
            AreaClipped ac2 = new AreaClipped();
            content.ClippedAreas.Add(ac2);

            AreaClipped bt = new AreaClipped();
            bt.StartTriangle = triangles.Count;
            bt.StartVertex = outline.Count;
            bt.TriangleCount = borderTriangles.Count;
            bt.VertexCount = 2 * outline.Count;
            content.BorderTriangle = bt;

            AreaClipped bv = new AreaClipped();
            bv.StartTriangle = 0;
            bv.StartVertex = 2*outline.Count;
            bv.TriangleCount = 0;
            bv.VertexCount = outline.Count;
            content.BorderVertex = bv;
            
            //Console.Write("vertices=[");
            //var scen = content;
            /*foreach (var v in scen.Vertices)
            {
                Console.WriteLine("{0:G},{1:G},{2:G};", (int)v.X, (int)v.Y, (int)v.Center);
            }
            Console.WriteLine("]");

            Console.Write("tri=[");
            foreach (var v in scen.Triangles)
            {
                Console.WriteLine("{0},{1},{2};", (int)v.Point1, (int)v.Point2, (int)v.Point3);
            }
            Console.WriteLine("]");


            Console.WriteLine("bt=[{0:G},{1:G},{2:G},{3:G}]", scen.BorderTriangle.StartTriangle, scen.BorderTriangle.StartVertex, scen.BorderTriangle.TriangleCount, scen.BorderTriangle.VertexCount);
            Console.WriteLine("bv=[{0:G},{1:G},{2:G},{3:G}]", scen.BorderVertex.StartTriangle, scen.BorderVertex.StartVertex, scen.BorderVertex.TriangleCount, scen.BorderVertex.VertexCount);
            var k = 0;
            foreach (var a in scen.ClippedAreas)
            {
                Console.WriteLine("t{4}=[{0:G},{1:G},{2:G},{3:G}]", a.StartTriangle, a.StartVertex, a.TriangleCount, a.VertexCount, k++);
            }
            */
            return content;

        }
        public static Point convertPoint(Point3D p)
        {
            return new Point((int)p.X / scaleFactor, (int)p.Y / scaleFactor);

        }
        public static Point3D convertPoint(Point p)
        {
            return new Point3D(p.X * scaleFactor, p.Y * scaleFactor,groundLevel);

        }
        public static bool isInside(AreaVertex v1, List<AreaVertex> outline)
        {
            var v2 = new AreaVertex();

            var crossings = 0;
            for (int i = 0; i < outline.Count; i++)
            {
                var b1 = outline.ElementAt(i);
                var k = i + 1;
                if (i == outline.Count - 1)
                {
                    k = 0;
                }
                var b2 = outline.ElementAt(k);
                if (v1.Equals(b1) || v1.Equals(b2) || v2.Equals(b1) || v2.Equals(b2)) continue;
                if (isRightHanded(new List<AreaVertex>() { v1, b1, b2 }) == isRightHanded(new List<AreaVertex>() { v2, b1, b2 }))
                {
                    continue;
                }
                else if (isRightHanded(new List<AreaVertex>() { v1, v2, b1 }) == isRightHanded(new List<AreaVertex>() { v1, v2, b2 }))
                {
                    continue;
                }
                else
                {
                    crossings++;
                }
            }
            return crossings % 2 == 1;
        }
        private static List<AreaVertex> getInnerRing(List<AreaVertex> outline)
        {
            var innerRing = outline.Select(x =>
            {
                var y = new AreaVertex();
                y.X = x.X;
                y.Y = x.Y;
                y.Z = x.Z;
                y.W = x.W;
                y.Center = x.Center;
                return y;
            }).ToList();
            for (int i = 0; i < outline.Count; i++)
            {
                var im = i - 1;
                if (im < 0)
                {
                    im = outline.Count - 1;
                }
                var ip = i + 1;
                if (ip == outline.Count)
                {
                    ip = 0;
                }
                var localAngle = (getAngle(outline.ElementAt(im), outline.ElementAt(i), outline.ElementAt(ip)) + Math.PI) / 2; 
                var angle=localAngle+ getAbsoluteAngle(outline.ElementAt(im), outline.ElementAt(i));
                float offset = 5 * scaleFactor / (float)Math.Abs(Math.Sin(localAngle));

                var tmp = innerRing[i];
                tmp.X += offset * (float)Math.Cos(angle);
                tmp.Y += offset * (float)Math.Sin(angle);
                innerRing[i] = tmp;
            }
            return innerRing;
        }
        private static List<MeshTriangularFace> getBorderTriangles(int count)
        {
            var lower = count;
            var forwardLower = count+1;
            var upper = 2*count;
            var forwardUpper = 2 * count + 1;
            var triangles = new List<MeshTriangularFace>();
            for (var i = 0; i < count-1; i++)
            {
                /*
                var triangle1 = new MeshTriangularFace(lower + i, forwardLower + i, upper + i);
                var triangle2 = new MeshTriangularFace(upper + i, forwardLower + i, forwardUpper + i);
              */

                var triangle1 = new MeshTriangularFace(lower + i, upper + i, forwardLower + i);
                var triangle2 = new MeshTriangularFace(upper + i, forwardUpper + i, forwardLower + i);
                triangles.Add(triangle1);
                triangles.Add(triangle2);
            }
            var triangle3 = new MeshTriangularFace(2*count-1, lower, 3*count-1);
            var triangle4 = new MeshTriangularFace(3 * count - 1, lower, upper);
            /*
            var triangle3 = new MeshTriangularFace(2 * count - 1, 3 * count - 1, lower);
            var triangle4 = new MeshTriangularFace(3 * count - 1, upper, lower);*/
            triangles.Add(triangle3);
            triangles.Add(triangle4);
            return triangles;

        }
        private static List<MeshTriangularFace> getTriangles(List<AreaVertex> outline, int start, int stop)
        {
            List<MeshTriangularFace> triangles = new List<MeshTriangularFace>();
            var i = start;
            var firstNode = outline.ElementAt(start);
            var secondIndex = ++i;
            
            while(i<stop){
                var thirdIndex = ++i;

                var secondNode = outline.ElementAt(secondIndex);
                var thirdNode=outline.ElementAt(thirdIndex);
                //var t=thirdIndex;
                //bool allRightHanded = true;
                /*while (t < stop)
                {
                    var tNode = outline.ElementAt(t++);

                    if (!isRightHanded(new List<AreaVertex>() { firstNode, secondNode, tNode }))
                    {
                        allRightHanded = false;
                        break;
                    }
                }
                if (!allRightHanded) continue;*/
                if (!isRightHanded(new List<AreaVertex>() { firstNode, secondNode, thirdNode }) ||
                    lineIntersect(firstNode, thirdNode, outline) || 
                    lineIntersect(secondNode, thirdNode, outline))
                {
                    continue;
                }
                MeshTriangularFace triangle;
                triangle = new MeshTriangularFace(start, secondIndex, thirdIndex);
                triangles.AddRange(getTriangles(outline, secondIndex, thirdIndex));
                secondIndex = thirdIndex;
                triangles.Add(triangle);
                
            }
            return triangles;

        }
        private static bool lineIntersect(AreaVertex v1, AreaVertex v2, List<AreaVertex> outline)
        {
            bool intersects = false;
            for (int i = 0; i < outline.Count; i++)
            {
                var b1 = outline.ElementAt(i);
                var k=i+1;
                if (i == outline.Count - 1)
                {
                     k= 0;
                }
                var b2 = outline.ElementAt(k);
                if (v1.Equals(b1) || v1.Equals(b2) || v2.Equals(b1) || v2.Equals(b2)) continue;
                if (isRightHanded(new List<AreaVertex>() { v1, b1, b2 }) == isRightHanded(new List<AreaVertex>() { v2, b1, b2 }))
                {
                    continue;
                }
                else if (isRightHanded(new List<AreaVertex>() { v1, v2, b1 }) == isRightHanded(new List<AreaVertex>() { v1, v2, b2 }))
                {
                    continue;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        private static bool isRightHanded(List<AreaVertex> polygon)
        {
            double angleSum = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                var im = i - 1;
                if (im < 0)
                {
                    im = polygon.Count - 1;
                }
                var ip = i + 1;
                if (ip == polygon.Count)
                {
                    ip = 0;
                }
                angleSum += getAngle(polygon.ElementAt(im), polygon.ElementAt(i), polygon.ElementAt(ip));
            }
            //Console.WriteLine(angleSum);
            return angleSum > 0;
        }
        private static double getAbsoluteAngle(AreaVertex v1, AreaVertex v2)
        {
            var angle = Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
            return angle;
        }
        private static double getAngle(AreaVertex v0, AreaVertex v1, AreaVertex v2)
        {
            var angle1 = getAbsoluteAngle(v0, v1);
            var angle2 = getAbsoluteAngle(v1, v2);
            var deltaAngle=angle2-angle1;
            if(deltaAngle< -Math.PI){
                deltaAngle+=2*Math.PI;
            }
            if(deltaAngle> Math.PI){
                deltaAngle-=2*Math.PI;
            }
            return deltaAngle;
        }
    }
}
