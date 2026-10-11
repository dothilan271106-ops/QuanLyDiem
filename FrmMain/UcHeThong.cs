using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CodeLogic;
using Database;
using Guna.UI2.WinForms;

namespace UI.UserControls
{
    public class UcHeThong : UserControl
    {
        private Guna2TextBox txtMkCu;
        private Guna2TextBox txtMkMoi;
        private Guna2TextBox txtMkXacNhan;
        private CheckBox chkHienMk;
        private Label lblThongBao;
        private Guna2Button btnDoiMatKhau;
        private Guna2Button btnDangXuat;

        private static readonly Color MauChinh = Color.FromArgb(94, 22, 110);
        private static readonly Color MauNen = Color.FromArgb(250, 243, 253);
        private static readonly Color MauChu = Color.FromArgb(60, 20, 70);

        public UcHeThong()
        {
            XayDungGiaoDien();
        }

        private void XayDungGiaoDien()
        {
            SuspendLayout();
            BackColor = MauNen;
            Padding = new Padding(24, 16, 24, 20);
            Font = new Font("Segoe UI", 9.5f);

            var lblTieuDe = new Label
            {
                Text = "Quản trị hệ thống",
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var user = PhienLamViec.NguoiDung;
            string tenDangNhap = user != null ? user.TenDangNhap : "admin";
            string hoTen = user != null ? user.HoTen : "Quản trị viên";
            string vaiTro = user != null ? user.VaiTro : "Admin";

            // Main layout panel
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            // ====== CARD 1: THÔNG TIN TÀI KHOẢN ======
            var cardInfo = new Guna2Panel
            {
                Width = 650,
                Height = 160,
                FillColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 16)
            };

            var lblHeaderInfo = new Label
            {
                Text = "THÔNG TIN TÀI KHOẢN HIỆN TẠI",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI Semibold", 11.5f),
                ForeColor = MauChinh
            };

            var lblChiTiet = new Label
            {
                Dock = DockStyle.Fill,
                Text = string.Format("• Tên đăng nhập: {0}\n• Họ và tên: {1}\n• Vai trò quyền hạn: {2}\n• Trạng thái: Đang hoạt động", tenDangNhap, hoTen, vaiTro),
                Font = new Font("Segoe UI", 10f),
                ForeColor = MauChu,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cardInfo.Controls.Add(lblChiTiet);
            cardInfo.Controls.Add(lblHeaderInfo);

            // ====== CARD 2: ĐỔI MẬT KHẨU ======
            var cardDoiMk = new Guna2Panel
            {
                Width = 650,
                Height = 350,
                FillColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 16)
            };

            var lblHeaderDoiMk = new Label
            {
                Text = "ĐỔI MẬT KHẨU ĐĂNG NHẬP",
                Location = new Point(20, 16),
                Size = new Size(610, 26),
                Font = new Font("Segoe UI Semibold", 11.5f),
                ForeColor = MauChinh
            };

            var lbl1 = new Label { Text = "Mật khẩu hiện tại:", Location = new Point(20, 52), Size = new Size(180, 22), ForeColor = MauChu };
            txtMkCu = new Guna2TextBox
            {
                Location = new Point(20, 76),
                Size = new Size(380, 38),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                PasswordChar = '●',
                Font = new Font("Segoe UI", 9.5f)
            };

            var lbl2 = new Label { Text = "Mật khẩu mới (ít nhất 6 ký tự):", Location = new Point(20, 122), Size = new Size(240, 22), ForeColor = MauChu };
            txtMkMoi = new Guna2TextBox
            {
                Location = new Point(20, 146),
                Size = new Size(380, 38),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                PasswordChar = '●',
                Font = new Font("Segoe UI", 9.5f)
            };

            var lbl3 = new Label { Text = "Xác nhận mật khẩu mới:", Location = new Point(20, 192), Size = new Size(200, 22), ForeColor = MauChu };
            txtMkXacNhan = new Guna2TextBox
            {
                Location = new Point(20, 216),
                Size = new Size(380, 38),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                PasswordChar = '●',
                Font = new Font("Segoe UI", 9.5f)
            };

            chkHienMk = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Location = new Point(415, 84),
                AutoSize = true,
                ForeColor = Color.FromArgb(120, 100, 130),
                Font = new Font("Segoe UI", 9f)
            };
            chkHienMk.CheckedChanged += (s, e) =>
            {
                char c = chkHienMk.Checked ? '\0' : '●';
                txtMkCu.PasswordChar = c;
                txtMkMoi.PasswordChar = c;
                txtMkXacNhan.PasswordChar = c;
            };

            lblThongBao = new Label
            {
                Location = new Point(20, 260),
                Size = new Size(610, 22),
                ForeColor = Color.FromArgb(200, 40, 50),
                Font = new Font("Segoe UI", 9f)
            };

