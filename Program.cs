// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Input;

namespace OpenTKTest
{
    class Game : GameWindow
    {


        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(1280, 1024, OpenTK.Graphics.GraphicsMode.Default, "Cole Erickson - Test", GameWindowFlags.Fullscreen)
        {
            VSync = VSyncMode.On;
        }
        ChunkManager cm;
        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //this.Location = new Point(200,200);

            CursorVisible = false;
            OpenTK.Input.Mouse.SetPosition(this.Location.X + Width / 2, this.Location.Y + Height / 2);
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);

            this.WindowBorder = OpenTK.WindowBorder.Hidden;

            cm = new ChunkManager();
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

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 4000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        float moveSpeed = 5f;
        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();

            Console.WriteLine(yaw);

            Vector3 move = Vector3.Zero;
            if (Keyboard[Key.W])
            {
                move.Z -= (float)Math.Cos(yaw);
                move.X -= (float)Math.Sin(yaw);
            }
            if (Keyboard[Key.S])
            {
                move.Z += (float)Math.Cos(yaw);
                move.X += (float)Math.Sin(yaw);
            }
            if (Keyboard[Key.A])
            {
                move.Z += (float)Math.Sin(yaw);
                move.X -= (float)Math.Cos(yaw);
            }
            if (Keyboard[Key.D])
            {
                move.X += (float)Math.Cos(yaw);
                move.Z -= (float)Math.Sin(yaw);
            }
            if (Keyboard[Key.Space])
                move.Y++;
            if (Keyboard[Key.LShift])
                move.Y--;
            if (move != Vector3.Zero)
                move.Normalize();
            playerPos += move * moveSpeed;

            yaw -= (Mouse.X - (Width / 2)) * 0.001f;
            pitch -= (Mouse.Y - (Height / 2)) * 0.001f;

            cameraPos = playerPos;

            if (yaw > MathHelper.Pi) yaw -= MathHelper.TwoPi; //MAGIC NUMBARSSSSS
            else if (yaw < -MathHelper.Pi) yaw += MathHelper.TwoPi;

            if (pitch > MathHelper.PiOver2 - 0.001f) pitch = MathHelper.PiOver2 - 0.001f;
            else if (pitch < -MathHelper.PiOver2 + 0.001f) pitch = -MathHelper.PiOver2 + 0.001f;

            Matrix4 rotationMatrix = Matrix4.CreateRotationX(pitch) * Matrix4.CreateRotationY(yaw) ;
            Vector3 transformedReference = Vector3.Transform(CAMERA_REFERENCE, rotationMatrix);
            Vector3 cameraLookAt = cameraPos + transformedReference;

            view = Matrix4.LookAt(cameraPos, cameraLookAt, Vector3.UnitY);


            //Console.WriteLine(cameraPos - transformedReference);

            OpenTK.Input.Mouse.SetPosition(this.Location.X + Width / 2, this.Location.Y + Height / 2);
        }

        readonly Vector3 CAMERA_REFERENCE = new Vector3(0, 0, -1);

        Vector3 target = Vector3.Zero;
        Matrix4 view;
        float yaw;
        float pitch;
        Vector3 playerPos = Vector3.UnitZ * 50;
        Vector3 cameraPos = Vector3.Zero;

        readonly Vector3 HEAD_OFFSET = new Vector3(0, 3, 0);

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);

            GL.CullFace(CullFaceMode.Front);
            GL.PolygonMode(MaterialFace.Back, PolygonMode.Point);
            GL.Color4(0.6f, 0.8f, 0.2f, 0.0f);
            cm.DrawChunks();

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
                game.Run(60.0);
            }
        }
    }
}