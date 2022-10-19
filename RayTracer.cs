using System;
using System.Runtime.InteropServices;

namespace rt
{
    class RayTracer
    {
        private Geometry[] geometries;
        private Light[] lights;

        public RayTracer(Geometry[] geometries, Light[] lights)
        {
            this.geometries = geometries;
            this.lights = lights;
        }

        private double ImageToViewPlane(int n, int imgSize, double viewPlaneSize)
        {
            var u = n * viewPlaneSize / imgSize;
            u -= viewPlaneSize / 2;
            return u;
        }

        private Intersection FindFirstIntersection(Line ray, double minDist, double maxDist)
        {
            var intersection = new Intersection();

            foreach (var geometry in geometries)
            {
                var intr = geometry.GetIntersection(ray, minDist, maxDist);

                if (!intr.Valid || !intr.Visible) continue;

                if (!intersection.Valid || !intersection.Visible)
                {
                    intersection = intr;
                }
                else if (intr.T < intersection.T)
                {
                    intersection = intr;
                }
            }

            return intersection;
        }

        private bool IsLit(Vector point, Light light)
        {
            // ADD CODE HERE: Detect whether the given point has a clear line of sight to the given light
            Line line=new Line(point,light.Position);
            //dist de la pozitia punctului la lumina
            double dist = (light.Position - point).Length();
            //mai mic decat 0 iasa cu pureci
            Intersection ins = FindFirstIntersection(line, 0.05, dist);
            return !ins.Visible;
        }

        public void Render(Camera camera, int width, int height, string filename)
        {
            var background = new Color();
            var viewParallel = (camera.Up ^ camera.Direction).Normalize();

            var image = new Image(width, height);

            var vecW = camera.Direction * camera.ViewPlaneDistance;
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    // ADD CODE HERE: Implement pixel color calculation
                    //Intersection
                    Vector X1 = camera.Position + vecW +
                                viewParallel * ImageToViewPlane(i, width, camera.ViewPlaneWidth) +
                                camera.Up * ImageToViewPlane(j, height, camera.ViewPlaneHeight);
                    Line line = new Line(camera.Position, X1);
                    Intersection inters = FindFirstIntersection(line, camera.FrontPlaneDistance, camera.BackPlaneDistance);

                    Color mc = background;
                    var culoare = new Color();
                    if (inters.Valid && inters.Visible)
                    {
                        
                        foreach (var light in lights)
                        {
                            //AMbient
                            culoare +=inters.Geometry.Material.Ambient* light.Ambient;
                            if (IsLit(inters.Position, light))
                            {
                                
                                //Difuse
                                var n = inters.Geometry.Normal(inters.Position);//normal to the surf. at the int. point
                                var t =  (light.Position-inters.Position ).Normalize();//vector from the int. point to the light

                                if (n * t > 0)
                                    culoare += inters.Geometry.Material.Diffuse * light.Diffuse * (n * t);
                                //Specular
                                var e = (camera.Position-inters.Position).Normalize();//vect from int. point to the camera
                                var r = (n * (n * t) * 2 - t);//reflection vector
                                if (e * r > 0)
                                    culoare += inters.Geometry.Material.Specular * light.Specular * Math.Pow(e * r,inters.Geometry.Material.Shininess);
                            }
                            culoare *= light.Intensity;

                            mc = culoare;
                        }
                        image.SetPixel(i, j, mc);
                    }
                }
            }

            image.Store(filename);
        }
    }
}