﻿// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenTKTest
{
    class Game : GameWindow
    {
        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, OpenTK.Graphics.GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;
        }
        Chunk chunk;
        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chunk = new Chunk();
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            chunk.Generate();
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);

            //Console.WriteLine(Marshal.SizeOf(new Vector3(4, 5, 6.001f)));
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 100.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        float moveSpeed = 1f;
        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();

            if (Keyboard[Key.W])
                cameraPosition.Z -= moveSpeed;
            if (Keyboard[Key.S])
                cameraPosition.Z += moveSpeed;
            if (Keyboard[Key.A])
                cameraPosition.X -= moveSpeed;
            if (Keyboard[Key.D])
                cameraPosition.X += moveSpeed;
            if (Keyboard[Key.Space])
                cameraPosition.Y += moveSpeed;
            if (Keyboard[Key.LShift])
                cameraPosition.Y -= moveSpeed;
        }

        Vector3 cameraPosition = Vector3.UnitZ * 50;

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(cameraPosition, cameraPosition - Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            if (chunk.IsReady)
            {
                GL.Color4(0.6f, 0.8f, 0.2f, 0.0f);
                chunk.Draw();
            }

            SwapBuffers();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}