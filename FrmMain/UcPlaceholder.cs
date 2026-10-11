using System.Drawing;
using System.Windows.Forms;

namespace UI.UserControls
{
    /// <summary>Trang tạm cho các menu chưa làm.</summary>
    public class UcPlaceholder : UserControl
    {
        public UcPlaceholder(string tieuDe)
        {
            BackColor = Color.FromArgb(252, 246, 251);
            Controls.Add(new Label
            {
                Text = tieuDe + " - đang phát triển",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14f),
                ForeColor = Color.Gray
            });
        }
    }
}
