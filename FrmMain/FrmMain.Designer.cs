using System.Drawing;
using System.Windows.Forms;
using UI.Controls;

namespace UI
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlSidebar = new Panel();
            this.pnlMenu = new Panel();
            this.btnTongQuan = new MenuButton();
            this.btnLopHocPhan = new MenuButton();
            this.btnQuanLyDiem = new MenuButton();
            this.btnBaoCao = new MenuButton();
            this.btnQuanTri = new MenuButton();
            this.pnlLogout = new Panel();
            this.btnDangXuat = new MenuButton();
            this.pnlLine = new Panel();
            this.pnlLogo = new Panel();
            this.lblLogoIcon = new Label();
            this.lblLogo = new Label();
            this.pnlMain = new Panel();
            this.pnlNoiDung = new Panel();
            this.pnlHeader = new Panel();
            this.lblTieuDe = new Label();
            this.lblXinChao = new Label();
            this.picAvatar = new PictureBox();

            var sidebarColor = Color.FromArgb(74, 12, 82);
            var pageColor = Color.FromArgb(252, 246, 251);

            this.pnlSidebar.SuspendLayout();
            this.pnlMenu.SuspendLayout();
            this.pnlLogout.SuspendLayout();
            this.pnlLogo.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.picAvatar).BeginInit();
            this.SuspendLayout();

            // ===== pnlSidebar =====
            this.pnlSidebar.BackColor = sidebarColor;
            this.pnlSidebar.Dock = DockStyle.Left;
            this.pnlSidebar.Width = 260;
            this.pnlSidebar.Controls.Add(this.pnlMenu);    // Fill (dock sau cùng)
            this.pnlSidebar.Controls.Add(this.pnlLogout);  // Bottom
            this.pnlSidebar.Controls.Add(this.pnlLogo);    // Top

            // ===== pnlLogo =====
            this.pnlLogo.BackColor = sidebarColor;
            this.pnlLogo.Dock = DockStyle.Top;
            this.pnlLogo.Height = 92;
            this.pnlLogo.Controls.Add(this.lblLogo);
            this.pnlLogo.Controls.Add(this.lblLogoIcon);

            this.lblLogoIcon.Text = "\U0001F393";
            this.lblLogoIcon.Font = new Font("Segoe UI Emoji", 24f);
            this.lblLogoIcon.ForeColor = Color.White;
            this.lblLogoIcon.Location = new Point(14, 20);
            this.lblLogoIcon.Size = new Size(70, 50);
            this.lblLogoIcon.TextAlign = ContentAlignment.MiddleCenter;

            this.lblLogo.Text = "QUẢN LÝ ĐIỂM\r\nSINH VIÊN";
            this.lblLogo.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            this.lblLogo.ForeColor = Color.White;
            this.lblLogo.Location = new Point(86, 20);
            this.lblLogo.Size = new Size(165, 50);
            this.lblLogo.TextAlign = ContentAlignment.MiddleLeft;

            // ===== pnlMenu =====
            this.pnlMenu.BackColor = sidebarColor;
            this.pnlMenu.Dock = DockStyle.Fill;
            this.pnlMenu.Padding = new Padding(0, 6, 0, 0);
            // Thêm theo thứ tự ngược: control thêm sau sẽ nằm trên cùng
            this.pnlMenu.Controls.Add(this.btnQuanTri);
            this.pnlMenu.Controls.Add(this.btnBaoCao);
            this.pnlMenu.Controls.Add(this.btnQuanLyDiem);
            this.pnlMenu.Controls.Add(this.btnLopHocPhan);
            this.pnlMenu.Controls.Add(this.btnTongQuan);

            InitMenuButton(this.btnTongQuan, "btnTongQuan", "Tổng quan", "\uE80F");
            InitMenuButton(this.btnLopHocPhan, "btnLopHocPhan", "Quản lý lớp học phần", "\uE8F1");
            InitMenuButton(this.btnQuanLyDiem, "btnQuanLyDiem", "Quản lý điểm", "\uE8A5");
            InitMenuButton(this.btnBaoCao, "btnBaoCao", "Tổng hợp & báo cáo", "\uE9D9");
            InitMenuButton(this.btnQuanTri, "btnQuanTri", "Quản trị hệ thống", "\uE713");

            // ===== pnlLogout =====
            this.pnlLogout.BackColor = sidebarColor;
            this.pnlLogout.Dock = DockStyle.Bottom;
            this.pnlLogout.Height = 96;
            this.pnlLogout.Padding = new Padding(26, 0, 26, 14);
            this.pnlLogout.Controls.Add(this.btnDangXuat);
            this.pnlLogout.Controls.Add(this.pnlLine);

            this.pnlLine.BackColor = Color.FromArgb(110, 50, 118);
            this.pnlLine.Dock = DockStyle.Top;
            this.pnlLine.Height = 1;

            InitMenuButton(this.btnDangXuat, "btnDangXuat", "Đăng xuất", "\uF3B1");
            this.btnDangXuat.Dock = DockStyle.Fill;

            // ===== pnlMain =====
            this.pnlMain.BackColor = pageColor;
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Controls.Add(this.pnlNoiDung);   // Fill
            this.pnlMain.Controls.Add(this.pnlHeader);    // Top

            // ===== pnlHeader =====
            this.pnlHeader.BackColor = Color.FromArgb(250, 240, 248);
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 86;
            this.pnlHeader.Controls.Add(this.picAvatar);
            this.pnlHeader.Controls.Add(this.lblXinChao);
            this.pnlHeader.Controls.Add(this.lblTieuDe);

            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new Font("Segoe UI", 22f, FontStyle.Bold);
            this.lblTieuDe.ForeColor = Color.FromArgb(90, 20, 95);
            this.lblTieuDe.Location = new Point(28, 18);
            this.lblTieuDe.Text = "Tổng quan";

            this.lblXinChao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.lblXinChao.AutoSize = false;
            this.lblXinChao.Font = new Font("Segoe UI", 11f);
            this.lblXinChao.ForeColor = Color.FromArgb(90, 20, 95);
            this.lblXinChao.Size = new Size(320, 30);        
            this.lblXinChao.Location = new Point(1020-320-80,28);
            this.lblXinChao.TextAlign = ContentAlignment.MiddleRight;
            this.lblXinChao.Text = "Xin chào, Admin";

            this.picAvatar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.picAvatar.Size = new Size(46, 46);         
            this.picAvatar.Location = new Point(1020-46-24, 20);
            this.picAvatar.Paint += new PaintEventHandler(this.picAvatar_Paint);

            // ===== pnlNoiDung =====
            this.pnlNoiDung.BackColor = pageColor;
            this.pnlNoiDung.Dock = DockStyle.Fill;

            // ===== FrmMain =====
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = pageColor;
            this.ClientSize = new Size(1280, 720);
            this.MinimumSize = new Size(1100, 650);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlSidebar);
            this.Font = new Font("Segoe UI", 9f);
            this.Name = "FrmMain";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Quản lý điểm sinh viên";
            this.Load += new System.EventHandler(this.FrmMain_Load);

            this.pnlSidebar.ResumeLayout(false);
            this.pnlMenu.ResumeLayout(false);
            this.pnlLogout.ResumeLayout(false);
            this.pnlLogo.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.picAvatar).EndInit();
            this.ResumeLayout(false);
        }

        private static void InitMenuButton(MenuButton b, string name, string text, string glyph)
        {
            b.Name = name;
            b.Text = text;
            b.IconGlyph = glyph;
            b.Dock = DockStyle.Top;
            b.Height = 54;
            b.TabStop = false;
        }

        private Panel pnlSidebar;
        private Panel pnlLogo;
        private Label lblLogoIcon;
        private Label lblLogo;
        private Panel pnlMenu;
        private MenuButton btnTongQuan;
        private MenuButton btnLopHocPhan;
        private MenuButton btnQuanLyDiem;
        private MenuButton btnBaoCao;
        private MenuButton btnQuanTri;
        private Panel pnlLogout;
        private Panel pnlLine;
        private MenuButton btnDangXuat;
        private Panel pnlMain;
        private Panel pnlHeader;
        private Label lblTieuDe;
        private Label lblXinChao;
        private PictureBox picAvatar;
        private Panel pnlNoiDung;
    }
}
