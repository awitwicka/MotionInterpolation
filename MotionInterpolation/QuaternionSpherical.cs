using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionInterpolation
{
    public class QuaternionSpherical : IInterpolation
    {
        GraphicsDevice device;

        public Vector3 Position0;
        public Vector3 Position1;
        public Quaternion Rotation0;
        public Quaternion Rotation1;
        public Color LineColor = Color.Black;

        private Axis Axe0;
        private Axis Axe1;
        private Axis AxeNext;
        private List<VertexPositionColor> LineVertices = new List<VertexPositionColor>();
        private List<short> LineIndices = new List<short>();

        public QuaternionSpherical(GraphicsDevice device, Vector3 position0, Vector3 position1, Quaternion rotation0, Quaternion rotation1)
        {
            this.device = device;
            Position0 = position0;
            Position1 = position1;
            Rotation0 = rotation0;
            Rotation1 = rotation1;
            Initialize();
        }
        //TODO: additional constructor with color & defult contructor

        private void Initialize()
        {
            LineVertices.Add(new VertexPositionColor(Position0, LineColor));
            Axe0 = new Axis(device);
            Axe0.QuanterionRotation(Rotation0);
            Axe0.Translate(Position0);

            Axe1 = new Axis(device);
            Axe1.QuanterionRotation(Rotation1);
            Axe1.Translate(Position1);

            AxeNext = new Axis(device);
            AxeNext.QuanterionRotation(Rotation0);
            AxeNext.Translate(Position0);
        }

        private void NextStep(float time) //0-1
        {
            var nextPos = (1 - time) * Position0 + time * Position1;
            var nextAngle = Quaternion.Slerp(Rotation0, Rotation1, time);
            //var nextAngle = Rotation0 * Math.Pow((Quaternion.Inverse(Rotation0) * Rotation1), time);

            //drawing line
            short vertCount = (short)LineVertices.Count;
            LineVertices.Add(new VertexPositionColor(nextPos, LineColor));
            LineIndices.Add((short)(vertCount - 1));
            LineIndices.Add(vertCount);

            AxeNext.Reset();
            AxeNext.QuanterionRotation(nextAngle); //reset and rotate
            AxeNext.Translate(nextPos); //translate

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="effect"></param>
        /// <param name="timeElapsedFromAnimationStart">in miliseconds</param>
        /// <param name="totalAnimationTime">in seconds</param>
        public void Draw(ArcBallCamera camera, BasicEffect effect, BasicEffect wireframeEffect, double timeElapsedFromAnimationStart, double totalAnimationTime, bool isAnimated)
        {
            if (isAnimated)
            {
                totalAnimationTime *= 1000;
                if (timeElapsedFromAnimationStart <= totalAnimationTime /*&& gameTime.ElapsedGameTime.TotalMilliseconds>=1*/)
                    NextStep((float)(timeElapsedFromAnimationStart / totalAnimationTime));
            }
            Axe0.Draw(effect);
            Axe1.Draw(effect);
            AxeNext.Draw(effect);

            if (LineVertices.Count - 1 > 0)
                foreach (EffectPass pass in wireframeEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, LineVertices.ToArray(), 0, LineVertices.Count, LineIndices.ToArray(), 0, LineVertices.Count - 1);
                }
        }
    }
}
