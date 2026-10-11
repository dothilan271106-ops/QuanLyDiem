using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Database;
using Guna.UI2.WinForms;

namespace UI.UserControls
{
    public class UcLopHocPhan : UserControl
    {
        private Guna2TextBox txtTimLop;
        private Guna2TextBox txtTimSV;
        private DataGridView dgvLopHP;
        private DataGridView dgvSV;
        private Label lblTieuDeSV;
        private string maLopHPDangChon = "";
        private string tenMonDangChon = "";

        private static readonly Color MauChinh = Color.FromArgb(94, 22, 110);
        private static readonly Color MauNhat = Color.FromArgb(243, 229, 250);
        private static readonly Color MauNen = Color.FromArgb(250, 243, 253);
        private static readonly Color MauChu = Color.FromArgb(60, 20, 70);

        public UcLopHocPhan()
        {
            XayDungGiaoDien();
            Load += UcLopHocPhan_Load;
        }

        private void XayDungGiaoDien()
        {
            SuspendLayout();
            BackColor = MauNen;
            Padding = new Padding(24, 16, 24, 20);
            Font = new Font("Segoe UI", 9.5f);

            var lblTieuDe = new Label
            {
                Text = "Quản lý lớp học phần",
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Main Split Panel: Top = Lớp học phần, Bottom = Sinh viên của lớp
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 260,
                Panel1MinSize = 180,
                Panel2MinSize = 180
            };

            // ====== PANEL 1: LỚP HỌC PHẦN ======
            var cardLop = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(12)
            };

            var pnlTimLop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = Color.White
            };

