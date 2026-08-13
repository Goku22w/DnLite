using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DnLite
{
    // Control that displays an image for decoration tokens. Inherits TokenControl so it carries GridWidth/GridHeight and Tag.
    public class DecorationControl : TokenControl
    {
        private Image _image;
        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                LoadImage();
                Invalidate();
            }
        }

        public DecorationControl(string imagePath, int gridW = 1, int gridH = 1) : base('\0', Color.Transparent)
        {
            GridWidth = Math.Max(1, gridW);
            GridHeight = Math.Max(1, gridH);
            ImagePath = imagePath;

            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        private void LoadImage()
        {
            // Dispose previous image if any
            try
            {
                _image?.Dispose();
                _image = null;

                if (!string.IsNullOrEmpty(_imagePath) && File.Exists(_imagePath))
                {
                    // Use FromFile to lock file; better to clone to avoid file lock
                    using (var img = Image.FromFile(_imagePath))
                    {
                        _image = new Bitmap(img);
                    }
                }
            }
            catch
            {
                try { _image?.Dispose(); } catch { }
                _image = null;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Do not call base.OnPaint - skip TokenControl painting (circle/letter)
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (_image != null)
            {
                // Calculate destination rect preserving aspect ratio and centered
                var dest = GetImageDestRect(_image, ClientRectangle);
                g.DrawImage(_image, dest);
            }
            else
            {
                // If no image, draw a placeholder rectangle
                using (Brush b = new SolidBrush(Color.DarkGray))
                {
                    g.FillRectangle(b, 0, 0, Width, Height);
                }
                using (Pen p = new Pen(Color.Black, 1f))
                {
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        private Rectangle GetImageDestRect(Image img, Rectangle destBounds)
        {
            int iw = img.Width;
            int ih = img.Height;
            int w = destBounds.Width;
            int h = destBounds.Height;

            float imageRatio = (float)iw / ih;
            float destRatio = (float)w / h;

            int dw, dh;
            if (imageRatio > destRatio)
            {
                // image is wider
                dw = w;
                dh = (int)(w / imageRatio);
            }
            else
            {
                dh = h;
                dw = (int)(h * imageRatio);
            }

            int dx = destBounds.X + (w - dw) / 2;
            int dy = destBounds.Y + (h - dh) / 2;
            return new Rectangle(dx, dy, dw, dh);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _image?.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }
    }
}
