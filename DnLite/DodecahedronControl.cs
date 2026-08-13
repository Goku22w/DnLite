using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DnLite
{
    public class DodecahedronControl : UserControl
    {
        private Timer timer;
        private double angleX = 0, angleY = 0, angleZ = 0;
        private List<Point3D> baseVertices;
        private List<Point3D> vertices;
        private List<(int, int)> edges;
        private int frames = 0;
        private int totalFrames = 240;
        private Random rng = new Random();
        private int lastRoll = 0;
        private float speedMultiplier = 100f;

        public event Action<int> RollCompleted;

        public DodecahedronControl()
        {
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;
            InitializeGeometry();

            timer = new Timer();
            timer.Interval = 16; // ~60fps
            timer.Tick += Timer_Tick;
        }

        public void StartAnimation(int frames = 120, float speedMul = 1f)
        {
            totalFrames = Math.Max(10, frames);
            speedMultiplier = Math.Max(0.1f, speedMul);
            this.frames = 0;
            lastRoll = 0;
            timer.Stop();
            angleX = angleY = angleZ = 0;
            vertices = baseVertices.Select(v => v.Rotate(angleX, angleY, angleZ)).ToList();
            timer.Start();
            Invalidate();
        }

        public void StopAnimation()
        {
            timer.Stop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(this.BackColor);

            if (vertices == null || edges == null) return;

            var pts2 = new List<PointF>(vertices.Count);
            int cx = this.ClientSize.Width / 2;
            int cy = this.ClientSize.Height / 2;

            double scale = Math.Min(this.ClientSize.Width, this.ClientSize.Height) * 0.18;
            double fov = 6.0;

            foreach (var v in vertices)
            {
                double z = v.Z + 4.0;
                double px = v.X * scale / (z / fov) + cx;
                double py = -v.Y * scale / (z / fov) + cy;
                pts2.Add(new PointF((float)px, (float)py));
            }

            using (var pen = new Pen(Color.DarkBlue, 2f))
            {
                foreach (var eidx in edges)
                {
                    var a = pts2[eidx.Item1];
                    var b = pts2[eidx.Item2];
                    g.DrawLine(pen, a, b);
                }
            }

            using (var b = new SolidBrush(Color.Red))
            {
                foreach (var p in pts2)
                {
                    g.FillEllipse(b, p.X - 3, p.Y - 3, 6, 6);
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Rotate faster by multiplying increments
            angleX += 0.05 * speedMultiplier;
            angleY += 0.03 * speedMultiplier;
            angleZ += 0.015 * speedMultiplier;

            vertices = baseVertices.Select(v => v.Rotate(angleX, angleY, angleZ)).ToList();

            this.Invalidate();

            frames++;
            if (frames >= totalFrames)
            {
                timer.Stop();
                lastRoll = rng.Next(1, 21);
                RollCompleted?.Invoke(lastRoll);
            }
        }

        private void InitializeGeometry()
        {
            double phi = (1.0 + Math.Sqrt(5.0)) / 2.0;
            double a = 1.0 / phi;
            double b = phi;

            var verts = new List<Point3D>();
            double[] signs = { -1, 1 };
            foreach (var sx in signs)
                foreach (var sy in signs)
                    foreach (var sz in signs)
                        verts.Add(new Point3D(sx, sy, sz));

            foreach (var s1 in signs)
                foreach (var s2 in signs)
                {
                    verts.Add(new Point3D(0, s1 * a, s2 * b));
                    verts.Add(new Point3D(s1 * a, s2 * b, 0));
                    verts.Add(new Point3D(s1 * b, 0, s2 * a));
                }

            baseVertices = verts.Distinct(new Point3DComparer()).ToList();
            double max = baseVertices.Max(v => Math.Max(Math.Abs(v.X), Math.Max(Math.Abs(v.Y), Math.Abs(v.Z))));
            for (int i = 0; i < baseVertices.Count; i++)
            {
                baseVertices[i] = baseVertices[i].Scale(1.0 / max * 1.2);
            }

            edges = new List<(int, int)>();
            for (int i = 0; i < baseVertices.Count; i++)
            {
                for (int j = i + 1; j < baseVertices.Count; j++)
                {
                    double d = baseVertices[i].DistanceTo(baseVertices[j]);
                    if (d < 1.05 && d > 0.1)
                    {
                        edges.Add((i, j));
                    }
                }
            }

            vertices = baseVertices.Select(v => v.Rotate(angleX, angleY, angleZ)).ToList();
        }

        private class Point3D
        {
            public double X;
            public double Y;
            public double Z;
            public Point3D(double x, double y, double z) { X = x; Y = y; Z = z; }
            public Point3D Scale(double s) => new Point3D(X * s, Y * s, Z * s);
            public double DistanceTo(Point3D o) => Math.Sqrt((X - o.X) * (X - o.X) + (Y - o.Y) * (Y - o.Y) + (Z - o.Z) * (Z - o.Z));
            public Point3D Rotate(double ax, double ay, double az)
            {
                double cx = Math.Cos(ax), sx = Math.Sin(ax);
                double cy = Math.Cos(ay), sy = Math.Sin(ay);
                double cz = Math.Cos(az), sz = Math.Sin(az);
                double x = X, y = Y, z = Z;
                double y1 = y * cx - z * sx;
                double z1 = y * sx + z * cx;
                double x2 = x * cy + z1 * sy;
                double z2 = -x * sy + z1 * cy;
                double x3 = x2 * cz - y1 * sz;
                double y3 = x2 * sz + y1 * cz;
                return new Point3D(x3, y3, z2);
            }
        }

        private class Point3DComparer : IEqualityComparer<Point3D>
        {
            public bool Equals(Point3D a, Point3D b)
            {
                if (a == null || b == null) return false;
                return Math.Abs(a.X - b.X) < 1e-6 && Math.Abs(a.Y - b.Y) < 1e-6 && Math.Abs(a.Z - b.Z) < 1e-6;
            }
            public int GetHashCode(Point3D p) => (p.X.GetHashCode() * 397) ^ p.Y.GetHashCode() ^ p.Z.GetHashCode();
        }
    }
}
