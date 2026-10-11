using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Class_lop;
using CodeLogic;
using Database;
using Guna.UI2.WinForms;

namespace UI
{
    public class FrmDangNhap : Form
    {
        private Guna2TextBox txtTenDangNhap;
        private Guna2TextBox txtMatKhau;
        private CheckBox chkHienMatKhau;
        private Label lblThongBao;
        private Guna2Button btnDangNhap;
        private Guna2Button btnThoat;

        private static readonly Color MauChinh = Color.FromArgb(94, 22, 110);
        private static readonly Color MauNen = Color.FromArgb(250, 243, 253);

        public FrmDangNhap()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Đăng nhập hệ thống";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(460, 480);
            BackColor = MauNen;
            Font = new Font("Segoe UI", 9.5f);

            var pnlCard = new Guna2Panel
            {
                Location = new Point(30, 25),
                Size = new Size(385, 390),
                FillColor = Color.White,
                BorderRadius = 16,
                Padding = new Padding(24)
            };

            var picIcon = new Label
            {
                Text = "\U0001F393",
                Font = new Font("Segoe UI Emoji", 32f),
                ForeColor = MauChinh,
                Location = new Point(0, 16),
                Size = new Size(385, 52),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var lblTieuDe = new Label
            {
                Text = "QUẢN LÝ ĐIỂM SINH VIÊN",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = MauChinh,
                Location = new Point(0, 72),
                Size = new Size(385, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var lblPhu = new Label
            {
                Text = "Vui lòng đăng nhập để tiếp tục",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(120, 100, 130),
                Location = new Point(0, 104),
                Size = new Size(385, 24),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            txtTenDangNhap = new Guna2TextBox
            {
                PlaceholderText = "Tên đăng nhập (vd: admin)",
                Location = new Point(30, 140),
                Size = new Size(325, 42),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                FillColor = Color.White,
                ForeColor = Color.FromArgb(60, 20, 70),
                Font = new Font("Segoe UI", 10f),
                Text = "admin"
            };

            txtMatKhau = new Guna2TextBox
            {
                PlaceholderText = "Mật khẩu (vd: 123456)",
                Location = new Point(30, 195),
                Size = new Size(325, 42),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                FillColor = Color.White,
                ForeColor = Color.FromArgb(60, 20, 70),
                Font = new Font("Segoe UI", 10f),
                PasswordChar = '●',
                Text = "123456"
            };

            chkHienMatKhau = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Location = new Point(32, 245),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 80, 110),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9f)
            };
            chkHienMatKhau.CheckedChanged += (s, e) =>
            {
                txtMatKhau.PasswordChar = chkHienMatKhau.Checked ? '\0' : '●';
            };

            lblThongBao = new Label
            {
                Text = "",
                Location = new Point(30, 272),
                Size = new Size(325, 20),
                ForeColor = Color.FromArgb(200, 40, 50),
                Font = new Font("Segoe UI", 8.5f),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            btnDangNhap = new Guna2Button
            {
                Text = "Đăng nhập",
                Location = new Point(30, 298),
                Size = new Size(325, 42),
                BorderRadius = 8,
                FillColor = MauChinh,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10.5f),
                Cursor = Cursors.Hand
            };
            btnDangNhap.Click += BtnDangNhap_Click;

            btnThoat = new Guna2Button
            {
                Text = "Thoát",
                Location = new Point(30, 346),
                Size = new Size(325, 34),
                BorderRadius = 8,
                FillColor = Color.Transparent,
                ForeColor = Color.FromArgb(120, 100, 130),
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand
            };
            btnThoat.Click += (s, e) => Application.Exit();

            txtMatKhau.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnDangNhap_Click(s, e); }
            };
            txtTenDangNhap.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; txtMatKhau.Focus(); }
            };

            pnlCard.Controls.Add(picIcon);
            pnlCard.Controls.Add(lblTieuDe);
            pnlCard.Controls.Add(lblPhu);
            pnlCard.Controls.Add(txtTenDangNhap);
            pnlCard.Controls.Add(txtMatKhau);
            pnlCard.Controls.Add(chkHienMatKhau);
            pnlCard.Controls.Add(lblThongBao);
            pnlCard.Controls.Add(btnDangNhap);
            pnlCard.Controls.Add(btnThoat);

            Controls.Add(pnlCard);
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            string tdn = (txtTenDangNhap.Text ?? "").Trim();
            string mk = txtMatKhau.Text ?? "";

            if (string.IsNullOrEmpty(tdn) || string.IsNullOrEmpty(mk))
            {
                HienLoi("Vui lòng nhập tên đăng nhập và mật khẩu.");
                return;
            }

            try
            {
                string hash = BamSha256(mk);

                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM taikhoan WHERE tendangnhap = @t AND matkhau = @m", conn))
                {
                    cmd.Parameters.Add("@t", SqlDbType.VarChar, 100).Value = tdn;
                    cmd.Parameters.Add("@m", SqlDbType.VarChar, 100).Value = hash;

                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);

                    if (count > 0)
                    {
                        PhienLamViec.DangNhap(new TaiKhoanDTO
                        {
                            TenDangNhap = tdn,
                            HoTen = tdn.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Quản trị viên" : tdn,
                            VaiTro = "Admin"
                        });

                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        HienLoi("Sai tên đăng nhập hoặc mật khẩu.");
                    }
                }
            }
            catch (Exception ex)
            {
                HienLoi("Lỗi kết nối CSDL: " + ex.Message);
            }
        }

        private void HienLoi(string thongBao)
        {
            lblThongBao.Text = thongBao;
        }

        public static string BamSha256(string matkhau)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(matkhau));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
