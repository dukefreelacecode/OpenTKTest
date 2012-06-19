// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Input;
using System.Collections.Generic;
using System.Threading;

namespace OpenTKTest
{
    public class Game : GameWindow
    {
        public static bool Round = true;
        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800,600, new OpenTK.Graphics.GraphicsMode(OpenTK.Graphics.ColorFormat.Empty, 16, 0, 16), "Cole Erickson - Test")
        {
            VSync = VSyncMode.On;
        }
        public World World;
        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Location = new Point(300, 300);

            //Height = DisplayDevice.GetDisplay(DisplayIndex.Default).Height;
            //Width = DisplayDevice.GetDisplay(DisplayIndex.Default).Width;


            CursorVisible = false;
            OpenTK.Input.Mouse.SetPosition(this.Location.X + Width / 2, this.Location.Y + Height / 2);
            GL.ClearColor(1f, 0.8f, 0.2f, 0.0f);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Multisample);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Keyboard.KeyDown += delegate(object sender, KeyboardKeyEventArgs eventArgs)
            {
                if (eventArgs.Key == Key.J) Round = !Round;
                if (eventArgs.Key != Key.E) return;
                wireframe = !wireframe;
            };
            Mouse.ButtonDown += delegate(object sender, MouseButtonEventArgs eventArgs)
                {
                    if (eventArgs.Button != MouseButton.Left) return;
                    if (selectedBlock == null || selectedBlock.Type == BlockType.Air) return;
                    World.SetBlock(selectedBlock.Location, BlockType.Air);
                };

            this.WindowBorder = OpenTK.WindowBorder.Hidden;

            World = new World();
            chunkGenerationThread = new Thread(new ThreadStart(delegate() { while (true) {
                for (int x = -genRadius; x < genRadius; x++)
                {
                    for (int z = -genRadius; z < genRadius; z++)
                    {
                    }
                }
                Thread.Sleep(2000); } }));
            //chunkGenerationThread.Start();
        }
        private Thread chunkGenerationThread;

        private const int genRadius = 5;

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

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 0.1f, 4000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        bool wireframe = false;
        float moveSpeed = 0.2f;
        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();

            Vector3 moveVector = Vector3.Zero;
            if (Keyboard[Key.W])
            {
                moveVector.Z -= (float)Math.Cos(yaw);
                moveVector.X -= (float)Math.Sin(yaw);
            }
            if (Keyboard[Key.S])
            {
                moveVector.Z += (float)Math.Cos(yaw);
                moveVector.X += (float)Math.Sin(yaw);
            }
            if (Keyboard[Key.A])
            {
                moveVector.Z += (float)Math.Sin(yaw);
                moveVector.X -= (float)Math.Cos(yaw);
            }
            if (Keyboard[Key.D])
            {
                moveVector.X += (float)Math.Cos(yaw);
                moveVector.Z -= (float)Math.Sin(yaw);
            }
            if (Keyboard[Key.Space])
                moveVector.Y++;
            if (Keyboard[Key.LShift])
                moveVector.Y--;
            if (moveVector != Vector3.Zero)
                moveVector.Normalize();
            playerPosition += moveVector * moveSpeed;

            yaw -= (Mouse.X - (Width / 2)) * 0.001f;
            pitch -= (Mouse.Y - (Height / 2)) * 0.001f;

            cameraPos = playerPosition;

            if (yaw > MathHelper.Pi) yaw -= MathHelper.TwoPi;
            else if (yaw < -MathHelper.Pi) yaw += MathHelper.TwoPi;

            if (pitch > MathHelper.PiOver2 - 0.001f) pitch = MathHelper.PiOver2 - 0.001f;
            else if (pitch < -MathHelper.PiOver2 + 0.001f) pitch = -MathHelper.PiOver2 + 0.001f;

            Matrix4 rotationMatrix = Matrix4.CreateRotationX(pitch) * Matrix4.CreateRotationY(yaw);
            Vector3 transformedReference = Vector3.Transform(cameraReferenceDirection, rotationMatrix);
            Vector3 cameraLookAt = cameraPos + transformedReference;

            positions = new List<Vector3>(100);
            selectedBlock = null;
            Tracer v = new Tracer(0,0,0, 1, 1, 1);
            v.plot(cameraPos, transformedReference, 16);
            while (v.next())
            {
                Vector3i location = v.get();
                positions.Add(location.ToVector3());

                Block b = World.GetBlockAt(location);
                
                if (b == null || b.Type == BlockType.Air) continue;
                else
                {
                    selectedBlock = b;
                    break;
                }
            }

            view = Matrix4.LookAt(cameraPos, cameraLookAt, Vector3.UnitY);

            OpenTK.Input.Mouse.SetPosition(this.Location.X + Width / 2, this.Location.Y + Height / 2);
        }

        List<Vector3> positions = new List<Vector3>();

        Block selectedBlock;
        //Vector3 transformedReference = new Vector3(0, 0, -1);
        readonly Vector3 cameraReferenceDirection = new Vector3(0, 0, -1);

        Vector3 target = Vector3.Zero;
        Matrix4 view;
        float yaw = (float)Math.PI;
        float pitch;
        Vector3 playerPosition = Vector3.UnitZ * -5;
        Vector3 cameraPos = Vector3.Zero;

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
            GL.PolygonMode(MaterialFace.Back, wireframe ? PolygonMode.Line : PolygonMode.Fill);
            GL.Color4(0.8f, 0.2f, 0.0f, 1.0f);

            World.DrawChunks(view);
            GL.LoadMatrix(ref view);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.Color4(1f, 1f, 1f, 0.5f);
            GL.VertexPointer(3, VertexPointerType.Float, 0, cube);

            GL.Begin(BeginMode.LineStrip);
            foreach (var location in positions)
            {
                GL.LoadMatrix(ref view);
                GL.Vertex3(location);
            }
            GL.End();
            if (selectedBlock != null)
            {
                GL.LoadMatrix(ref view);
                GL.Translate(selectedBlock.Location.ToVector3() + new Vector3(Chunk.BLOCK_RENDER_SIZE / (float)2));
                GL.Scale(new Vector3(1.1f));
                GL.DrawElements(BeginMode.Triangles, 36, DrawElementsType.UnsignedByte, triangles);
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
                game.Run(60.0);
            }
        }

        #region Cube information

        byte[] triangles =
		{
			0, 1, 2, // front
			2, 3, 0,
			4, 6, 5, // back
			6, 4, 7,
			7, 4, 0, // left
			3, 7, 0,
			2, 1, 5, //right
			6, 2, 5,
			1, 0, 5, // top
			5, 0, 4,
			3, 2, 6, // bottom
			7, 3, 6,
		};

        float[] cube = {
			-0.5f,  0.5f,  0.5f, // vertex[0]
			 0.5f,  0.5f,  0.5f, // vertex[1]
			 0.5f, -0.5f,  0.5f, // vertex[2]
			-0.5f, -0.5f,  0.5f, // vertex[3]
			-0.5f,  0.5f, -0.5f, // vertex[4]
			 0.5f,  0.5f, -0.5f, // vertex[5]
			 0.5f, -0.5f, -0.5f, // vertex[6]
			-0.5f, -0.5f, -0.5f, // vertex[7]
		};

        #endregion
    }
}