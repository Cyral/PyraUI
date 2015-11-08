using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pyratron.UI.Monogame
{
    public class Primitives
    {
        private readonly RasterizerState fillRasterizer = new RasterizerState
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
            MultiSampleAntiAlias = true
        };

        private readonly PrimitiveBatch primitiveBatch;
        private Dictionary<long, VertexBuffer> cache = new Dictionary<long, VertexBuffer>();

        private readonly GraphicsDevice graphicsDevice;

        private RasterizerState wireframeRasterizer = new RasterizerState
        {
            FillMode = FillMode.WireFrame,
            CullMode = CullMode.None,
            MultiSampleAntiAlias = true
        };

        public Primitives(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.RasterizerState = fillRasterizer;
            primitiveBatch = new PrimitiveBatch(graphicsDevice);

            this.graphicsDevice = graphicsDevice;
        }

        public void DrawRectangle(Rectangle rect, Color color, bool filled, float radius = 0)
        {
            if (graphicsDevice.RasterizerState.CullMode != fillRasterizer.CullMode)
                graphicsDevice.RasterizerState = fillRasterizer;
            if (filled)
            {
                primitiveBatch.Begin(PrimitiveType.TriangleList);


                primitiveBatch.AddTriangle(new Vector2(rect.X, rect.Y + radius),
                    new Vector2(rect.X + rect.Width, rect.Y + radius),
                    new Vector2(rect.X, rect.Y + rect.Height - radius), color);

                primitiveBatch.AddTriangle(new Vector2(rect.X + rect.Width, rect.Y + radius),
                    new Vector2(rect.X, rect.Y + rect.Height - radius),
                    new Vector2(rect.X + rect.Width, rect.Y + rect.Height - radius), color);

                primitiveBatch.AddTriangle(new Vector2(rect.X + radius, rect.Y),
                    new Vector2(rect.X + rect.Width - radius, rect.Y),
                    new Vector2(rect.X + radius, rect.Y + rect.Height), color);

                primitiveBatch.AddTriangle(new Vector2(rect.X + rect.Width - radius, rect.Y),
                    new Vector2(rect.X + radius, rect.Y + rect.Height),
                    new Vector2(rect.X + rect.Width - radius, rect.Y + rect.Height), color);


                if (radius > 0)
                {
                    primitiveBatch.AddCircle(new Vector2(rect.X + radius, rect.Y + radius), radius, color, 180, 270);
                    primitiveBatch.AddCircle(new Vector2(rect.X + radius, rect.Y + rect.Height - radius), radius, color, 90, 180);
                    primitiveBatch.AddCircle(new Vector2(rect.X + rect.Width - radius, rect.Y + radius), radius, color, 270, 360);
                    primitiveBatch.AddCircle(new Vector2(rect.X + rect.Width - radius, rect.Y + rect.Height - radius),
                        radius, color, 0, 90);
                }

                primitiveBatch.End();
            }
            else
            {
                primitiveBatch.Begin(PrimitiveType.LineList);

                primitiveBatch.AddLine(new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), color);
                primitiveBatch.AddLine(new Vector2(rect.X, rect.Y + rect.Height),
                    new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
                primitiveBatch.AddLine(new Vector2(rect.X + rect.Width, rect.Y),
                    new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
                primitiveBatch.AddLine(new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), color);

                primitiveBatch.End();
            }
        }
    }


    // PrimitiveBatch is a class that handles efficient rendering automatically for its
    // users, in a similar way to SpriteBatch. PrimitiveBatch can render lines, points,
    // and triangles to the screen. In this sample, it is used to draw a spacewars
    // retro scene.
    public class PrimitiveBatch : IDisposable
    {
        // This constant controls how large the vertices buffer is. Larger buffers will
        // require flushing less often, which can increase performance. However, having
        // buffer that is unnecessarily large will waste memory.
        private const int DefaultBufferSize = 512;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        private readonly BasicEffect basicEffect;

        // the device that we will issue draw calls to.
        private readonly GraphicsDevice device;

        // A block of vertices that calling AddVertex will fill. Flush will draw using
        // this array, and will determine how many primitives to draw from
        // positionInBuffer.
        private readonly VertexPositionColor[] vertices = new VertexPositionColor[DefaultBufferSize];

        // hasBegun is flipped to true once Begin is called, and is used to make
        // sure users don't call End before Begin is called.
        private bool hasBegun;

        private bool isDisposed;

        // how many verts does each of these primitives take up? points are 1,
        // lines are 2, and triangles are 3.
        private int numVertsPerPrimitive;

        // Keeps track of how many vertices have been added. this value increases until
        // we run out of space in the buffer, at which time Flush is automatically
        // called.
        private int positionInBuffer;

        // this value is set by Begin, and is the type of primitives that we are
        // drawing.
        private PrimitiveType primitiveType;

        // the constructor creates a new PrimitiveBatch and sets up all of the internals
        // that PrimitiveBatch will need.
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            device = graphicsDevice;

            // set up a new basic effect, and enable vertex colors.
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            // projection uses CreateOrthographicOffCenter to create 2d projection
            // matrix with 0,0 in the upper left.
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height, 0,
                    0, 1);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType)
        {
            if (hasBegun)
            {
                throw new InvalidOperationException
                    ("End must be called before Begin can be called again.");
            }

            // these three types reuse vertices, so we can't flush properly without more
            // complex logic. Since that's a bit too complicated for this sample, we'll
            // simply disallow them.
            if (primitiveType == PrimitiveType.LineStrip ||
                primitiveType == PrimitiveType.TriangleStrip)
            {
                throw new NotSupportedException
                    ("The specified primitiveType is not supported by PrimitiveBatch.");
            }

            this.primitiveType = primitiveType;

            // how many verts will each of these primitives require?
            numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);

            //tell our basic effect to begin.
            basicEffect.CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            hasBegun = true;
        }

        public void AddLine(Vector2 p1, Vector2 p2, Color color)
        {
            AddVertex(p1, color);
            AddVertex(p2, color);
        }

        public void AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            AddVertex(p1, color);
            AddVertex(p2, color);
            AddVertex(p3, color);
        }

        public void AddCircle(Vector2 pos, float radius, Color color, int start = 0, int end = 360)
        {
            var size = (int)Math.Ceiling(360 / radius / 3);
            
            for (var a = start; a < end; a += 1)
            {
                var heading = MathHelper.ToRadians(a);

                var newVert = pos +
                              (new Vector2((float) (Math.Cos(heading) * radius), (float) (Math.Sin(heading) * radius)));
                var heading2 = MathHelper.ToRadians(a + size);
                var newVert2 = pos +
                               (new Vector2((float) (Math.Cos(heading2) * radius), (float) (Math.Sin(heading2) * radius)));
                //   if (a > 0)
                //    primitiveBatch.AddVertex(last, Color.White);
                AddVertex(newVert, color);
                AddVertex(pos, color);
                AddVertex(newVert2, color);
            }
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. for lines, twice, and for triangles 3 times.
        // this function can only be called once begin has been called.
        // if there is not enough room in the vertices buffer, Flush is called
        // automatically.
        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");
            }

            // are we starting a new primitive? if so, and there will not be enough room
            // for a whole primitive, flush.
            var newPrimitive = ((positionInBuffer % numVertsPerPrimitive) == 0);

            if (newPrimitive &&
                (positionInBuffer + numVertsPerPrimitive) >= vertices.Length)
            {
                Flush();
            }

            // once we know there's enough room, set the vertex in the buffer,
            // and increase position.
            vertices[positionInBuffer].Position = new Vector3(vertex, 0);
            vertices[positionInBuffer].Color = color;

            positionInBuffer++;
        }

        // End is called once all the primitives have been drawn using AddVertex.
        // it will call Flush to actually submit the draw call to the graphics card, and
        // then tell the basic effect to end.
        public void End()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before End can be called.");
            }

            // Draw whatever the user wanted us to draw
            Flush();

            hasBegun = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                if (basicEffect != null)
                    basicEffect.Dispose();

                isDisposed = true;
            }
        }

        // Flush is called to issue the draw call to the graphics card. Once the draw
        // call is made, positionInBuffer is reset, so that AddVertex can start over
        // at the beginning. End will call this to draw the primitives that the user
        // requested, and AddVertex will call this if there is not enough room in the
        // buffer.
        private void Flush()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before Flush can be called.");
            }

            // no work to do
            if (positionInBuffer == 0)
            {
                return;
            }

            // how many primitives will we draw?
            var primitiveCount = positionInBuffer / numVertsPerPrimitive;

            // submit the draw call to the graphics card
            device.DrawUserPrimitives(primitiveType, vertices, 0,
                primitiveCount);

            // now that we've drawn, it's ok to reset positionInBuffer back to zero,
            // and write over any vertices that may have been set previously.
            positionInBuffer = 0;
        }

        // NumVertsPerPrimitive is a boring helper function that tells how many vertices
        // it will take to draw each kind of primitive.
        private static int NumVertsPerPrimitive(PrimitiveType primitive)
        {
            int numVertsPerPrimitive;
            switch (primitive)
            {
                case PrimitiveType.LineList:
                    numVertsPerPrimitive = 2;
                    break;
                case PrimitiveType.TriangleList:
                    numVertsPerPrimitive = 3;
                    break;
                default:
                    throw new InvalidOperationException("primitive is not valid");
            }
            return numVertsPerPrimitive;
        }
    }
}