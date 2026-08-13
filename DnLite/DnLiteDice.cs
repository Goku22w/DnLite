using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnLite
{
    public partial class DnLiteDice : Form
    {
        public int? LastRoll { get; private set; }
        private DodecahedronControl dodecaControl;
        public event Action<int> RollCompleted;
        // When true the dice window will attempt to stay above its owner (the display)
        public bool StayAboveOwner { get; set; } = true;

        private bool hooksInstalled = false;
        public DnLiteDice()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                // Ensure owner hooks are installed as soon as the form is shown so it will remain above the owner
                TryInstallOwnerHooks();
            }
            catch { }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // 0x200 is the Win32 class style constant for CS_NOCLOSE
                const int CS_NOCLOSE = 0x200;

                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }

        private void RoleDieButton_Click(object sender, EventArgs e)
        {
            // Disable button to prevent re-entry
            RoleDieButton.Enabled = false;

            // Create dodecahedron animation control and add to form
            try
            {
                if (dodecaControl == null)
                {
                    dodecaControl = new DodecahedronControl();
                    // place above the button
                    dodecaControl.Location = new System.Drawing.Point(10, 10);
                    dodecaControl.Size = new System.Drawing.Size(this.ClientSize.Width - 20, this.ClientSize.Height - 60);
                    dodecaControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    this.Controls.Add(dodecaControl);
                    dodecaControl.BringToFront();
                }

                // ensure we don't add multiple handlers
                dodecaControl.RollCompleted -= OnRollCompleted;
                dodecaControl.RollCompleted += OnRollCompleted;
                // Start with faster spin: fewer frames and higher speed multiplier
                dodecaControl.StartAnimation(60, 3.5f);
                // Ensure the dice stays above the owner window if requested
                TryInstallOwnerHooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start dodecahedron animation: {ex.Message}");
                RoleDieButton.Enabled = true;
            }
        }

        private void OnRollCompleted(int roll)
        {
            // Unsubscribe to avoid repeated calls
            if (dodecaControl != null) dodecaControl.RollCompleted -= OnRollCompleted;
            LastRoll = roll;
            // Update UI label inside this form instead of MessageBox and do not close the form
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((Action)(() => OnRollCompleted(roll)));
                    return;
                }
                // show result in DiceRollOutputLabel as plain text
                try
                {
                    DiceRollOutputLabel.Text = roll.ToString();
                }
                catch { }
                // re-enable roll button
                try { RoleDieButton.Enabled = true; } catch { }
                // raise external event so other forms can react
                RollCompleted?.Invoke(roll);
            }
            catch { }
        }

        private void TryInstallOwnerHooks()
        {
            try
            {
                if (!StayAboveOwner) return;
                if (this.Owner == null) return;
                if (hooksInstalled) return;

                this.Owner.Activated += Owner_Activated;
                this.Owner.Move += Owner_MoveOrResize;
                this.Owner.Resize += Owner_MoveOrResize;
                hooksInstalled = true;
                // Ensure this window is above owner now
                BringAboveOwner();
            }
            catch { }
        }

        private void Owner_Activated(object sender, EventArgs e)
        {
            try { BringAboveOwner(); } catch { }
        }

        private void Owner_MoveOrResize(object sender, EventArgs e)
        {
            try { BringAboveOwner(); } catch { }
        }

        private void BringAboveOwner()
        {
            // An owned form normally stays above its owner. Reinforce by bringing to front and optionally setting TopMost briefly.
            try
            {
                if (this.Owner != null)
                {
                    this.Owner.BringToFront();
                    this.BringToFront();
                    // briefly toggle TopMost to ensure z-order on some systems
                    bool prev = this.TopMost;
                    this.TopMost = true;
                    this.TopMost = prev;
                }
            }
            catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Unhook events on owner when closing
            try
            {
                if (this.Owner != null && hooksInstalled)
                {
                    this.Owner.Activated -= Owner_Activated;
                    this.Owner.Move -= Owner_MoveOrResize;
                    this.Owner.Resize -= Owner_MoveOrResize;
                }
            }
            catch { }
            base.OnFormClosed(e);
        }

        private void DnLiteDice_Load(object sender, EventArgs e)
        {
            DiceRollOutputLabel.Text = "0";
        }
    }
}
