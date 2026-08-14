using System;
using System.Drawing;
using System.Windows.Forms;

namespace DnLite
{
    // Lightweight control that paints itself as a filled circle with a single letter centered inside.
    public class TokenControl : Control
    {
        public char Letter { get; set; }
        public Color FillColor { get; set; }
        public string ImagePath { get; set; }
        // How many grid cells this token occupies horizontally and vertically
        public int GridWidth { get; set; } = 1;
        public int GridHeight { get; set; } = 1;

        public TokenControl(char letter, Color fillColor, string imagePath = "")
        {
            Letter = letter;
            FillColor = fillColor;
            ImagePath = imagePath;

            // Make the control draw itself and reduce flicker
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Check if we have a valid image path and file exists
            bool hasImage = !string.IsNullOrEmpty(ImagePath) && System.IO.File.Exists(ImagePath);

            if (hasImage)
            {
                try
                {
                    // Load the image
                    using (Image img = Image.FromFile(ImagePath))
                    {
                        // Create a circular clipping region
                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddEllipse(0, 0, Width - 1, Height - 1);
                            g.SetClip(path);

                            // Draw the image to fill the circle
                            g.DrawImage(img, 0, 0, Width, Height);

                            // Reset clip
                            g.ResetClip();
                        }

                        // Draw a black border
                        using (Pen p = new Pen(Color.Black, 2f))
                        {
                            g.DrawEllipse(p, 1, 1, Width - 3, Height - 3);
                        }
                    }
                }
                catch
                {
                    // If image loading fails, fall back to the default rendering
                    hasImage = false;
                }
            }

            // If no image or image loading failed, use the default colored circle with letter
            if (!hasImage)
            {
                // Fill ellipse with the configured color
                using (Brush b = new SolidBrush(FillColor))
                {
                    g.FillEllipse(b, 0, 0, Width - 1, Height - 1);
                }

                // Draw a black border
                using (Pen p = new Pen(Color.Black, 2f))
                {
                    g.DrawEllipse(p, 1, 1, Width - 3, Height - 3);
                }

                // Draw the letter centered
                if (Letter != '\0')
                {
                    string s = Letter.ToString();
                    // Choose a font size that fits the control
                    float fontSize = Math.Max(8f, Math.Min(Width, Height) / 2.2f);
                    using (Font f = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    using (Brush fb = new SolidBrush(Color.White))
                    {
                        var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(s, f, fb, new RectangleF(0, 0, Width, Height), sf);
                    }
                }
            }
        }
    }
}
