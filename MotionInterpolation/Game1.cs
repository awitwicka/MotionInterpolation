﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MotionInterpolation
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public bool IsAnimated = false;
        public bool IsStepVisible = true;
        private int stepsNumber = 0;
        public int StepsNumber { get { return stepsNumber; } set { stepsNumber = value; ResetInterpolationsSteps(); } }

        public EulerLinear EulerLinInterpolation;
        public QuaternionLinear QuaternionLinInterpolation;
        public QuaternionSpherical QuaternionSpherInterpolation;

        public Vector3 Position0;
        public Vector3 Position1;
        public Vector3 EulerRotation0;
        public Vector3 EulerRotation1;

        private Quaternion Rotation0;
        private Quaternion Rotation1;

        private BasicEffect effect;
        private BasicEffect wireframeEffect;
        private ArcBallCamera camera;
        private List<VertexPositionColor> GlobalAxisVertices = new List<VertexPositionColor>();
        private List<short> GlobalAxisIndices = new List<short>();
        private double timeElapsedFromAnimationStart = 0;

        //camera control
        private float scrollRate = 1.0f;
        private MouseState previousMouse;

        Viewport defaultViewport;
        Viewport leftViewport;
        Viewport rightViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            //graphics.PreferredBackBufferWidth = 900;
            //graphics.PreferredBackBufferHeight = 900;
            //graphics.SwapChainPanel.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0,0,0,0));
            //graphics.GraphicsDevice.Viewport.Bounds = new Rectangle(50,50,100,100);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            Position0 = new Vector3(-1, -1, -1);
            Position1 = new Vector3(10, 10, 10);
            EulerRotation0 = new Vector3(0, 0, 0);
            EulerRotation1 = new Vector3(20, 170, 80);
            //EulerRotation0 = new Vector3(2, 10, 60);
            //EulerRotation1 = new Vector3(180, -174, 190);
            //todo check if not in constructor
            effect = new BasicEffect(graphics.GraphicsDevice);
            wireframeEffect = new BasicEffect(graphics.GraphicsDevice);
            camera = new ArcBallCamera(new Vector3(0f, 0f, 0f), MathHelper.ToRadians(-200), 0f, 10f, 300f, 50f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 512f);

            Rotation0 = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-EulerRotation0.Z), MathHelper.ToRadians(-EulerRotation0.X), MathHelper.ToRadians(EulerRotation0.Y));
            Rotation1 = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-EulerRotation1.Z), MathHelper.ToRadians(-EulerRotation1.X), MathHelper.ToRadians(EulerRotation1.Y));
            //Rotation0 = Utility.EulerToQuaternion(MathHelper.ToRadians(-EulerRotation0.Z), MathHelper.ToRadians(EulerRotation0.Y), MathHelper.ToRadians(EulerRotation0.X));
            //Rotation1 = Utility.EulerToQuaternion(MathHelper.ToRadians(-EulerRotation1.Z), MathHelper.ToRadians(EulerRotation1.Y), MathHelper.ToRadians(EulerRotation1.X));

            InitializeGlobalAxis(4);

            base.Initialize();
        }
        private void InitializeGlobalAxis(int size)
        {
            //x
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(size, 0, 0), Color.Red));
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.Red));
            //z
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, size, 0), Color.Blue));
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue));
            //y
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, size), Color.Green));
            GlobalAxisVertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.Green));

            for (int i = 0; i < GlobalAxisVertices.Count; i++)
            {
                GlobalAxisIndices.Add((short)i);
            }
        }
        protected override void LoadContent()
        {
            EulerLinInterpolation = new EulerLinear(graphics.GraphicsDevice, Position0, Position1, EulerRotation0, EulerRotation1);
            QuaternionLinInterpolation = new QuaternionLinear(graphics.GraphicsDevice, Position0, Position1, Rotation0, Rotation1);
            QuaternionSpherInterpolation = new QuaternionSpherical(graphics.GraphicsDevice, Position0, Position1, Rotation0, Rotation1);
            QuaternionSpherInterpolation.IsVisible = false;

            defaultViewport = GraphicsDevice.Viewport;
            leftViewport = defaultViewport;
            rightViewport = defaultViewport;
            leftViewport.Width = leftViewport.Width / 2;
            rightViewport.Width = rightViewport.Width / 2;
            rightViewport.X = leftViewport.Width;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1.0f, 10000f);
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            //CAMERA
            if (keyState.IsKeyDown(Keys.W))
            {
                camera.Elevation -= MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                camera.Elevation += MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                 camera.Rotation -= MathHelper.ToRadians(2);
            }
            if (keyState.IsKeyDown(Keys.D))
            {
               camera.Rotation += MathHelper.ToRadians(2);
            }
            if (mouseState.ScrollWheelValue - previousMouse.ScrollWheelValue != 0)
            {
                float wheelChange = mouseState.ScrollWheelValue - previousMouse.ScrollWheelValue;
                camera.ViewDistance -= (wheelChange / 120) * scrollRate;
            }
            previousMouse = mouseState;
            base.Update(gameTime);
        }
        
        private void ResetInterpolationsSteps()
        {
            EulerLinInterpolation.ResetSteps(StepsNumber);
            QuaternionLinInterpolation.ResetSteps(StepsNumber);
            QuaternionSpherInterpolation.ResetSteps(StepsNumber);
        }
        private void DrawAxis(ArcBallCamera camera, Effect wireframeEffect)
        {
            foreach (EffectPass pass in wireframeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, GlobalAxisVertices.ToArray(), 0, GlobalAxisVertices.Count, GlobalAxisIndices.ToArray(), 0, GlobalAxisIndices.Count / 2);
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.VertexColorEnabled = true;
            effect.EnableDefaultLighting();
            effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();

            wireframeEffect.World = Matrix.Identity;
            wireframeEffect.View = camera.View;
            wireframeEffect.Projection = camera.Projection;
            wireframeEffect.VertexColorEnabled = true;

            if(IsAnimated)
                timeElapsedFromAnimationStart += gameTime.ElapsedGameTime.TotalMilliseconds;

            GraphicsDevice.Viewport = leftViewport;
            DrawAxis(camera, wireframeEffect);
            EulerLinInterpolation.Draw(camera, effect, wireframeEffect, timeElapsedFromAnimationStart, 10, IsAnimated);
            if (IsStepVisible)
                EulerLinInterpolation.DrawStages(camera, effect, wireframeEffect);

            GraphicsDevice.Viewport = rightViewport;
            DrawAxis(camera, wireframeEffect);
            QuaternionLinInterpolation.Draw(camera, effect, wireframeEffect, timeElapsedFromAnimationStart, 10, IsAnimated);
            if (IsStepVisible)
                QuaternionLinInterpolation.DrawStages(camera, effect, wireframeEffect);
            QuaternionSpherInterpolation.Draw(camera, effect, wireframeEffect, timeElapsedFromAnimationStart, 10, IsAnimated);
            if (IsStepVisible)
                QuaternionSpherInterpolation.DrawStages(camera, effect, wireframeEffect);
            base.Draw(gameTime);
        }
    }
}
