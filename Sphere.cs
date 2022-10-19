using System;

namespace rt
{
    public class Sphere : Geometry
    {
        private Vector Center { get; set; }
        private double Radius { get; set; }

        public Sphere(Vector center, double radius, Material material, Color color) : base(material, color)
        {
            Center = center;
            Radius = radius;
        }

        public override Intersection GetIntersection(Line line, double minDist, double maxDist)
        {
            Vector dx = line.Dx;
            Vector ct = this.Center;
            Vector x0 = line.X0;
            Vector b = x0 - ct;
            double delta=(dx*b)*(dx*b)-b.Length2()+Radius*Radius;
            if (delta < 0)
            {
                return new Intersection();
            }
            else
            {
                double t1 = (-1 * (dx * b) - Math.Sqrt(delta));
                double t2 = (-1 * (dx * b) + Math.Sqrt(delta));
                if (t1 > t2)
                {
                    if (t2 < maxDist && t2 > minDist) 
                    {
                        return new Intersection(true, true, this, line, t2);
                    } 
                }
                else
                {
                    if (t1 > minDist && t1 < maxDist)
                    {
                        return new Intersection(true, true, this, line, t1);
                    }
                }
            }
            return new Intersection();
        }

        public override Vector Normal(Vector v)
        {
            var n = v - Center;
            n.Normalize();
            return n;
        }
    }
}