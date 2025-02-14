using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace towers_of_hanoi
{
    public class Scene3D
    {
        private Viewport3D viewport;

        public Scene3D(Viewport3D Viewport)
        {
            viewport = Viewport;
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 3);
            camera.LookDirection = new Vector3D(0, 0, -1);
            camera.FieldOfView = 60;
            viewport.Camera = camera;

            AmbientLight light = new AmbientLight();
            light.Color = Colors.White;

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = light;
            viewport.Children.Add(visual);
        }

        public void CreateDisc(float radius, float height, float innerRadius, int horizontalResolution, int verticalResolution)
        {
            MeshGeometry3D disc = new MeshGeometry3D();

            // create the inner curve
            float deltaRadians = 2.0f * MathF.PI / horizontalResolution;
            for (int vertex = 0; vertex < horizontalResolution; vertex++)
            {
                disc.Positions.Add(new Point3D(Math.Cos(deltaRadians * vertex) * innerRadius, Math.Sin(deltaRadians * vertex) * innerRadius, 0));
            }

            // wrap in a GeometryModel3D
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = disc;
            model.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);
        }
    }
}
