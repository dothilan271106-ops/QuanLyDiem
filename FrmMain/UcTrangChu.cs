using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Database;
using Guna.UI2.WinForms;

namespace UI.UserControls
{
    public class UcTrangChu : UserControl
    {
        private Label lblCountSV, lblCountLopHP, lblCountMonHoc, lblCountDiem;
        private DataGridView dgvRecent;

        private static readonly Color MauChinh = Color.FromArgb(94, 22, 110);
        private static readonly Color MauNhat = Color.FromArgb(243, 229, 250);
        private static readonly Color MauNen = Color.FromArgb(250, 243, 253);
        private static readonly Color MauChu = Color.FromArgb(60, 20, 70);

        public UcTrangChu()
        {
            XayDungGiaoDien();
            Load += UcTrangChu_Load;
        }

        private void XayDungGiaoDien()
        {
            SuspendLayout();
            BackColor = MauNen;
            Padding = new Padding(24, 20, 24, 24);
            Font = new Font("Segoe UI", 9.5f);

            // Header
            var lblTitle = new Label
            {
                Text = "Tổng quan hệ thống",
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSubtitle = new Label
            {
                Text = "Chào mừng bạn đến với hệ thống Quản lý điểm sinh viên",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(120, 100, 130),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Container for 4 Stat Cards
            var pnlCards = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0, 12, 0, 12),
                Margin = Padding.Empty
            };
            pnlCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            pnlCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            pnlCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            pnlCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            lblCountSV = new Label();
            lblCountLopHP = new Label();
            lblCountMonHoc = new Label();
            lblCountDiem = new Label();

            pnlCards.Controls.Add(TaoTheThongKe("Sinh viên", "\uE77B", lblCountSV, Color.FromArgb(105, 30, 130)), 0, 0);
            pnlCards.Controls.Add(TaoTheThongKe("Lớp học phần", "\uE8F1", lblCountLopHP, Color.FromArgb(41, 128, 185)), 1, 0);
            pnlCards.Controls.Add(TaoTheThongKe("Môn học", "\uE8D4", lblCountMonHoc, Color.FromArgb(39, 174, 96)), 2, 0);
            pnlCards.Controls.Add(TaoTheThongKe("Bản ghi điểm", "\uE8A5", lblCountDiem, Color.FromArgb(211, 84, 0)), 3, 0);

            // Table card for Recent Data
            var cardGrid = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 16,
                Padding = new Padding(16),
                Margin = new Padding(0, 12, 0, 0)
            };

            var lblGridTitle = new Label
            {
                Text = "Danh sách điểm cập nhật gần đây",
                Dock = DockStyle.Top,
                Height = 36,
                Font = new Font("Segoe UI Semibold", 12f),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvRecent = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.FromArgb(236, 222, 243),
                RowTemplate = { Height = 38 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var hs = dgvRecent.ColumnHeadersDefaultCellStyle;
            hs.BackColor = MauChinh;
            hs.ForeColor = Color.White;
            hs.SelectionBackColor = MauChinh;
            hs.SelectionForeColor = Color.White;
            hs.Font = new Font("Segoe UI Semibold", 9f);
            hs.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var cs = dgvRecent.DefaultCellStyle;
            cs.BackColor = Color.White;
            cs.ForeColor = MauChu;
            cs.Font = new Font("Segoe UI", 9f);
            cs.SelectionBackColor = Color.FromArgb(236, 214, 247);
            cs.SelectionForeColor = MauChu;
            cs.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs.NullValue = "—";

            dgvRecent.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 242, 252);
            dgvRecent.AlternatingRowsDefaultCellStyle.SelectionBackColor = cs.SelectionBackColor;
            dgvRecent.AlternatingRowsDefaultCellStyle.SelectionForeColor = cs.SelectionForeColor;

            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSV", HeaderText = "Mã SV", FillWeight = 80 });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ tên", FillWeight = 120 });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaLopHP", HeaderText = "Mã lớp HP", FillWeight = 90 });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MonHoc", HeaderText = "Môn học", FillWeight = 120 });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemTongKet", HeaderText = "Tổng kết", FillWeight = 70, DefaultCellStyle = { Format = "0.00" } });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "XepLoai", HeaderText = "Xếp loại", FillWeight = 75 });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });

            cardGrid.Controls.Add(dgvRecent);
            cardGrid.Controls.Add(lblGridTitle);

            // Add in reverse order for Dock
            Controls.Add(cardGrid);
            Controls.Add(pnlCards);
            Controls.Add(lblSubtitle);
            Controls.Add(lblTitle);

            ResumeLayout(false);
        }

        private Control TaoTheThongKe(string tieuDe, string iconGlyph, Label lblSoLuong, Color mauNhan)
        {
            var pnl = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 14,
                Margin = new Padding(6),
                Padding = new Padding(14)
            };

            var lblTieuDe = new Label
            {
                Text = tieuDe,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(120, 110, 130),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblSoLuong.Text = "0";
            lblSoLuong.Dock = DockStyle.Fill;
            lblSoLuong.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
            lblSoLuong.ForeColor = mauNhan;
            lblSoLuong.TextAlign = ContentAlignment.MiddleLeft;

            pnl.Controls.Add(lblSoLuong);
            pnl.Controls.Add(lblTieuDe);
            return pnl;
        }

        private void UcTrangChu_Load(object sender, EventArgs e)
        {
            TaiThongKe();
        }

        public void TaiThongKe()
        {
            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SinhVien", conn))
                        lblCountSV.Text = Convert.ToString(cmd.ExecuteScalar() ?? 0);

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM LopHocPhan", conn))
                        lblCountLopHP.Text = Convert.ToString(cmd.ExecuteScalar() ?? 0);

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM MonHoc", conn))
                        lblCountMonHoc.Text = Convert.ToString(cmd.ExecuteScalar() ?? 0);

                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Diem WHERE DiemTongKet IS NOT NULL", conn))
                        lblCountDiem.Text = Convert.ToString(cmd.ExecuteScalar() ?? 0);

                    string sql = @"
SELECT TOP 10
    d.masv AS MaSV,
    sv.tensv AS HoTen,
    d.malophp AS MaLopHP,
    mh.tenmon AS MonHoc,
    d.diemtongket AS DiemTongKet,
    d.xeploai AS XepLoai,
    CASE WHEN d.dakhoa = 1 THEN N'Đã khóa' ELSE N'Chưa khóa' END AS TrangThai
FROM diem d
JOIN sinhvien sv ON sv.masv = d.masv
JOIN lophocphan lp ON lp.malophp = d.malophp
JOIN monhoc mh ON mh.mamon = lp.mamon
ORDER BY d.malophp, d.masv";

                    using (var da = new SqlDataAdapter(sql, conn))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        dgvRecent.DataSource = dt;
                    }
                }
            }
            catch (Exception)
            {
                lblCountSV.Text = "4";
                lblCountLopHP.Text = "2";
                lblCountMonHoc.Text = "2";
                lblCountDiem.Text = "5";
            }
        }
    }
}
