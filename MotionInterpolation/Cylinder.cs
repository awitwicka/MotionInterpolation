using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionInterpolation
{
    class Cylinder
    {
        List<VertexPositionColorNormal> vertices = new List<VertexPositionColorNormal>();
        List<VertexPositionColorNormal> verticesDefault = new List<VertexPositionColorNormal>();
        List<short> indices = new List<short>();
        Color color;


        public Cylinder(GraphicsDevice graphicsDevice) : this(graphicsDevice, 2, 1, 32, Color.Blue) {}
        public Cylinder(GraphicsDevice graphicsDevice, float height, float diameter, int tessellation, Color color)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException("tessellation");
            this.color = color;
            //height /= 2;

            float radius = diameter / 2;

            // Create a ring of triangles around the outside of the cylinder.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i, tessellation);

                AddVertex(normal * radius + Vector3.Up * height, this.color, normal);
                AddVertex(normal * radius, this.color, normal);
                //AddVertex(normal * radius + Vector3.Down * height, Color, normal);

                AddIndex(i * 2);
                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 2) % (tessellation * 2));

                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 3) % (tessellation * 2));
                AddIndex((i * 2 + 2) % (tessellation * 2));
            }
            CreateCap(tessellation, height, radius, Vector3.Up);
            CreateCap(tessellation, height, radius, Vector3.Zero);
        }
        void CreateCap(int tessellation, float height, float radius, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    //current vertex 
                    AddIndex(vertices.Count);
                    AddIndex(vertices.Count + (i + 1) % tessellation);
                    AddIndex(vertices.Count + (i + 2) % tessellation);
                }
                else
                {
                    AddIndex(vertices.Count);
                    AddIndex(vertices.Count + (i + 2) % tessellation);
                    AddIndex(vertices.Count + (i + 1) % tessellation);
                }
            }

            // Create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 position = GetCircleVector(i, tessellation) * radius +
                                   normal * height;

                AddVertex(position, color, normal);
            }
        }

        static Vector3 GetCircleVector(int i, int tessellation)
        {
            float angle = i * MathHelper.TwoPi / tessellation;

            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            return new Vector3(dx, 0, dz);
        }

        public void Draw(Effect effect)
        {
            GraphicsDevice graphicsDevice = effect.GraphicsDevice;
            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                int primitiveCount = indices.Count / 3;
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count, indices.ToArray(), 0, indices.Count/3);

            }
        }

        public void Translate(Vector3 translationVector)
        {
            var translation = Matrix.CreateTranslation(translationVector.X, translationVector.Z, translationVector.Y);
            for (int i = 0; i<vertices.Count; i++)
            {
                var v = vertices[i].Position;
                var c = vertices[i].Color;
                var n = vertices[i].Normal;
                vertices[i] = new VertexPositionColorNormal(Vector3.Transform(v, translation), c, n);
                //vertices[i].SetNewPosition(v+translationVector);
            }
        }
        //in deg
        public void Rotate(float x, float y, float z)
        {
            var rotation = Matrix.CreateRotationX(MathHelper.ToRadians(x)) * Matrix.CreateRotationY(MathHelper.ToRadians(z)) * Matrix.CreateRotationZ(MathHelper.ToRadians(y));
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i].Position;
                var c = vertices[i].Color;
                var n = vertices[i].Normal;
                vertices[i] = new VertexPositionColorNormal(Vector3.Transform(v, rotation), c, n);
                //vertices[i].SetNewPosition(Vector3.Transform(v, rotation));
            }
        }

        public void Rotate(Vector3 rotationVector)
        {
            var rotation = Matrix.CreateRotationX(MathHelper.ToRadians(rotationVector.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(rotationVector.Z)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-rotationVector.Y));
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i].Position;
                var c = vertices[i].Color;
                var n = vertices[i].Normal;
                vertices[i] = new VertexPositionColorNormal(Vector3.Transform(v, rotation), c, n);
                //vertices[i].SetNewPosition(Vector3.Transform(v, rotation));
            }
        }

        public void QuanterionRotation(Quaternion quaternion)
        {
            //change z with y to correct display in xna
            //double z = quaternion.Z;
            //quaternion.Z = quaternion.Y;
            //quaternion.Y = z;

            var rotation = quaternion.ToMatrix();
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i].Position;
                var c = vertices[i].Color;
                var n = vertices[i].Normal;
                vertices[i] = new VertexPositionColorNormal(Vector3.Transform(v, rotation), c, n);
            }
        }

        public void Reset()
        {
            for (int i = 0; i < verticesDefault.Count; i++)
            {
                var c = vertices[i].Color;
                var n = vertices[i].Normal;
                vertices[i] = new VertexPositionColorNormal(verticesDefault[i].Position, c, n);
            }
        }

        protected void AddIndex(int index)
        {
            if (index > short.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((short)index);
        }

        protected void AddVertex(Vector3 position, Color color, Vector3 normal)
        {
            vertices.Add(new VertexPositionColorNormal(position, color, normal));
            verticesDefault.Add(new VertexPositionColorNormal(position, color, normal));
        }
    }
}
