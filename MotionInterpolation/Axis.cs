using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionInterpolation
{
    public class Axis
    {
        public float X;
        public float Y;
        public float Z;

        GraphicsDevice device;
        Cylinder x;
        Cylinder y;
        Cylinder z;
        
        public Axis(GraphicsDevice device)
        {
            this.device = device;
            x = new Cylinder(device, 2, 0.5f, 16, Color.Red);
            x.Rotate(0, -90, 0);
            y = new Cylinder(device, 2, 0.5f, 16, Color.Green);
            y.Rotate(90, 0, 0);
            z = new Cylinder(device, 2, 0.5f, 16, Color.Blue);
        }

        public void Draw(BasicEffect effect)
        {
            x.Draw(effect);
            y.Draw(effect);
            z.Draw(effect);
        }

        public void Translate(Vector3 translationVector)
        {
            x.Translate(translationVector);
            y.Translate(translationVector);
            z.Translate(translationVector);
        }
        //in deg
        public void Rotate(float x, float y, float z)
        {
            this.x.Rotate(x, y, z);
            this.y.Rotate(x, y, z);
            this.z.Rotate(x, y, z);
        }

        public void Rotate(Vector3 rotationVector)
        {
            this.x.Rotate(rotationVector);
            this.y.Rotate(rotationVector);
            this.z.Rotate(rotationVector);
        }

        public void QuanterionRotation(Quaternion quaternion)
        {
            this.x.QuanterionRotation(quaternion);
            this.y.QuanterionRotation(quaternion);
            this.z.QuanterionRotation(quaternion);
        }

        public void Reset()
        {
            x.Reset();
            x.Rotate(0, -90, 0);
            y.Reset();
            y.Rotate(90, 0, 0);
            z.Reset();
        }
    }
}