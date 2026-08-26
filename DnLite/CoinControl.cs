using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinControl
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public enum CoinSide
    {
        Heads,
        Tails
    }
    public class CoinFlipViewer : UserControl
    {
        private readonly Timer _animationTimer = new Timer();
        private float _rotation;
        private float _startRotation;
        private float _endRotation;
        private float _animationProgress;
        private float _durationSeconds = 1.2f;
        private DateTime _startTime;
        private bool _isAnimating;
        private bool _displayResult;
        private CoinSide _result;
        private float _speedMultiplier = 1.0f;
        private int _totalFrames;
        private int _currentFrame;
        public event Action<CoinSide> FlipCompleted;

        public CoinFlipViewer()
        {
            DoubleBuffered = true;
            _animationTimer.Interval = 16; // ~60fps
            _animationTimer.Tick += AnimationTimer_Tick;
            BackColor = Color.Transparent;
            Size = new Size(180, 180);

            // Initialize coin to show as full coin facing user
            InitializeReadyState();
        }

        public void InitializeReadyState()
        {
            _rotation = 90f; // Coin facing user (full visible)
            _isAnimating = false;
            _displayResult = false; // Show grey coin, not result
            _animationTimer.Stop();
            Invalidate(); // Trigger repaint
        }
        public void Flip(CoinSide finalSide, float animationDuration = 1.2f, float speedMul = 1.0f)
        {
            if (_isAnimating) return; // Prevent overlapping animations

            _result = finalSide;
            _durationSeconds = Math.Max(0.3f, animationDuration);
            _speedMultiplier = Math.Max(0.1f, speedMul);
            _displayResult = false; // Start hiding result while flipping

            // Randomly choose starting position (Heads or Tails)
            Random rng = new Random();
            CoinSide startingSide = (rng.Next(2) == 0) ? CoinSide.Heads : CoinSide.Tails;

            // Set starting rotation based on random starting side
            // 90° = Heads (full coin facing user)
            // 270° = Tails (full coin facing user, different face)
            _startRotation = (startingSide == CoinSide.Heads) ? 90f : 270f;
            _rotation = _startRotation;

            // Calculate end rotation: coin always ends facing user
            // Target final angles (absolute):
            //   - Heads = 90° (sin(90°) = 1, displays as full coin, shows "H")
            //   - Tails = 270° (sin(270°) = -1, abs = 1, displays as full coin, shows "T")
            float targetFinalAngle = (finalSide == CoinSide.Heads) ? 90f : 270f;
            int fullSpins = 8; // 8 full rotations before landing

            // Calculate end rotation: do 8 full spins, then land on target angle
            // This ensures we always end at the correct angle (90 or 270)
            _endRotation = (fullSpins * 360f) + targetFinalAngle;

            _startTime = DateTime.Now;
            _isAnimating = true;
            _currentFrame = 0;
            _totalFrames = (int)(_durationSeconds * 60f); // Approximate frame count
            _animationTimer.Start();
            Invalidate();
        }
        public void StopAnimation()
        {
            _animationTimer.Stop();
            _isAnimating = false;
        }
        public CoinSide CurrentVisibleSide
        {
            get
            {
                // Calculate which side is currently visible based on rotation
                float normalizedRotation = (_rotation % 360f);
                if (normalizedRotation < 0) normalizedRotation += 360f;
                return (normalizedRotation < 180f) ? CoinSide.Heads : CoinSide.Tails;
            }
        }
        public bool IsAnimating => _isAnimating;

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = (float)(DateTime.Now - _startTime).TotalSeconds;
            _animationProgress = Math.Min(elapsed / _durationSeconds, 1f);

            // Ease-in-out cubic interpolation for smooth acceleration/deceleration
            float t = _animationProgress;
            float eased = t < 0.5f 
                ? 4f * t * t * t 
                : (float)(1f - Math.Pow(-2f * t + 2f, 3f) / 2f);

            _rotation = _startRotation + (_endRotation - _startRotation) * eased;

            Invalidate();
            _currentFrame++;

            // Animation complete
            if (_animationProgress >= 1f)
            {
                _animationTimer.Stop();
                _isAnimating = false;
                _displayResult = true; // Show result (colored) when animation ends
                _rotation = _endRotation;

                // Fire completion event with final result
                FlipCompleted?.Invoke(_result);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Don't clear background - let transparency show through
            // g.Clear(BackColor);

            var cx = ClientSize.Width / 2f;
            var cy = ClientSize.Height * 0.35f;  // Position coin in upper portion
            var baseRadius = Math.Min(ClientSize.Width, ClientSize.Height) * 0.34f;

            // Calculate perspective effect based on rotation
            // As coin rotates from Heads (0°) to Tails (180°), height scales from full to thin
            float normalizedRotation = (_rotation % 360f);
            if (normalizedRotation < 0) normalizedRotation += 360f;

            // Create perspective: coin height varies as it spins
            float perspectiveScale = Math.Abs((float)Math.Sin(normalizedRotation * Math.PI / 180f));
            float coinHeight = baseRadius * 2f * perspectiveScale;

            // Edge case: very thin coin when nearly edge-on
            if (perspectiveScale < 0.05f)
                perspectiveScale = 0.05f;

            GraphicsState state = g.Save();
            g.TranslateTransform(cx, cy);

            // Draw outer rim (always visible)
            using (var rimPen = new Pen(Color.WhiteSmoke, 2f))
            {
                g.DrawEllipse(rimPen, -baseRadius, -coinHeight / 2f, baseRadius * 2f, coinHeight);
            }

            // Determine which face to draw
            CoinSide visibleSide = CurrentVisibleSide;

            // Use neutral gray color for ready state
            // Use colored faces (blue/red) when animating or after flip with result
            Color faceColor;
            string label;

            if (_isAnimating || _displayResult)
            {
                // During animation or showing result: show colored faces with H/T
                faceColor = visibleSide == CoinSide.Heads 
                    ? Color.FromArgb(220, 240, 255)  // Light blue for Heads
                    : Color.FromArgb(255, 220, 220); // Light red for Tails
                label = visibleSide == CoinSide.Heads ? "H" : "T";
            }
            else
            {
                // Ready state: neutral gray coin, no label
                faceColor = Color.FromArgb(200, 200, 200); // Neutral gray
                label = "";
            }

            // Draw face with perspective
            if (perspectiveScale > 0.1f)
            {
                using (var faceBrush = new SolidBrush(faceColor))
                {
                    float innerRadius = baseRadius - 4f;
                    g.FillEllipse(faceBrush, -innerRadius, -coinHeight / 2f + 2f, innerRadius * 2f, coinHeight - 4f);
                }

                // Draw decorative edge seam
                using (var seamPen = new Pen(Color.FromArgb(100, 100, 100), 1f))
                {
                    float seamRadius = baseRadius * 0.8f;
                    g.DrawEllipse(seamPen, -seamRadius, -coinHeight / 2f * 0.8f, seamRadius * 2f, coinHeight * 0.8f);
                }

                // Draw text label (Heads/Tails) only during animation
                if (!string.IsNullOrEmpty(label))
                {
                    float fontSize = baseRadius * (perspectiveScale > 0.3f ? 0.4f : 0.25f);

                    using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(Color.FromArgb(30, 30, 40)))
                    {
                        var textSize = g.MeasureString(label, font);
                        g.DrawString(label, font, textBrush,
                            -textSize.Width / 2f, -textSize.Height / 2f);
                    }
                }
            }
            else
            {
                // Coin is edge-on: draw thin line
                using (var edgePen = new Pen(Color.FromArgb(150, 150, 150), 2f))
                {
                    g.DrawLine(edgePen, -baseRadius, 0, baseRadius, 0);
                }
            }

            g.Restore(state);

            // Draw outer frame (wireframe aesthetic)
            using (var framePen = new Pen(Color.FromArgb(80, 255, 255, 255), 1f))
            {
                g.DrawRectangle(framePen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            }
        }
    }
}