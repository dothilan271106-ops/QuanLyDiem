using CodeLogic;
using FrmMain;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using UI.Controls;
using UI.UserControls;

namespace UI
{
    /// <summary>Khung chính: sidebar + header + vùng nội dung nạp UserControl.</summary>
    public partial class FrmMain : Form
    {
        private MenuButton _current;

        /// <summary>true nếu người dùng bấm Đăng xuất (Program dùng để quay lại form đăng nhập).</summary>
        public bool DangXuat { get; private set; }

        public FrmMain()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            var user = PhienLamViec.NguoiDung;
            lblXinChao.Text = "Xin chào, " + (user != null ? user.HoTen : "Admin");

            btnTongQuan.Click += Menu_Click;
            btnLopHocPhan.Click += Menu_Click;
            btnQuanLyDiem.Click += Menu_Click;
            btnBaoCao.Click += Menu_Click;
            btnQuanTri.Click += Menu_Click;
            btnDangXuat.Click += btnDangXuat_Click;

            Navigate(btnTongQuan);   // trang mặc định
        }

        private void Menu_Click(object sender, EventArgs e)
        {
            Navigate((MenuButton)sender);
        }

        /// <summary>Đổi trạng thái menu và nạp UserControl tương ứng.</summary>
        private void Navigate(MenuButton btn)
        {
            if (_current == btn) return;

            if (_current != null) _current.Active = false;
            btn.Active = true;
            _current = btn;

            // Mỗi menu -> 1 UserControl tương ứng
            UserControl page;
            switch (btn.Name)
            {
                case "btnTongQuan":
                    page = new UcTrangChu();
                    break;
                case "btnLopHocPhan":
                    page = new UcLopHocPhan();
                    break;
                case "btnQuanLyDiem":
                    page = new UCQuanLyDiemSinhVien();
                    break;
                case "btnBaoCao":
                    page = new UcBaoCao();
                    break;
                case "btnQuanTri":
                    page = new UcHeThong();
                    break;
                default:
                    page = new UcPlaceholder(btn.Text);
                    break;
            }
            ShowPage(page, btn.Text);
        }

        private void ShowPage(UserControl page, string title)
        {
            pnlNoiDung.SuspendLayout();
            while (pnlNoiDung.Controls.Count > 0)   // giải phóng trang cũ
            {
                var old = pnlNoiDung.Controls[0];
                pnlNoiDung.Controls.Remove(old);
                old.Dispose();
            }
            page.Dock = DockStyle.Fill;
            pnlNoiDung.Controls.Add(page);
            pnlNoiDung.ResumeLayout();

            lblTieuDe.Text = title;
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            var rs = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Đăng xuất",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs != DialogResult.Yes) return;

            PhienLamViec.DangXuat();
            DangXuat = true;
            Close();
        }

        // Vẽ avatar tròn + icon người dùng
        private void picAvatar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, picAvatar.Width - 1, picAvatar.Height - 1);
            using (var br = new SolidBrush(Color.FromArgb(170, 45, 130)))
                g.FillEllipse(br, r);
            using (var f = new Font("Segoe MDL2 Assets", 18f))
            using (var br = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString("\uE77B", f, br, r, sf);
        }
    }
}
