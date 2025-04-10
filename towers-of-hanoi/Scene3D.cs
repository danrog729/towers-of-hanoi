﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace towers_of_hanoi
{
    public class Scene3D
    {
        public static float hoverTime = 0.1f;
        public static float dropTime = 0.3f;

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
        public List<ModelVisual3D> poleList;
        public List<ModelVisual3D> hitBoxes;

        private float discHeight;
        private int poleCount;
        private float poleRadius;

        private int draggingFrom;
        public int DraggingFrom
        {
            get => draggingFrom;
            set
            {
                return;
            }
        }

        public int DraggingLastOver;

        private bool _validDragDrop;
        public bool ValidDragDrop
        {
            get => _validDragDrop;
            set
            {
                return;
            }
        }

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
            poleList = new List<ModelVisual3D>();
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
            _validDragDrop = false;

            if (result is RayMeshGeometry3DHitTestResult meshResult)
            {
                GeometryModel3D? hitModel = meshResult.ModelHit as GeometryModel3D;
                if (hitModel != null && hitBoxes.Contains(meshResult.VisualHit))
                {
                    draggingFrom = hitBoxes.IndexOf((ModelVisual3D)meshResult.VisualHit);
                    _validDragDrop = true;
                }
            }
        }

        public void SelectDirectMove(int poleNumber)
        {
            _validDragDrop = true;
            draggingFrom = poleNumber;
        }

        public int MoveDragAndDrop(System.Windows.Point clickPosition)
        {
            HitTestResult result = VisualTreeHelper.HitTest(viewport, clickPosition);

            if (result is RayMeshGeometry3DHitTestResult meshResult)
            {
                GeometryModel3D? hitModel = meshResult.ModelHit as GeometryModel3D;
                if (hitModel != null && hitBoxes.Contains(meshResult.VisualHit))
                {
                    return hitBoxes.IndexOf((ModelVisual3D)meshResult.VisualHit);
                }
            }
            return -1;
        }

        public (int,int) ReleaseDragAndDrop(System.Windows.Point clickPosition)
        {
            HitTestResult result = VisualTreeHelper.HitTest(viewport, clickPosition);
            int draggingTo = 0;
            bool valid = false;
            _validDragDrop = false;

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

        public void ReleaseDirectMove()
        {
            _validDragDrop = false;
        }

        public void HoverDisc(int discIndex, int targetPole)
        {
            // move the disc above the pole its on

            Transform3DGroup? transformGroup = discList[discList.Count - discIndex].Transform as Transform3DGroup;
            if (transformGroup != null)
            {
                TranslateTransform3D? oldTransform = transformGroup.Children[0] as TranslateTransform3D;
                if (oldTransform != null)
                {
                    DoubleAnimation xOffset = new DoubleAnimation();
                    xOffset.From = oldTransform.OffsetX;
                    xOffset.To = (-(float)(poleCount - 1) / 2 + targetPole) * poleRadius * 2.5;
                    xOffset.Duration = new Duration(TimeSpan.FromSeconds(hoverTime * 0.8 / Preferences.AnimationSpeed));
                    xOffset.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

                    DoubleAnimation yOffset = new DoubleAnimation();
                    yOffset.From = oldTransform.OffsetY;
                    yOffset.To = discList.Count * discHeight + 2 * discHeight;
                    yOffset.Duration = new Duration(TimeSpan.FromSeconds(hoverTime / Preferences.AnimationSpeed));
                    yOffset.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

                    oldTransform.BeginAnimation(TranslateTransform3D.OffsetXProperty, xOffset);
                    oldTransform.BeginAnimation(TranslateTransform3D.OffsetYProperty, yOffset);
                }
            }
        }

        public void DropDisc(int discIndex, int targetPole, int discCountOnTargetPole)
        {
            // move the disc

            Transform3DGroup? transformGroup = discList[discList.Count - discIndex].Transform as Transform3DGroup;
            if (transformGroup != null)
            {
                TranslateTransform3D? oldTransform = transformGroup.Children[0] as TranslateTransform3D;
                if (oldTransform != null)
                {
                    DoubleAnimation xOffset = new DoubleAnimation();
                    xOffset.From = oldTransform.OffsetX;
                    xOffset.To = (-(float)(poleCount - 1) / 2 + targetPole) * poleRadius * 2.5;
                    xOffset.Duration = new Duration(TimeSpan.FromSeconds(dropTime / 3 / Preferences.AnimationSpeed));
                    xOffset.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

                    DoubleAnimation yOffset = new DoubleAnimation();
                    yOffset.From = oldTransform.OffsetY;
                    yOffset.To = discCountOnTargetPole * discHeight;
                    yOffset.Duration = new Duration(TimeSpan.FromSeconds(dropTime / Preferences.AnimationSpeed));
                    yOffset.EasingFunction = new BounceEase { Bounces = 3, Bounciness = 5, EasingMode = EasingMode.EaseOut };

                    oldTransform.BeginAnimation(TranslateTransform3D.OffsetXProperty, xOffset);
                    oldTransform.BeginAnimation(TranslateTransform3D.OffsetYProperty, yOffset);
                }
            }
            App.MainApp.dropSound.PlayWait();
        }

        private ModelVisual3D CreateDisc(float radius, float height, float innerRadius, int majorSegments, int minorSegments, float bevel, System.Windows.Media.SolidColorBrush colour)
        {
            MeshGeometry3D disc = new MeshGeometry3D();

            // add the vertices
            float majorDeltaTheta = 2 * MathF.PI / majorSegments;
            float minorDeltaTheta = MathF.PI / minorSegments;
            for (int crossSection = 0; crossSection < majorSegments; crossSection++)
            {
                // create one cross-section

                // inner ring
                double horizontal = innerRadius + height / 2 + bevel;
                double vertical = -height / 2;
                disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection),
                        vertical,
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
                for (int vertex = 0; vertex <= minorSegments; vertex++)
                {
                    horizontal = innerRadius + height / 2 - Math.Sin(minorDeltaTheta * vertex) * height / 2;
                    vertical = -Math.Cos(minorDeltaTheta * vertex) * height / 2;
                    disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection), 
                        vertical, 
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
                }
                horizontal = innerRadius + height / 2 + bevel;
                vertical = height / 2;
                disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection),
                        vertical,
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                // outer ring
                horizontal = radius - height / 2 - bevel;
                vertical = height / 2;
                disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection),
                        vertical,
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
                for (int vertex = 0; vertex <= minorSegments; vertex++)
                {
                    horizontal = radius - height / 2 + Math.Sin(minorDeltaTheta * vertex) * height / 2;
                    vertical = Math.Cos(minorDeltaTheta * vertex) * height / 2;
                    disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection), 
                        vertical, 
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
                }
                horizontal = radius - height / 2 - bevel;
                vertical = -height / 2;
                disc.Positions.Add(new Point3D(
                        horizontal * Math.Cos(majorDeltaTheta * crossSection),
                        vertical,
                        horizontal * Math.Sin(majorDeltaTheta * crossSection)));
            }

            // join the vertices into faces
            int crossSectionResolution = minorSegments * 2 + 6;
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
            model.Material = new DiffuseMaterial(colour);

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);

            discList.Add(visual);
            return visual;
        }

        private ModelVisual3D CreatePole(float width, float height, float baseHeight, float radius, float innerRadius, int segments, float bevel, System.Windows.Media.SolidColorBrush colour)
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

                horizontal = innerRadius + bevel;
                vertical = height;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = radius - bevel;
                vertical = height;
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
                vertical = height - bevel;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = radius;
                vertical = baseHeight + bevel;
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

                horizontal = radius + bevel;
                vertical = baseHeight;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = width - bevel;
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
                vertical = baseHeight - bevel;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = width;
                vertical = bevel;
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

                horizontal = width - bevel;
                vertical = 0;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = innerRadius + bevel;
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

                horizontal = innerRadius;
                vertical = bevel;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));

                horizontal = innerRadius;
                vertical = height - bevel;
                pole.Positions.Add(new Point3D(
                    horizontal * Math.Cos(majorDeltaTheta * crossSection),
                    vertical,
                    horizontal * Math.Sin(majorDeltaTheta * crossSection)));
            }

            // join the vertices into faces
            int crossSectionResolution = 18;
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
            model.Material = new DiffuseMaterial(colour);

            // wrap in a ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;

            // add to the viewport
            viewport.Children.Add(visual);

            poleList.Add(visual);
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
            this.discHeight = discHeight;
            this.poleCount = poleCount;

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

            _cameraTarget.Y = (1 - Math.Sqrt(2) / 2) * discHeight * discCount;

            SolidColorBrush poleColour = (SolidColorBrush)App.MainApp.FindResource("PoleColour");
            poleRadius = discCount + discHeight + 1;
            for (int poleNumber = 0; poleNumber < poleCount; poleNumber++)
            {
                float horizontalMargin = poleRadius * 0.5f;
                float verticalMargin = poleRadius * 0.5f;
                ModelVisual3D pole = CreatePole(poleRadius, discCount * discHeight + 2 * discHeight, discHeight, 1, 0.5f, (int)(poleRadius * 8), 0.2f, poleColour);
                ModelVisual3D hitBox = CreateHitbox(poleRadius * 2 + horizontalMargin, discCount * discHeight + discHeight + verticalMargin, poleRadius * 2 + horizontalMargin);
                Transform3DGroup transform = new Transform3DGroup();
                transform.Children.Add(new TranslateTransform3D()
                {
                    OffsetX = (-(float)(poleCount - 1) / 2 + poleNumber) * poleRadius * 2.5,
                    OffsetY = -discHeight * 1.5
                });
                pole.Transform = transform;
                hitBox.Transform = transform;
            }
            double startingPoleX = (-(float)(poleCount - 1) / 2 + startingPole) * (discCount + discHeight + 1) * 2.5;
            if (poleCount == 0)
            {
                startingPoleX = 0;
            }

            SolidColorBrush? discTop = (SolidColorBrush?)App.MainApp.TryFindResource("DiscTop");
            SolidColorBrush? discBottom = (SolidColorBrush?)App.MainApp.TryFindResource("DiscBottom");

            for (int discNumber = 0; discNumber < discCount; discNumber++)
            {
                float radius = discCount - discNumber + discHeight + 1;
                System.Windows.Media.SolidColorBrush colour;
                if (discTop == null || discBottom == null)
                {
                    // get the specified colour
                    colour = (System.Windows.Media.SolidColorBrush)App.MainApp.FindResource("Disc" + discNumber);
                }
                else
                {
                    // calculate the colour
                    System.Windows.Media.Color topColour = discTop.Color;
                    System.Windows.Media.Color bottomColour = discBottom.Color;
                    float progress = (float)discNumber / discCount;
                    System.Windows.Media.Color blend = System.Windows.Media.Color.FromArgb(
                        (byte)Lerp(bottomColour.A, topColour.A, progress),
                        (byte)Lerp(bottomColour.R, topColour.R, progress),
                        (byte)Lerp(bottomColour.G, topColour.G, progress),
                        (byte)Lerp(bottomColour.B, topColour.B, progress)
                        );
                    colour = new SolidColorBrush(blend);
                }
                ModelVisual3D disc = CreateDisc(radius, 2, 1, (int)(radius * 8), 8, 0.2f, colour);
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
                float width = poleCount * 2 * poleRadius + (poleCount - 1) * poleRadius;
                float z = (float)(width / (2 * Math.Tan(camera.FieldOfView / 360 * Math.PI)) + discHeight + 2);

                SetCameraPos(
                    0, 
                    discHeight * discCount / 2, 
                    z
                    );
            }
        }

        private int Lerp(int start, int end, float progress)
        {
            return (int)(start + (end - start) * progress);
        }

        public void Recolour()
        {
            SolidColorBrush? discTop = (SolidColorBrush?)App.MainApp.TryFindResource("DiscTop");
            SolidColorBrush? discBottom = (SolidColorBrush?)App.MainApp.TryFindResource("DiscBottom");
            for (int discIndex = 0; discIndex < discList.Count; discIndex++)
            {
                ModelVisual3D visual = discList[discIndex];
                GeometryModel3D? geometry = visual.Content as GeometryModel3D;
                if (geometry != null)
                {
                    System.Windows.Media.SolidColorBrush colour;
                    if (discTop == null || discBottom == null)
                    {
                        // get the specified colour
                        colour = (System.Windows.Media.SolidColorBrush)App.MainApp.FindResource("Disc" + discIndex);
                    }
                    else
                    {
                        // calculate the colour
                        System.Windows.Media.Color topColour = discTop.Color;
                        System.Windows.Media.Color bottomColour = discBottom.Color;
                        float progress = (float)discIndex / discList.Count;
                        System.Windows.Media.Color blend = System.Windows.Media.Color.FromArgb(
                            (byte)Lerp(bottomColour.A, topColour.A, progress),
                            (byte)Lerp(bottomColour.R, topColour.R, progress),
                            (byte)Lerp(bottomColour.G, topColour.G, progress),
                            (byte)Lerp(bottomColour.B, topColour.B, progress)
                            );
                        colour = new SolidColorBrush(blend);
                    }
                    geometry.Material = new DiffuseMaterial(colour);
                }
            }
            SolidColorBrush poleColour = (SolidColorBrush)App.MainApp.FindResource("PoleColour");
            for (int poleIndex = 0; poleIndex < poleList.Count; poleIndex++)
            {
                ModelVisual3D visual = poleList[poleIndex];
                GeometryModel3D? geometry = visual.Content as GeometryModel3D;
                if (geometry != null)
                {
                    geometry.Material = new DiffuseMaterial(poleColour);
                }
            }
        }
    }
}