            btnDoiMatKhau = new Guna2Button
            {
                Text = "Cập nhật mật khẩu",
                Location = new Point(20, 288),
                Size = new Size(180, 40),
                BorderRadius = 8,
                FillColor = MauChinh,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Cursor = Cursors.Hand
            };
            btnDoiMatKhau.Click += BtnDoiMatKhau_Click;

            cardDoiMk.Controls.Add(lblHeaderDoiMk);
            cardDoiMk.Controls.Add(lbl1);
            cardDoiMk.Controls.Add(txtMkCu);
            cardDoiMk.Controls.Add(lbl2);
            cardDoiMk.Controls.Add(txtMkMoi);
            cardDoiMk.Controls.Add(lbl3);
            cardDoiMk.Controls.Add(txtMkXacNhan);
            cardDoiMk.Controls.Add(chkHienMk);
            cardDoiMk.Controls.Add(lblThongBao);
            cardDoiMk.Controls.Add(btnDoiMatKhau);

            // ====== CARD 3: ĐĂNG XUẤT ======
            var cardLogout = new Guna2Panel
            {
                Width = 650,
                Height = 100,
                FillColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 16)
            };

            btnDangXuat = new Guna2Button
            {
                Text = "Đăng xuất khỏi hệ thống",
                Location = new Point(20, 28),
                Size = new Size(220, 42),
                BorderRadius = 8,
                FillColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Cursor = Cursors.Hand
            };
            btnDangXuat.Click += (s, e) =>
            {
                var f = FindForm();
                if (f is FrmMain main)
                {
                    var btnDx = main.Controls.Find("btnDangXuat", true);
                    if (btnDx.Length > 0 && btnDx[0] is Button b)
                    {
                        b.PerformClick();
                    }
                    else
                    {
                        f.Close();
                    }
                }
            };

            cardLogout.Controls.Add(btnDangXuat);

            flow.Controls.Add(cardInfo);
            flow.Controls.Add(cardDoiMk);
            flow.Controls.Add(cardLogout);

            Controls.Add(flow);
            Controls.Add(lblTieuDe);

            ResumeLayout(false);
        }

        private void BtnDoiMatKhau_Click(object sender, EventArgs e)
        {
            string cu = txtMkCu.Text ?? "";
            string moi = txtMkMoi.Text ?? "";
            string xacNhan = txtMkXacNhan.Text ?? "";

            if (string.IsNullOrEmpty(cu) || string.IsNullOrEmpty(moi) || string.IsNullOrEmpty(xacNhan))
            {
                lblThongBao.ForeColor = Color.FromArgb(200, 40, 50);
                lblThongBao.Text = "Vui lòng nhập đầy đủ các ô mật khẩu.";
                return;
            }

            if (moi.Length < 6)
            {
                lblThongBao.ForeColor = Color.FromArgb(200, 40, 50);
                lblThongBao.Text = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return;
            }

            if (moi != xacNhan)
            {
                lblThongBao.ForeColor = Color.FromArgb(200, 40, 50);
                lblThongBao.Text = "Mật khẩu xác nhận không khớp.";
                return;
            }

            string tenDangNhap = PhienLamViec.NguoiDung != null ? PhienLamViec.NguoiDung.TenDangNhap : "admin";

            try
            {
                string hashCu = FrmDangNhap.BamSha256(cu);
                string hashMoi = FrmDangNhap.BamSha256(moi);

                using (var conn = new SqlConnection(DbHelper.ConnStr))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM taikhoan WHERE tendangnhap = @t AND matkhau = @m", conn))
                    {
                        cmd.Parameters.Add("@t", SqlDbType.VarChar, 100).Value = tenDangNhap;
                        cmd.Parameters.Add("@m", SqlDbType.VarChar, 100).Value = hashCu;

                        int dung = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        if (dung == 0)
                        {
                            lblThongBao.ForeColor = Color.FromArgb(200, 40, 50);
                            lblThongBao.Text = "Mật khẩu hiện tại không chính xác.";
                            return;
                        }
                    }

                    using (var cmd = new SqlCommand("UPDATE taikhoan SET matkhau = @m WHERE tendangnhap = @t", conn))
                    {
                        cmd.Parameters.Add("@m", SqlDbType.VarChar, 100).Value = hashMoi;
                        cmd.Parameters.Add("@t", SqlDbType.VarChar, 100).Value = tenDangNhap;
                        cmd.ExecuteNonQuery();
                    }
                }

                txtMkCu.Clear();
                txtMkMoi.Clear();
                txtMkXacNhan.Clear();
                lblThongBao.ForeColor = Color.FromArgb(39, 174, 96);
                lblThongBao.Text = "Đổi mật khẩu thành công!";
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblThongBao.ForeColor = Color.FromArgb(200, 40, 50);
                lblThongBao.Text = "Lỗi đổi mật khẩu: " + ex.Message;
            }
        }
    }
}