            txtTimLop = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Tìm theo mã lớp HP, mã môn, tên môn học, giảng viên...",
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                FillColor = Color.White,
                ForeColor = MauChu,
                Font = new Font("Segoe UI", 9.5f)
            };
            txtTimLop.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TaiDanhSachLop(); } };

            var btnTimLop = new Guna2Button
            {
                Text = "Tìm kiếm",
                Dock = DockStyle.Right,
                Width = 100,
                BorderRadius = 8,
                FillColor = MauChinh,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btnTimLop.Click += (s, e) => TaiDanhSachLop();

            var btnLamMoi = new Guna2Button
            {
                Text = "Làm mới",
                Dock = DockStyle.Right,
                Width = 90,
                BorderRadius = 8,
                FillColor = MauNhat,
                ForeColor = MauChinh,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btnLamMoi.Click += (s, e) => { txtTimLop.Clear(); txtTimSV.Clear(); TaiDanhSachLop(); };

            var spacer1 = new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White };
            var spacer2 = new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White };

            pnlTimLop.Controls.Add(txtTimLop);
            pnlTimLop.Controls.Add(spacer1);
            pnlTimLop.Controls.Add(btnTimLop);
            pnlTimLop.Controls.Add(spacer2);
            pnlTimLop.Controls.Add(btnLamMoi);

            dgvLopHP = TaoBang();
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaLopHP", HeaderText = "Mã lớp HP", FillWeight = 85 });
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaMon", HeaderText = "Mã môn", FillWeight = 75 });
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenMon", HeaderText = "Tên môn học", FillWeight = 140 });
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoTin", HeaderText = "Số tín", FillWeight = 55 });
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GiangVien", HeaderText = "Giảng viên", FillWeight = 110 });
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SiSo", HeaderText = "Sĩ số", FillWeight = 55 });
            dgvLopHP.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng thái", FillWeight = 100 });

            dgvLopHP.SelectionChanged += DgvLopHP_SelectionChanged;

            cardLop.Controls.Add(dgvLopHP);
            cardLop.Controls.Add(pnlTimLop);
            split.Panel1.Controls.Add(cardLop);

            // ====== PANEL 2: SINH VIÊN CỦA LỚP ======
            var cardSV = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(12)
            };

            var pnlHeaderSV = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = Color.White
            };

            lblTieuDeSV = new Label
            {
                Dock = DockStyle.Fill,
                Text = "DANH SÁCH SINH VIÊN TRONG LỚP HỌC PHẦN",
                Font = new Font("Segoe UI Semibold", 10.5f),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtTimSV = new Guna2TextBox
            {
                Dock = DockStyle.Right,
                Width = 240,
                PlaceholderText = "Tìm mã SV, họ tên sinh viên...",
                BorderRadius = 8,
                BorderColor = Color.FromArgb(225, 205, 235),
                FillColor = Color.White,
                ForeColor = MauChu,
                Font = new Font("Segoe UI", 9f)
            };
            txtTimSV.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TaiDanhSachSV(); } };

            var btnTimSV = new Guna2Button
            {
                Text = "Tìm SV",
                Dock = DockStyle.Right,
                Width = 85,
                BorderRadius = 8,
                FillColor = MauChinh,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btnTimSV.Click += (s, e) => TaiDanhSachSV();

            var spacer3 = new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White };

            pnlHeaderSV.Controls.Add(lblTieuDeSV);
            pnlHeaderSV.Controls.Add(spacer3);
            pnlHeaderSV.Controls.Add(txtTimSV);
            pnlHeaderSV.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 6, BackColor = Color.White });
            pnlHeaderSV.Controls.Add(btnTimSV);

            dgvSV = TaoBang();
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "STT", HeaderText = "STT", FillWeight = 45 });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSV", HeaderText = "Mã SV", FillWeight = 75 });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ tên", FillWeight = 120 });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgaySinh", HeaderText = "Ngày sinh", FillWeight = 80 });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LopSH", HeaderText = "Lớp SH", FillWeight = 75 });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemCC", HeaderText = "CC (10%)", FillWeight = 70, DefaultCellStyle = { Format = "0.00" } });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemGK", HeaderText = "GK (30%)", FillWeight = 70, DefaultCellStyle = { Format = "0.00" } });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemCK", HeaderText = "CK (60%)", FillWeight = 70, DefaultCellStyle = { Format = "0.00" } });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemTongKet", HeaderText = "Tổng kết", FillWeight = 75, DefaultCellStyle = { Format = "0.00" } });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "XepLoai", HeaderText = "Xếp loại", FillWeight = 80 });
            dgvSV.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KetQua", HeaderText = "Kết quả", FillWeight = 80 });

            cardSV.Controls.Add(dgvSV);
            cardSV.Controls.Add(pnlHeaderSV);
            split.Panel2.Controls.Add(cardSV);

            Controls.Add(split);
            Controls.Add(lblTieuDe);

            ResumeLayout(false);
        }

        private DataGridView TaoBang()
        {
            var g = new DataGridView
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
                RowTemplate = { Height = 36 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var hs = g.ColumnHeadersDefaultCellStyle;
            hs.BackColor = MauChinh;
            hs.ForeColor = Color.White;
            hs.SelectionBackColor = MauChinh;
            hs.SelectionForeColor = Color.White;
            hs.Font = new Font("Segoe UI Semibold", 9f);
            hs.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var cs = g.DefaultCellStyle;
            cs.BackColor = Color.White;
            cs.ForeColor = MauChu;
            cs.Font = new Font("Segoe UI", 9f);
            cs.SelectionBackColor = Color.FromArgb(236, 214, 247);
            cs.SelectionForeColor = MauChu;
            cs.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs.NullValue = "—";

            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 242, 252);
            g.AlternatingRowsDefaultCellStyle.SelectionBackColor = cs.SelectionBackColor;
            g.AlternatingRowsDefaultCellStyle.SelectionForeColor = cs.SelectionForeColor;
            return g;
        }

        private void UcLopHocPhan_Load(object sender, EventArgs e)
        {
            TaiDanhSachLop();
        }

        private void TaiDanhSachLop()
        {
            try
            {
                string kw = (txtTimLop.Text ?? "").Trim();
                string sql = @"
SELECT l.malophp AS MaLopHP,
       l.mamon AS MaMon,
       m.tenmon AS TenMon,
       m.sotin AS SoTin,
       l.giangvien AS GiangVien,
       COUNT(d.masv) AS SiSo,
       CASE 
           WHEN COUNT(d.masv) = 0 THEN N'Chưa có SV'
           WHEN ISNULL(MIN(CAST(d.dakhoa AS INT)), 0) = 1 THEN N'Đã khoá điểm'
           ELSE N'Đang mở'
       END AS TrangThai
FROM lophocphan l
JOIN monhoc m ON m.mamon = l.mamon
LEFT JOIN diem d ON d.malophp = l.malophp
WHERE (@kw = '' OR l.malophp LIKE @likeKw OR l.mamon LIKE @likeKw OR m.tenmon LIKE @likeKw OR l.giangvien LIKE @likeKw)
GROUP BY l.malophp, l.mamon, m.tenmon, m.sotin, l.giangvien
ORDER BY l.malophp";

                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", kw);
                    cmd.Parameters.AddWithValue("@likeKw", "%" + kw + "%");

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        dgvLopHP.DataSource = dt;
                    }
                }

                if (dgvLopHP.Rows.Count > 0)
                {
                    dgvLopHP.Rows[0].Selected = true;
                    DgvLopHP_SelectionChanged(null, null);
                }
                else
                {
                    maLopHPDangChon = "";
                    tenMonDangChon = "";
                    lblTieuDeSV.Text = "Không có lớp học phần nào phù hợp";
                    dgvSV.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lớp học phần: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvLopHP_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLopHP.CurrentRow == null) return;

            maLopHPDangChon = Convert.ToString(dgvLopHP.CurrentRow.Cells[0].Value);
            tenMonDangChon = Convert.ToString(dgvLopHP.CurrentRow.Cells[2].Value);

            lblTieuDeSV.Text = string.Format("SINH VIÊN LỚP HỌC PHẦN: {0} - {1}", maLopHPDangChon, tenMonDangChon);
            TaiDanhSachSV();
        }

        private void TaiDanhSachSV()
        {
            if (string.IsNullOrEmpty(maLopHPDangChon))
            {
                dgvSV.DataSource = null;
                return;
            }

            try
            {
                string kw = (txtTimSV.Text ?? "").Trim();
                string sql = @"
SELECT ROW_NUMBER() OVER (ORDER BY sv.masv) AS STT,
       sv.masv AS MaSV,
       sv.tensv AS HoTen,
       CONVERT(VARCHAR(10), sv.ngaysinh, 103) AS NgaySinh,
       sv.malp AS LopSH,
       d.diemcc AS DiemCC,
       d.diemgk AS DiemGK,
       d.diemck AS DiemCK,
       d.diemtongket AS DiemTongKet,
       d.xeploai AS XepLoai,
       CASE 
           WHEN d.diemtongket IS NULL THEN N'Chưa tổng kết'
           WHEN d.diemtongket >= 5.0 THEN N'Đạt'
           ELSE N'Không đạt'
       END AS KetQua
FROM diem d
JOIN sinhvien sv ON sv.masv = d.masv
WHERE d.malophp = @maLopHP AND (@kw = '' OR sv.masv LIKE @likeKw OR sv.tensv LIKE @likeKw)
ORDER BY sv.masv";

                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maLopHP", maLopHPDangChon);
                    cmd.Parameters.AddWithValue("@kw", kw);
                    cmd.Parameters.AddWithValue("@likeKw", "%" + kw + "%");

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        dgvSV.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
