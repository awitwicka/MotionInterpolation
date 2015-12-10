using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionInterpolation
{
    public class EulerLinear : IInterpolation
    {
        GraphicsDevice device;

        public Vector3 Position0;
        public Vector3 Position1;
        public Vector3 Rotation0;
        public Vector3 Rotation1;
        public Color LineColor = Color.Black;
        public bool IsVisible = true;

        private Axis Axe0;
        private Axis Axe1;
        private Axis AxeNext;
        private List<VertexPositionColor> LineVertices = new List<VertexPositionColor>();
        private List<short> LineIndices = new List<short>();
        private List<Axis> Steps = new List<Axis>();

        public EulerLinear(GraphicsDevice device, Vector3 position0, Vector3 position1, Vector3 rotation0, Vector3 rotation1)
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
            Axe0.Rotate(Rotation0);
            Axe0.Translate(Position0);
            
            Axe1 = new Axis(device);
            Axe1.Rotate(Rotation1);
            Axe1.Translate(Position1);

            AxeNext = new Axis(device);
            AxeNext.Rotate(Rotation0);
            AxeNext.Translate(Position0);   
        }

        public void ResetSteps(int steps)
        {
            Steps.Clear();
            float step = 1 / (float)(steps+1);
            for (int i = 1; i <= steps; i++)
            {
                Steps.Add(new Axis(device));   
                NextStep(step*i, Steps.Last(), false);
            }
        }

        private void NextStep(float time, Axis axis, bool drawLine) //0-1
        {
            var nextPos = (1 - time) * Position0 + time * Position1;
            var nextAngle = (1 - time) * Rotation0 + time * Rotation1;

            //TODO: check Z not inverted
            //var rotation = Matrix.CreateRotationX(MathHelper.ToRadians(nextAngle.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(nextAngle.Z)) * Matrix.CreateRotationZ(MathHelper.ToRadians(nextAngle.Y));
            //Vector3 nextAxisX = Vector3.Transform(new Vector3(1, 0, 0), rotation);
            //Vector3 nextAxisY = Vector3.Transform(new Vector3(0, 1, 0), rotation);
            //Vector3 nextAxisZ = Vector3.Transform(new Vector3(0, 0, 1), rotation);

            //drawing line
            if (drawLine)
            {
                short vertCount = (short)LineVertices.Count;
                LineVertices.Add(new VertexPositionColor(nextPos, LineColor));
                LineIndices.Add((short)(vertCount - 1));
                LineIndices.Add(vertCount);
            }
            axis.Reset();
            axis.Rotate(nextAngle); //reset and rotate
            axis.Translate(nextPos); //translate

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
                    NextStep((float)(timeElapsedFromAnimationStart / totalAnimationTime), AxeNext, true);
            }
            if (IsVisible)
            {
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
        public void DrawStages(ArcBallCamera camera, BasicEffect effect, BasicEffect wireframeEffect)
        {
            if (IsVisible)
            {
                Axe0.Draw(effect);
                Axe1.Draw(effect);
                foreach (var a in Steps)
                    a.Draw(effect);
            }
        }

    }
}
