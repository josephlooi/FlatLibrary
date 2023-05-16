using System;
using FlatEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace FlatEngine.Graphics
{
    public sealed class Shapes : IDisposable
    {
        private const double minLineThickness = FlatMath.MinAccuracy;
        private const double maxLineThickness = 32;

        private const int circlePoints = 24;
        private const int minRegularPolygonPoints = 3;
        private const int maxRegularPolygonPoints = 128;

        private const int maxVertexCount = 1024;
        private const int maxIndexCount = maxVertexCount * 3;

        private readonly Game game;
        private readonly BasicEffect effect;
        private readonly Vertex[] vertices;
        private readonly int[] indices;

        private bool isStarted;
        private bool isDisposed;
        private int shapeCount;
        private int vertexCount;
        private int indexCount;
        private Camera camera;


        public Shapes(Game game, Camera camera)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.camera = camera;
            this.isDisposed = false;
            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.World = Matrix.Identity;
            this.effect.Projection = Matrix.Identity;
            this.effect.View = Matrix.Identity;

            this.vertices = new Vertex[maxVertexCount];
            this.indices = new int[maxIndexCount];

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;

            this.isStarted = false;
        }



        public void Dispose()
        {
            if (this.isDisposed) return;
            this.effect?.Dispose();
            this.isDisposed = true;
        }
        public void Begin(Camera camera)
        {
            if (this.isStarted) throw new Exception("batching already started");

            if (camera == null)
            {
                Viewport vp = this.game.GraphicsDevice.Viewport;
                this.effect.View = Matrix.Identity;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0f, vp.Width, 0f, vp.Height, 0f, 1f);
            }
            else
            {
                this.camera = camera;
                this.camera.Update();
                this.effect.View = this.camera.View;
                this.effect.Projection = this.camera.Projection;
            }
            this.isStarted = true;
        }
        public void End()
        {
            Flush();
            this.isStarted = false;
        }



        private void ConfirmStart()
        {
            if (!this.isStarted) throw new Exception("batching never started");
        }
        private void ConfirmEnoughSpace(int vertexCount, int indexCount)
        {
            if (vertexCount > maxVertexCount) throw new Exception($"Max shape vertices of {maxVertexCount} exceeded");
            if (indexCount > maxIndexCount) throw new Exception($"Max shape indices of {maxIndexCount} exceeded");

            // confirm that the next vertices and indices being drawn are not overwriting previous ones.
            if (this.vertexCount + vertexCount > this.vertices.Length ||
                this.indexCount + indexCount > this.indices.Length) Flush();
        }
        public void Flush()
        {
            if (this.shapeCount == 0) return;

            this.ConfirmStart();

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.game.GraphicsDevice.DrawUserIndexedPrimitives<Vertex>(
                    PrimitiveType.TriangleList,
                    this.vertices, 0, this.vertexCount,
                    this.indices, 0, this.indexCount / 3);
            }
            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;
        }



        private void GenerateQuad(FlatVector q1, FlatVector q2, FlatVector q3, FlatVector q4, Color color)
        {
            this.GenerateQuad(q1.X, q1.Y, q2.X, q2.Y, q3.X, q3.Y, q4.X, q4.Y, color);
        }
        private void GenerateQuad(double q1x, double q1y, double q2x, double q2y, double q3x, double q3y, double q4x, double q4y, Color color)
        {
            this.ConfirmStart();
            this.ConfirmEnoughSpace(4, 6);

            this.IndexTriangles(2);
            this.vertices[this.vertexCount++] = new Vertex(new Vector3((float)q1x, (float)q1y, 0f), color);
            this.vertices[this.vertexCount++] = new Vertex(new Vector3((float)q2x, (float)q2y, 0f), color);
            this.vertices[this.vertexCount++] = new Vertex(new Vector3((float)q3x, (float)q3y, 0f), color);
            this.vertices[this.vertexCount++] = new Vertex(new Vector3((float)q4x, (float)q4y, 0f), color);

            this.shapeCount++;
        }
        private void IndexTriangles(int triangleCount)
        {
            int index = 1;
            for (int i = 0; i < triangleCount; i++)
            {
                this.indices[this.indexCount++] = this.vertexCount;
                this.indices[this.indexCount++] = index + this.vertexCount;
                this.indices[this.indexCount++] = ++index + this.vertexCount;
            }
        }



        public void DrawSquare(FlatVector positon, double size, double thickness, Color color)
        {
            this.DrawSquare(positon.X, positon.Y, size, thickness, color);
        }
        public void DrawSquare(double x, double y, double size, double thickness, Color color)
        {
            double halfSize = FlatMath.Div2(size);
            this.DrawRectangle(x - halfSize, y - halfSize, size, size, thickness, color);
        }
        public void DrawRectangle(FlatVector positon, double width, double height, double thickness, Color color)
        {
            this.DrawRectangle(positon.X, positon.Y, width, height, thickness, color);
        }
        public void DrawRectangle(double x, double y, double width, double height, double thickness, Color color)
        {
            double left = x;
            double right = x + width;
            double bottom = y;
            double top = y + height;
            this.DrawLine(left, top, right, top, thickness, color);
            this.DrawLine(right, top, right, bottom, thickness, color);
            this.DrawLine(right, bottom, left, bottom, thickness, color);
            this.DrawLine(left, bottom, left, top, thickness, color);
        }
        public void DrawSquareFill(FlatVector positon, double size, Color color)
        {
            this.DrawSquareFill(positon.X, positon.Y, size, color);
        }
        public void DrawSquareFill(double x, double y, double size, Color color)
        {
            double halfSize = FlatMath.Div2(size);
            this.DrawRectangleFill(x - halfSize, y - halfSize, size, size, color);
        }
        public void DrawRectangleFill(FlatVector positon, double width, double height, Color color)
        {
            this.DrawRectangleFill(positon.X, positon.Y, width, height, color);
        }
        public void DrawRectangleFill(double x, double y, double width, double height, Color color)
        {
            double left = x;
            double right = x + width;
            double bottom = y;
            double top = y + height;
            this.GenerateQuad(
                left, top,
                right, top,
                right, bottom,
                left, bottom,
                color);
        }
        public void DrawLine(double ax, double ay, double bx, double by, double thickness, Color color)
        {
            this.DrawLine(new FlatVector(ax, ay), new FlatVector(bx, by), thickness, color);
        }
        public void DrawLine(FlatVector a, FlatVector b, double thickness, Color color)
        {
            thickness = FlatTools.Clamp(thickness, minLineThickness, maxLineThickness);
            thickness *= FlatMath.Div2(this.camera.Z / this.camera.BaseZ);

            FlatVector edge = (b - a).Unit() * thickness;
            FlatVector normal = edge.Normal;

            FlatVector q1 = a + normal - edge;
            FlatVector q2 = b + normal + edge;
            FlatVector q3 = b - normal + edge;
            FlatVector q4 = a - normal - edge;

            this.GenerateQuad(q1, q2, q3, q4, color);
        }



        private int LevelOfDetail(double radius)
        {
            //double levelOfDetail = Shapes.CirclePoints * Math.Pow(radius, -8f * this.camera.Zoom);
            //levelOfDetail = FlatTools.Clamp(levelOfDetail, 4, Shapes.maxRegularPolygonPoints);
            //Console.WriteLine(levelOfDetail);
            //return (int)Math.Round(levelOfDetail);
            return Shapes.circlePoints;
        }
        public void DrawCircle(FlatVector position, double radius, double thickness, Color color)
        {
            this.DrawCircle(position.X, position.Y, radius, thickness, color);
        }
        public void DrawCircle(double x, double y, double radius, double thickness, Color color)
        {
            this.DrawRegularPolygon(x, y, radius, this.LevelOfDetail(radius), thickness, color);
        }
        public void DrawCircleFill(FlatVector position, double radius, Color color)
        {
            this.DrawCircleFill(position.X, position.Y, radius, color);
        }
        public void DrawCircleFill(double x, double y, double radius, Color color)
        {
            this.DrawRegularPolygonFill(x, y, radius, this.LevelOfDetail(radius), color);
        }
        public void DrawRegularPolygon(FlatVector position, double radius, int vertexCount, double thickness, Color color)
        {
            DrawRegularPolygon(position.X, position.Y, radius, vertexCount, thickness, color);
        }
        public void DrawRegularPolygon(double x, double y, double radius, int vertexCount, double thickness, Color color)
        {
            vertexCount = FlatTools.Clamp(vertexCount, Shapes.minRegularPolygonPoints, Shapes.maxRegularPolygonPoints);

            double angle = FlatMath.PiMul2 / vertexCount;
            (double sin, double cos) = Math.SinCos(angle);
            double ax = radius;
            double ay = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                double bx = Math.FusedMultiplyAdd(cos, ax, -sin * ay);
                double by = Math.FusedMultiplyAdd(sin, ax,  cos * ay);

                this.DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

                ax = bx;
                ay = by;
            }
        }
        public void DrawRegularPolygonFill(FlatVector position, double radius, int vertexCount, Color color)
        {
            DrawRegularPolygonFill(position.X, position.Y, radius, vertexCount, color);
        }
        public void DrawRegularPolygonFill(double x, double y, double radius, int vertexCount, Color color)
        {
            this.ConfirmStart();

            vertexCount = FlatTools.Clamp(vertexCount, Shapes.minRegularPolygonPoints, Shapes.maxRegularPolygonPoints);
            int triangleCount = vertexCount - 2;
            int indexCount = triangleCount * 3;

            this.ConfirmEnoughSpace(vertexCount, indexCount);
            this.IndexTriangles(triangleCount);

            double angle = FlatMath.PiMul2 / vertexCount;
            (double sin, double cos) = Math.SinCos(angle);
            double x1 = radius;
            double y1 = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                double x2 = x1;
                double y2 = y1;

                this.vertices[this.vertexCount++] = new Vertex(new Vector3((float)(x2 + x), (float)(y2 + y), 0f), color);

                x1 = Math.FusedMultiplyAdd(cos, x2, -sin * y2);
                y1 = Math.FusedMultiplyAdd(sin, x2,  cos * y2);
            }
            shapeCount++;
        }
        public void DrawPolygon(FlatVector[] vertices, double thickness, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                FlatVector a = vertices[i];
                FlatVector b = vertices[(i + 1) % vertices.Length];
                this.DrawLine(a, b, thickness, color);
            }
        }
        public void DrawPolygonFill(FlatVector[] vertices, Color color)
        {
            this.ConfirmStart();
            
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));
            else if (vertices.Length < 3) throw new ArgumentOutOfRangeException(nameof(vertices));
            int vertexCount = vertices.Length;
            int indexCount = vertexCount - 2;

            this.ConfirmEnoughSpace(vertexCount, indexCount);
            this.IndexTriangles(indexCount);

            for (int i = 0; i < vertices.Length; i++)
            {
                FlatVector v = vertices[i];
                this.vertices[this.vertexCount++] = new Vertex(new Vector3(v.ToXna(), 0), color);
            }
            this.shapeCount++;
        }



        public static double GetArea(FlatVector[] vertices)
        {
            double totalArea = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                FlatVector a = vertices[i];
                FlatVector b = vertices[(i + 1) % vertices.Length];
                totalArea = Math.FusedMultiplyAdd(b.X - a.X, b.Y + a.Y, totalArea);
            }
            return FlatMath.Div2(Math.Abs(totalArea));
        }
        public static FlatVector GetCenter(FlatVector[] vertices)
        {
            FlatVector sum = FlatVector.Zero;
            for (int i = 0; i < vertices.Length; i++) sum += vertices[i];
            return sum / vertices.Length;
        }


        //public void DrawIrregularPolygonFillExp(FlatVector[] vertices, FlatTransform transform, Color color)
        //{
        //    this.ConfirmStart();

        //    if (vertices == null) throw new ArgumentNullException(nameof(vertices));
        //    else if (vertices.Length < 3) throw new ArgumentOutOfRangeException(nameof(vertices));

        //    int[] indices = Shapes.Triangulate(vertices);
        //    if (indices == null) throw new ArgumentNullException(nameof(indices));
        //    else if (indices.Length < 3) throw new ArgumentOutOfRangeException(nameof(indices));

        //    this.ConfirmEnoughSpace(vertices.Length, indices.Length);

        //    for (int i = 0; i < indices.Length; i++) this.indices[this.currIndexI++] = indices[i] + this.currVertexI;

        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        double x = vertices[i].X;
        //        double y = vertices[i].Y;
        //        FlatVector.Transform(ref x, ref y, transform);
        //        this.vertices[this.currVertexI++] = new Vertex(x, y, color).ToXna();
        //    }
        //    this.shapeCount++;
        //}

        //public void DrawTriangulatedPolygon(FlatVector[] vertices, FlatTransform transform, double thickness, Color color)
        //{
        //    int[] triangles = Shapes.Triangulate(vertices);

        //    for (int i = 0; i < triangles.Length; i += 3)
        //    {
        //        int a = triangles[i];
        //        int b = triangles[i + 1];
        //        int c = triangles[i + 2];

        //        Vector2 A = vertices[a];
        //        Vector2 B = vertices[b];
        //        Vector2 C = vertices[c];

        //        A = FlatUtil.Transform(A, transform);
        //        B = FlatUtil.Transform(B, transform);
        //        C = FlatUtil.Transform(C, transform);

        //        this.DrawLine(A, B, thickness, color);
        //        this.DrawLine(B, C, thickness, color);
        //        this.DrawLine(C, A, thickness, color);
        //    }
        //}

        //public void DrawVertices(FlatVector[] vertices, FlatTransform transform, double thickness, Color color)
        //{
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        double x = vertices[i].X;
        //        double y = vertices[i].Y;
        //        FlatVector.Transform(ref x, ref y, transform);
        //        this.vertices[this.currVertexI++] = new Vertex(x, y, color).ToXna();
        //    }
        //}
        //private static int[] Triangulate(FlatVector[] vertices)
        //{
        //    List<int> indexList = new List<int>();
        //    for (int i = 0; i < vertices.Length; i++) indexList.Add(i);

        //    int triangleTotalCount = vertices.Length - 2;
        //    int triangleTotalIndexCount = triangleTotalCount * 3;

        //    int[] triangles = new int[triangleTotalIndexCount];
        //    int triangleIndexCount = 0;

        //    while (indexList.Count > 3)
        //    {
        //        for (int i = 0; i < indexList.Count; i++)
        //        {
        //            int a = indexList[i];
        //            int b = FlatUtil.GetItem(indexList, i - 1);
        //            int c = FlatUtil.GetItem(indexList, i + 1);

        //            FlatVector A = vertices[a];
        //            FlatVector B = vertices[b];
        //            FlatVector C = vertices[c];

        //            FlatVector AB = B - A;
        //            FlatVector AC = C - A;

        //            //double Ax = vertices[a].X;
        //            //double Ay = vertices[a].Y;
        //            //double Bx = vertices[b].X;
        //            //double By = vertices[b].Y;
        //            //double Cx = vertices[c].X;
        //            //double Cy = vertices[c].Y;

        //            //double ABx = Bx - Ax;
        //            //double ABy = By - Ay;
        //            //double ACx = Cx - Ax;
        //            //double ACy = Cy - Ay;

        //            if ((AB / AC) < 0) continue; // is ear convex?

        //            bool isEar = true;
        //            for (int j = 0; j < vertices.Length; j++)
        //            {
        //                if (j == a || j == b || j == c) continue; // skip points that compose ear

        //                double crossA = (A - B) / (vertices[j] - B);
        //                double crossB = (C - A) / (vertices[j] - A);
        //                double crossC = (B - C) / (vertices[j] - C);

        //                if (crossA <= 0 && crossB <= 0 && crossC <= 0) // does ear contain an other point?
        //                {
        //                    isEar = false;
        //                    break;
        //                }
        //            }
        //            if (isEar)
        //            {
        //                foreach (int index in new int[] { b, a, c }) triangles[triangleIndexCount++] = index;
        //                indexList.RemoveAt(i);
        //                break;
        //            }
        //        }
        //    }
        //    for (int i = 0; i < 3; i++) triangles[triangleIndexCount++] = indexList[i];
        //    return triangles;
        //}
    }
}
