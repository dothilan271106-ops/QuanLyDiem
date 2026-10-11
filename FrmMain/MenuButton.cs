using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace UI.Controls
{
    /// <summary>Nút menu sidebar: icon + chữ, bo góc, trạng thái Active/Hover.</summary>
    public class MenuButton : Button
    {
        private bool _active, _hover;
        private static readonly Font IconFont = new Font("Segoe MDL2 Assets", 15f);
        private static readonly Font TextFont = new Font("Segoe UI", 11f);

        /// <summary>Ký tự icon (Segoe MDL2 Assets), vd "\uE80F".</summary>
        public string IconGlyph { get; set; }

        public bool Active
        {
            get { return _active; }
            set { _active = value; Invalidate(); }
        }

        public MenuButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Height = 54;
            ForeColor = Color.White;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Parent != null ? Parent.BackColor : BackColor);

            var rect = new Rectangle(14, 4, Width - 28, Height - 8);
            using (var path = RoundRect(rect, 10))
            {
                if (_active)
                {
                    using (var br = new LinearGradientBrush(rect, Color.FromArgb(178, 48, 140),
                                                            Color.FromArgb(150, 38, 120), 0f))
                        g.FillPath(br, path);
                }
                else if (_hover)
                {
                    using (var br = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                        g.FillPath(br, path);
                }
            }

            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
            using (var br = new SolidBrush(ForeColor))
            {
                var iconRect = new RectangleF(rect.X + 14, rect.Y, 30, rect.Height);
                var textRect = new RectangleF(rect.X + 52, rect.Y, rect.Width - 60, rect.Height);
                if (!string.IsNullOrEmpty(IconGlyph)) g.DrawString(IconGlyph, IconFont, br, iconRect, sf);
                g.DrawString(Text, TextFont, br, textRect, sf);
            }
        }

        private static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
