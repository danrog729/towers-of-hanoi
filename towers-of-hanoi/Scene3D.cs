using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
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

        private Point3D _cameraTarget;
        public Point3D CameraTarget
        {
            get => _cameraTarget;
            set
            {
                _cameraTarget = value;
                PerspectiveCamera? camera = viewport.Camera as PerspectiveCamera;
                if (camera != null)
                {
                    SetCameraPos((float)camera.Position.X, (float)camera.Position.Y, (float)camera.Position.Z);
                }
            }
        }

        public List<ModelVisual3D> discList;
        public List<ModelVisual3D> hitBoxes;

        private int draggingFrom;

        public Scene3D(Viewport3D Viewport)
        {
            viewport = Viewport;
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 20, 20);
            camera.LookDirection = new Vector3D(0, -20, -20);
            camera.FieldOfView = 60;
            viewport.Camera = camera;
            CameraTarget = new Point3D(0, 0, 0);
            discList = new List<ModelVisual3D>();
            hitBoxes = new List<ModelVisual3D>();
        }

        public void SetCameraPos(float x, float y, float z)
        {
            PerspectiveCamera? camera = viewport.Camera as PerspectiveCamera;
            if (camera != null)
            {
                camera.Position = new Point3D(x, y, z);
                camera.LookDirection = new Vector3D(CameraTarget.X - x, CameraTarget.Y - y, CameraTarget.Z - z);
            }
        }

        public void RotateCamera(float yaw, float pitch)
        {
            PerspectiveCamera? camera = viewport.Camera as PerspectiveCamera;
            if (camera != null)
            {
                float x = (float)camera.Position.X;
                float y = (float)camera.Position.Y;
                float z = (float)camera.Position.Z;

                // change yaw
                float currentYaw = MathF.Atan2(z, x);
                float newYaw = currentYaw + yaw;
                float yawHypotenuse = MathF.Sqrt(x * x + z * z);
                x = yawHypotenuse * MathF.Cos(newYaw);
                z = yawHypotenuse * MathF.Sin(newYaw);

                // change pitch
                float radius = MathF.Sqrt(x * x + z * z);
                float currentPitch = MathF.Atan2(y, radius);
                float newPitch = currentPitch + pitch;
                if (newPitch > 1.5f)
                {
                    newPitch = 1.5f;
                }
                else if (newPitch < -1.5f)
                {
                    newPitch = -1.5f;
                }
                float pitchHypotenuse = MathF.Sqrt(y * y + radius * radius);
                float newRadius = pitchHypotenuse * MathF.Cos(newPitch);
                float newHeight = pitchHypotenuse * MathF.Sin(newPitch);

                x = newRadius * MathF.Cos(newYaw);
                y = newHeight;
                z = newRadius * MathF.Sin(newYaw);

                SetCameraPos(x, y, z);
            }
        }

        public void ZoomCamera(float amount)
        {
            PerspectiveCamera? camera = viewport.Camera as PerspectiveCamera;
            if (camera != null)
            {
                float x = (float)(camera.Position.X - camera.Position.X * amount);
                float y = (float)(camera.Position.Y - camera.Position.Y * amount);
                float z = (float)(camera.Position.Z - camera.Position.Z * amount);
                if (x < 1000 && y < 1000 && z < 1000 || amount > 0)
                {
                    SetCameraPos(x, y, z);
                }
            }
        }

        public void SelectObjectForDragAndDrop(System.Windows.Point clickPosition)
        {
            HitTestResult result = VisualTreeHelper.HitTest(viewport, clickPosition);

            if (result is RayMeshGeometry3DHitTestResult meshResult)
            {
                GeometryModel3D? hitModel = meshResult.ModelHit as GeometryModel3D;
                if (hitModel != null && hitBoxes.Contains(meshResult.VisualHit))
                {
                    draggingFrom = hitBoxes.IndexOf((ModelVisual3D)meshResult.VisualHit);
                }
            }
        }

        public (int,int) ReleaseDragAndDrop(System.Windows.Point clickPosition)
        {
            HitTestResult result = VisualTreeHelper.HitTest(viewport, clickPosition);
            int draggingTo = 0;
            bool valid = false;

            if (result is RayMeshGeometry3DHitTestResult meshResult)
            {
                GeometryModel3D? hitModel = meshResult.ModelHit as GeometryModel3D;
                if (hitModel != null && hitBoxes.Contains(meshResult.VisualHit))
                {
                    draggingTo = hitBoxes.IndexOf((ModelVisual3D)meshResult.VisualHit);
                    valid = true;
                }
            }
            if (valid)
            {
                // return the movement
                return (draggingFrom, draggingTo);
            }
            else
            {
                // return a movement onto the same tower
                return (draggingFrom, draggingFrom);
            }
        }

        private ModelVisual3D CreateDisc(float radius, float height, float innerRadius, int majorSegments, int minorSegments, System.Windows.Media.Color colour)
        {
            MeshGeometry3D disc = new MeshGeometry3D();

            // add the vertices
            float majorDeltaTheta = 2 * MathF.PI / majorSegments;
            float minorDeltaTheta = MathF.PI / minorSegments;
            for (int crossSection = 0; crossSection < majorSegments; crossSection++)
            {
                // create one cross-section
                // inner ring
                for (int vertex = 0; vertex <= minorSegments; vertex++)
                {
                    double horizontal = innerRadius + height / 2 - Math.Sin(minorDeltaTheta * vertex) * height / 2;
                    double vertical = -Math.Cos(minorDeltaTheta * vertex) * height / 2;
                    disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection), 
                        vertical, 
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
                }
                // outer ring
                for (int vertex = 0; vertex <= minorSegments; vertex++)
                {
                    double horizontal = radius - height / 2 + Math.Sin(minorDeltaTheta * vertex) * height / 2;
                    double vertical = Math.Cos(minorDeltaTheta * vertex) * height / 2;
                    disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection), 
                        vertical, 
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
                }
            }

            // join the vertices into faces
            int crossSectionResolution = minorSegments * 2 + 2;
            for (int vertex = 0; vertex < disc.Positions.Count; vertex++)
            {
                // triangulate with the previous ring's analog and the one before it
                int analog = vertex - crossSectionResolution;
                if (analog < 0)
                {
                    analog += disc.Positions.Count;
                }
                int previous = analog - 1;
                int previousIsNegative = 0;
                if (previous < 0)
                {
                    previousIsNegative = 1;
                }
                if (previous - (previous % crossSectionResolution + previousIsNegative) != analog - analog % crossSectionResolution)
                {
                    previous += crossSectionResolution;
                }
                disc.TriangleIndices.Add(analog);
                disc.TriangleIndices.Add(previous);
                disc.TriangleIndices.Add(vertex);
                // triangulate with the previous ring's analog and the one after the current
                int next = vertex + 1;
                if (next - next % crossSectionResolution != vertex - vertex % crossSectionResolution)
                {
                    next -= crossSectionResolution;
                }
                disc.TriangleIndices.Add(analog);
                disc.TriangleIndices.Add(vertex);
                disc.TriangleIndices.Add(next);
            }

            // wrap in a GeometryModel3D
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = disc;
            model.Material = new DiffuseMaterial(new SolidColorBrush(colour));

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);

            discList.Add(visual);
            return visual;
        }

        private ModelVisual3D CreatePole(float width, float height, float baseHeight, float radius, float innerRadius, int segments, System.Windows.Media.Color colour)
        {
            MeshGeometry3D pole = new MeshGeometry3D();

            // add the vertices
            float majorDeltaTheta = 2 * MathF.PI / segments;
            for (int crossSection = 0; crossSection < segments; crossSection++)
            {
                // create one cross-section
                double horizontal = innerRadius;
                double  vertical = height;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = radius;
                vertical = height;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = radius;
                vertical = baseHeight;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = width;
                vertical = baseHeight;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = width;
                vertical = 0;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = innerRadius;
                vertical = 0;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));
            }

            // join the vertices into faces
            int crossSectionResolution = 6;
            for (int vertex = 0; vertex < pole.Positions.Count; vertex++)
            {
                // triangulate with the previous ring's analog and the one before it
                int analog = vertex - crossSectionResolution;
                if (analog < 0)
                {
                    analog += pole.Positions.Count;
                }
                int previous = analog - 1;
                int previousIsNegative = 0;
                if (previous < 0)
                {
                    previousIsNegative = 1;
                }
                if (previous - (previous % crossSectionResolution + previousIsNegative) != analog - analog % crossSectionResolution)
                {
                    previous += crossSectionResolution;
                }
                pole.TriangleIndices.Add(analog);
                pole.TriangleIndices.Add(previous);
                pole.TriangleIndices.Add(vertex);
                // triangulate with the previous ring's analog and the one after the current
                int next = vertex + 1;
                if (next - next % crossSectionResolution != vertex - vertex % crossSectionResolution)
                {
                    next -= crossSectionResolution;
                }
                pole.TriangleIndices.Add(analog);
                pole.TriangleIndices.Add(vertex);
                pole.TriangleIndices.Add(next);
            }

            // wrap in a GeometryModel3D
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = pole;
            model.Material = new DiffuseMaterial(new SolidColorBrush(colour));

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);

            return visual;
        }

        private ModelVisual3D CreateHitbox(float width, float height, float depth)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-width / 2, 0, -depth / 2));
            mesh.Positions.Add(new Point3D(-width / 2, 0, depth / 2));
            mesh.Positions.Add(new Point3D(width / 2, 0, -depth / 2));
            mesh.Positions.Add(new Point3D(width / 2, 0, depth / 2));
            mesh.Positions.Add(new Point3D(-width / 2, height, -depth / 2));
            mesh.Positions.Add(new Point3D(-width / 2, height, depth / 2));
            mesh.Positions.Add(new Point3D(width / 2, height, -depth / 2));
            mesh.Positions.Add(new Point3D(width / 2, height, depth / 2));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(6);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(5);

            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // wrap in a GeometryModel3D
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = mesh;
            model.Material = new EmissiveMaterial(new SolidColorBrush(Colors.Black));

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);

            hitBoxes.Add(visual);

            return visual;
        }

        private ModelVisual3D CreateBoundingBox(float bigNumber)
        {
            // add the bounding cube
            MeshGeometry3D boundingCube = new MeshGeometry3D();

            boundingCube.Positions.Add(new Point3D(-bigNumber, -bigNumber, -bigNumber));
            boundingCube.Positions.Add(new Point3D(-bigNumber, -bigNumber, bigNumber));
            boundingCube.Positions.Add(new Point3D(-bigNumber, bigNumber, -bigNumber));
            boundingCube.Positions.Add(new Point3D(-bigNumber, bigNumber, bigNumber));
            boundingCube.Positions.Add(new Point3D(bigNumber, -bigNumber, -bigNumber));
            boundingCube.Positions.Add(new Point3D(bigNumber, -bigNumber, bigNumber));
            boundingCube.Positions.Add(new Point3D(bigNumber, bigNumber, -bigNumber));
            boundingCube.Positions.Add(new Point3D(bigNumber, bigNumber, bigNumber));

            boundingCube.TriangleIndices.Add(0);
            boundingCube.TriangleIndices.Add(2);
            boundingCube.TriangleIndices.Add(1);
            boundingCube.TriangleIndices.Add(1);
            boundingCube.TriangleIndices.Add(2);
            boundingCube.TriangleIndices.Add(3);

            boundingCube.TriangleIndices.Add(0);
            boundingCube.TriangleIndices.Add(4);
            boundingCube.TriangleIndices.Add(2);
            boundingCube.TriangleIndices.Add(2);
            boundingCube.TriangleIndices.Add(4);
            boundingCube.TriangleIndices.Add(6);

            boundingCube.TriangleIndices.Add(0);
            boundingCube.TriangleIndices.Add(1);
            boundingCube.TriangleIndices.Add(4);
            boundingCube.TriangleIndices.Add(1);
            boundingCube.TriangleIndices.Add(5);
            boundingCube.TriangleIndices.Add(4);

            boundingCube.TriangleIndices.Add(1);
            boundingCube.TriangleIndices.Add(3);
            boundingCube.TriangleIndices.Add(5);
            boundingCube.TriangleIndices.Add(3);
            boundingCube.TriangleIndices.Add(7);
            boundingCube.TriangleIndices.Add(5);

            boundingCube.TriangleIndices.Add(4);
            boundingCube.TriangleIndices.Add(5);
            boundingCube.TriangleIndices.Add(6);
            boundingCube.TriangleIndices.Add(5);
            boundingCube.TriangleIndices.Add(7);
            boundingCube.TriangleIndices.Add(6);

            boundingCube.TriangleIndices.Add(2);
            boundingCube.TriangleIndices.Add(6);
            boundingCube.TriangleIndices.Add(3);
            boundingCube.TriangleIndices.Add(3);
            boundingCube.TriangleIndices.Add(6);
            boundingCube.TriangleIndices.Add(7);

            // wrap in a GeometryModel3D
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = boundingCube;
            model.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Transparent));

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);

            return visual;
        }

        public void Reset(int discCount, int poleCount, int startingPole, float discHeight)
        {
            viewport.Children.Clear();
            discList.Clear();
            hitBoxes.Clear();

            // setup the lighting
            DirectionalLight light = new DirectionalLight();
            light.Color = Colors.White;
            light.Direction = new Vector3D(-1.0, -1.0, -1.0);

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = light;
            viewport.Children.Add(visual);

            // add the bounding cube
            CreateBoundingBox(1000);

            System.Windows.Media.Color[] colours = { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Purple };
            _cameraTarget.Y = (1 - Math.Sqrt(2) / 2) * discHeight * discCount;
            for (int poleNumber = 0; poleNumber < poleCount; poleNumber++)
            {
                float radius = discCount + discHeight + 1;
                float horizontalMargin = radius * 0.5f;
                float verticalMargin = radius * 0.5f;
                ModelVisual3D pole = CreatePole(radius, discCount * discHeight + discHeight, discHeight, 1, 0.5f, (int)(radius * 8), Colors.Blue);
                ModelVisual3D hitBox = CreateHitbox(radius * 2 + horizontalMargin, discCount * discHeight + discHeight + verticalMargin, radius * 2 + horizontalMargin);
                Transform3DGroup transform = new Transform3DGroup();
                transform.Children.Add(new TranslateTransform3D()
                {
                    OffsetX = (-(float)(poleCount - 1) / 2 + poleNumber) * radius * 2.5,
                    OffsetY = -discHeight * 1.5
                });
                pole.Transform = transform;
                hitBox.Transform = transform;
            }
            double startingPoleX = (-(float)(poleCount - 1) / 2 + startingPole) * (discCount + discHeight + 1) * 2.5;
            for (int discNumber = 0; discNumber < discCount; discNumber++)
            {
                float radius = discCount - discNumber + discHeight + 1;
                ModelVisual3D disc = CreateDisc(radius, 2, 1, (int)(radius * 8), 8, colours[discNumber % colours.Length]);
                Transform3DGroup transform = new Transform3DGroup();
                transform.Children.Add(new TranslateTransform3D()
                {
                    OffsetX = startingPoleX,
                    OffsetY = discNumber * discHeight
                });
                disc.Transform = transform;
            }
            PerspectiveCamera? camera = viewport.Camera as PerspectiveCamera;
            if (camera != null)
            {
                SetCameraPos((float)camera.Position.X, (float)camera.Position.Y, (float)camera.Position.Z);
            }
        }
    }
}
