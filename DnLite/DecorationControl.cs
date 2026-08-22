using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace DnLite
{
    // Control that displays an image for decoration tokens. Inherits TokenControl so it carries GridWidth/GridHeight and Tag.
    public class DecorationControl : TokenControl
    {
        private Image _image;
        private string _imagePath;
        private bool _isAnimatedGif;
        private MemoryStream _imageStream; // Keep the stream alive for GIFs

        public new string ImagePath
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
            // Stop animation for previous image
            if (_isAnimatedGif && _image != null)
            {
                ImageAnimator.StopAnimate(_image, OnFrameChanged);
            }

            // Dispose previous image and stream
            try
            {
                _image?.Dispose();
                _image = null;
                _imageStream?.Dispose();
                _imageStream = null;
                _isAnimatedGif = false;

                if (!string.IsNullOrEmpty(_imagePath))
                {
                    // Resolve relative paths relative to the application directory
                    string fullPath = _imagePath;
                    if (!Path.IsPathRooted(_imagePath))
                    {
                        // Get the application's base directory
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        fullPath = Path.Combine(baseDir, _imagePath);
                    }

                    System.Diagnostics.Debug.WriteLine($"DecorationControl: Attempting to load image from: {fullPath}");
                    System.Diagnostics.Debug.WriteLine($"DecorationControl: File exists: {File.Exists(fullPath)}");

                    if (File.Exists(fullPath))
                    {
                        // Read the entire file into memory to avoid file locking
                        // and keep the data available for GIF animation
                        byte[] imageData = File.ReadAllBytes(fullPath);
                        _imageStream = new MemoryStream(imageData);
                        _imageStream.Position = 0;

                        // Create image from the memory stream (keep stream alive for GIFs)
                        _image = Image.FromStream(_imageStream, false, false);

                        // Check if it's an animated GIF
                        if (_image != null && IsAnimatedGif(_image))
                        {
                            _isAnimatedGif = true;
                            ImageAnimator.Animate(_image, OnFrameChanged);
                        }
                    }
                }
                }
                catch (Exception ex)
                {
                    // Silently handle errors - corrupted or invalid image files
                    System.Diagnostics.Debug.WriteLine($"DecorationControl: Failed to load image '{Path.GetFileName(_imagePath)}': {ex.Message}");
                    try { _image?.Dispose(); } catch { }
                    try { _imageStream?.Dispose(); } catch { }
                    _image = null;
                    _imageStream = null;
                    _isAnimatedGif = false;
                }
        }

        private bool IsAnimatedGif(Image image)
        {
            // Check if the image format is GIF
            if (image.RawFormat.Guid != ImageFormat.Gif.Guid)
                return false;

            // Check if it has multiple frames (animated)
            var dimension = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(dimension);
            return frameCount > 1;
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            // Update the current frame and invalidate to redraw
            if (_image != null && _isAnimatedGif)
            {
                try
                {
                    ImageAnimator.UpdateFrames(_image);
                    if (!IsDisposed && InvokeRequired)
                    {
                        BeginInvoke(new Action(() => { if (!IsDisposed) Invalidate(); }));
                    }
                    else if (!IsDisposed)
                    {
                        Invalidate();
                    }
                }
                catch
                {
                    // Ignore any errors during animation
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Do not call base.OnPaint - skip TokenControl painting (circle/letter)
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

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
                // Stop animation before disposing
                if (_isAnimatedGif && _image != null)
                {
                    try { ImageAnimator.StopAnimate(_image, OnFrameChanged); } catch { }
                }
                try { _image?.Dispose(); } catch { }
                try { _imageStream?.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }
    }
}
