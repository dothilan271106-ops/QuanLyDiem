using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Database;

namespace UI.UserControls
{
   
    public class UCQuanLyDiemSinhVien : UserControl
    {
        // ====== CẤU HÌNH ======
        // true  = hiển thị dữ liệu mẫu (để xem giao diện ngay)
        // false = đọc từ SQL Server
        private static readonly bool DungDuLieuMau = false;

        private static string ConnStr => DbHelper.ConnStr;

        // Các alias phía sau AS phải giữ nguyên vì grid bind theo tên này.
        private const string Sql = @"
SELECT  d.masv                  AS MaSV,
        sv.tensv                AS HoTen,
        d.malophp               AS MaLopHP,
        mh.tenmon               AS MonHoc,
        d.diemcc                AS ChuyenCan,
        d.diemgk                AS GiuaKy,
        d.diemck                AS CuoiKy,
        d.xeploai               AS XepLoai,
        CASE
            WHEN d.dakhoa = 1 THEN N'Đã khóa'
            WHEN d.diemcc IS NULL AND d.diemgk IS NULL AND d.diemck IS NULL THEN N'Chưa nhập'
            ELSE N'Chưa khóa'
        END                     AS TrangThai
FROM    diem d
JOIN    sinhvien   sv ON sv.masv    = d.masv
JOIN    lophocphan lp ON lp.malophp = d.malophp
JOIN    monhoc     mh ON mh.mamon   = lp.mamon
ORDER BY d.malophp, d.masv";

        // ====== MÀU SẮC ======
        private static readonly Color MauChinh = Color.FromArgb(94, 22, 110);
        private static readonly Color MauNhat = Color.FromArgb(243, 229, 250);
        private static readonly Color MauNen = Color.FromArgb(250, 243, 253);
        private static readonly Color MauChu = Color.FromArgb(60, 20, 70);
        private static readonly Color MauVien = Color.FromArgb(225, 205, 235);

        // ====== CONTROL ======
        private Guna2TextBox txtTimKiem;
        private Guna2Button btnTimKiem, btnNhapDiem, btnSuaDiem, btnKhoaDiem, btnLamMoi;
        private Label lblTong;
        private DataGridView dgv;
        private DataTable _dt;

        public UCQuanLyDiemSinhVien()
        {
            XayDungGiaoDien();
            Load += UCQuanLyDiemSinhVien_Load;
        }

        private void UCQuanLyDiemSinhVien_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            TaiDuLieu();
        }

        // =====================================================================
        //  DỰNG GIAO DIỆN
        // =====================================================================
        private void XayDungGiaoDien()
        {
            SuspendLayout();

            BackColor = MauNen;
            Padding = new Padding(24, 20, 24, 24);
            Font = new Font("Segoe UI", 9.5f);

            // ---- Tiêu đề ----
            var lblTieuDe = new Label
            {
                Text = "Quản lý điểm",
                Dock = DockStyle.Top,
                Height = 52,
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // ---- Thẻ trắng bo góc ----
            var card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 16,
                Padding = new Padding(16)
            };

            // ---- Hàng 1: ô tìm kiếm + nút Tìm kiếm ----
            var pnlTim = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(0, 0, 0, 10),
                BackColor = Color.White
            };

            txtTimKiem = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Tìm mã sinh viên, họ tên, mã lớp HP, môn học...",
                BorderRadius = 8,
                BorderColor = MauVien,
                FillColor = Color.White,
                ForeColor = MauChu,
                Font = new Font("Segoe UI", 9.5f),
                IconLeft = TaoIcon("\uE721", Color.FromArgb(130, 100, 150), 16),
                IconLeftSize = new Size(16, 16)
            };
            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TimKiem(); }
            };

            var spacer = new Panel { Dock = DockStyle.Right, Width = 10, BackColor = Color.White };

            btnTimKiem = TaoNut("Tìm kiếm", "\uE721", true);
            btnTimKiem.Dock = DockStyle.Right;
            btnTimKiem.Width = 116;
            btnTimKiem.Margin = Padding.Empty;
            btnTimKiem.Click += (s, e) => TimKiem();

            // Thứ tự Add: Fill trước, các control Right sau
            pnlTim.Controls.Add(txtTimKiem);
            pnlTim.Controls.Add(spacer);
            pnlTim.Controls.Add(btnTimKiem);

            // ---- Hàng 2: các nút chức năng + Tổng số ----
            var pnlNut = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(0, 0, 0, 12),
                BackColor = Color.White
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Margin = Padding.Empty
            };

            btnNhapDiem = TaoNut("Nhập điểm", "\uE710", true);
            btnNhapDiem.Click += btnNhapDiem_Click;

            btnSuaDiem = TaoNut("Sửa điểm", "\uE70F", false);
            btnKhoaDiem = TaoNut("Khóa điểm", "\uE72E", false);
            btnLamMoi = TaoNut("Làm mới", "\uE72C", false);

       

            btnSuaDiem.Click += btnSuaDiem_Click;
            btnKhoaDiem.Click += btnKhoaDiem_Click;
            btnLamMoi.Click += btnLamMoi_Click;

            flow.Controls.AddRange(new Control[] { btnNhapDiem, btnSuaDiem, btnKhoaDiem, btnLamMoi });

            var pnlTong = new Guna2Panel
            {
                Dock = DockStyle.Right,
                Width = 104,
                FillColor = MauNhat,
                BorderRadius = 8,
                Padding = new Padding(4)
            };
            lblTong = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Tổng số: 0",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = MauChinh,
                BackColor = MauNhat,
                Font = new Font("Segoe UI Semibold", 9.5f)
            };
            pnlTong.Controls.Add(lblTong);

            pnlNut.Controls.Add(flow);
            pnlNut.Controls.Add(pnlTong);

            // ---- Bảng dữ liệu ----
            dgv = TaoGrid();

            // Thứ tự Add: Fill trước, rồi Top (cái add sau nằm trên cùng)
            card.Controls.Add(dgv);
            card.Controls.Add(pnlNut);
            card.Controls.Add(pnlTim);

            Controls.Add(card);
            Controls.Add(lblTieuDe);

            ResumeLayout(false);
        }

        private DataGridView TaoGrid()
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
                RowTemplate = { Height = 42 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Header tím
            var hs = g.ColumnHeadersDefaultCellStyle;
            hs.BackColor = MauChinh;
            hs.ForeColor = Color.White;
            hs.SelectionBackColor = MauChinh;
            hs.SelectionForeColor = Color.White;
            hs.Font = new Font("Segoe UI Semibold", 9f);
            hs.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ô dữ liệu
            var cs = g.DefaultCellStyle;
            cs.BackColor = Color.White;
            cs.ForeColor = MauChu;
            cs.Font = new Font("Segoe UI", 9f);
            cs.SelectionBackColor = Color.FromArgb(236, 214, 247);
            cs.SelectionForeColor = MauChu;
            cs.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs.NullValue = "—";

            // Dòng xen kẽ
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 242, 252);
            g.AlternatingRowsDefaultCellStyle.SelectionBackColor = cs.SelectionBackColor;
            g.AlternatingRowsDefaultCellStyle.SelectionForeColor = cs.SelectionForeColor;

            // Cột
            g.Columns.Add(Cot("MaSV", "Mã SV", 70));
            g.Columns.Add(Cot("HoTen", "Họ tên", 120));
            g.Columns.Add(Cot("MaLopHP", "Mã lớp HP", 85));
            g.Columns.Add(Cot("MonHoc", "Môn học", 120));
            g.Columns.Add(Cot("ChuyenCan", "Chuyên cần", 80, "0.00"));
            g.Columns.Add(Cot("GiuaKy", "Giữa kỳ", 70, "0.00"));
            g.Columns.Add(Cot("CuoiKy", "Cuối kỳ", 70, "0.00"));
            g.Columns.Add(Cot("XepLoai", "Xếp loại", 75));
            g.Columns.Add(Cot("TrangThai", "Trạng thái", 95));

            g.CellPainting += Dgv_CellPainting;
            g.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) btnSuaDiem_Click(s, e); };
            return g;
        }

        private static DataGridViewTextBoxColumn Cot(string prop, string header, float weight, string format = null)
        {
            var c = new DataGridViewTextBoxColumn
            {
                Name = "col" + prop,
                DataPropertyName = prop,
                HeaderText = header,
                FillWeight = weight,
                MinimumWidth = 60,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            if (format != null) c.DefaultCellStyle.Format = format;
            return c;
        }

        private Guna2Button TaoNut(string text, string glyph, bool chinh)
        {
            Color nen = chinh ? MauChinh : MauNhat;
            Color chu = chinh ? Color.White : MauChinh;

            var b = new Guna2Button
            {
                Text = text,
                Size = new Size(124, 36),
                BorderRadius = 8,
                FillColor = nen,
                ForeColor = chu,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0),
                Image = TaoIcon(glyph, chu, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(14, 0),
                TextAlign = HorizontalAlignment.Center,
                TextOffset = new Point(8, 0)
            };
            b.HoverState.FillColor = chinh ? Color.FromArgb(120, 40, 140) : Color.FromArgb(232, 210, 244);
            return b;
        }

        /// <summary>Vẽ icon từ font Segoe MDL2 Assets (Windows 10+) thành Bitmap.</summary>
        private static Image TaoIcon(string glyph, Color color, int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            using (var f = new Font("Segoe MDL2 Assets", size * 0.8f, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var br = new SolidBrush(color))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.DrawString(glyph, f, br, new RectangleF(0, 0, size, size), sf);
            }
            return bmp;
        }

        // =====================================================================
        //  VẼ "PILL" TRẠNG THÁI
        // =====================================================================
        private void Dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgv.Columns[e.ColumnIndex].Name != "colTrangThai") return;

            // Vẽ nền + viền ô, bỏ phần chữ
            e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

            string text = Convert.ToString(e.Value);
            if (string.IsNullOrEmpty(text)) { e.Handled = true; return; }

            Color nen, chu;
            switch (text)
            {
                case "Đã khóa":
                    nen = Color.FromArgb(253, 222, 226); chu = Color.FromArgb(190, 40, 60); break;
                case "Chưa khóa":
                    nen = Color.FromArgb(214, 245, 226); chu = Color.FromArgb(30, 130, 70); break;
                default: // Chưa nhập
                    nen = Color.FromArgb(232, 230, 245); chu = Color.FromArgb(100, 90, 130); break;
            }

            using (var font = new Font("Segoe UI", 8.5f))
            {
                Size ts = TextRenderer.MeasureText(text, font);
                int w = ts.Width + 18, h = 22;
                var rect = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - w) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - h) / 2, w, h);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = BoGoc(rect, h))
                using (var br = new SolidBrush(nen))
                    e.Graphics.FillPath(br, path);

                TextRenderer.DrawText(e.Graphics, text, font, rect, chu,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
            e.Handled = true;
        }

        private static GraphicsPath BoGoc(Rectangle r, int d)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // =====================================================================
        //  DỮ LIỆU
        // =====================================================================
        public void TaiDuLieu()
        {
            try
            {
                if (DungDuLieuMau)
                {
                    _dt = TaoDuLieuMau();
                }
                else
                {
                    _dt = new DataTable();
                    using (var conn = new SqlConnection(ConnStr))
                    using (var da = new SqlDataAdapter(Sql, conn))
                        da.Fill(_dt);
                }

                dgv.DataSource = _dt;
                TimKiem();   // áp dụng lại bộ lọc hiện tại (nếu có) và cập nhật "Tổng số"
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc dữ liệu: " + ex.Message, "Quản lý điểm",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TimKiem()
        {
            if (_dt == null) return;

            string kw = txtTimKiem.Text.Trim();
            if (kw.Length == 0)
            {
                _dt.DefaultView.RowFilter = "";
            }
            else
            {
                string s = kw.Replace("'", "''")
                             .Replace("[", "[[]")
                             .Replace("%", "[%]")
                             .Replace("*", "[*]");
                _dt.DefaultView.RowFilter = string.Format(
                    "MaSV LIKE '%{0}%' OR HoTen LIKE '%{0}%' OR MaLopHP LIKE '%{0}%' OR MonHoc LIKE '%{0}%'", s);
            }
            lblTong.Text = "Tổng số: " + _dt.DefaultView.Count;
        }

        private static DataTable TaoDuLieuMau()
        {
            var t = new DataTable();
            t.Columns.Add("MaSV", typeof(string));
            t.Columns.Add("HoTen", typeof(string));
            t.Columns.Add("MaLopHP", typeof(string));
            t.Columns.Add("MonHoc", typeof(string));
            t.Columns.Add("ChuyenCan", typeof(double));
            t.Columns.Add("GiuaKy", typeof(double));
            t.Columns.Add("CuoiKy", typeof(double));
            t.Columns.Add("XepLoai", typeof(string));
            t.Columns.Add("TrangThai", typeof(string));

            t.Rows.Add("SV001", "Nguyễn Văn An", "INT101-01", "Lập trình C#", 8.0, 7.5, 8.0, "Khá", "Chưa khóa");
            t.Rows.Add("SV002", "Trần Thị Bình", "INT101-01", "Lập trình C#", 7.0, 8.0, 8.5, "Giỏi", "Chưa khóa");
            t.Rows.Add("SV001", "Nguyễn Văn An", "INT102-01", "Cơ sở dữ liệu", 9.0, 8.0, 7.5, "Khá", "Đã khóa");
            t.Rows.Add("SV004", "Phạm Thu Dung", "INT102-01", "Cơ sở dữ liệu", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, "Chưa nhập");
            t.Rows.Add("SV003", "Lê Minh Cường", "INT103-01", "Mạng máy tính", 6.0, 7.0, 6.5, "Trung bình", "Chưa khóa");
            return t;
        }

        // =====================================================================
        //  SỰ KIỆN NÚT
        // =====================================================================
        /// <summary>Lấy MaSV + MaLopHP của dòng đang chọn; null nếu chưa chọn.</summary>
        private bool LayDongChon(out string maSV, out string maLopHP)
        {
            maSV = maLopHP = null;
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng trong bảng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            maSV = Convert.ToString(dgv.CurrentRow.Cells["colMaSV"].Value);
            maLopHP = Convert.ToString(dgv.CurrentRow.Cells["colMaLopHP"].Value);
            return true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UCQuanLyDiemSinhVien
            // 
            this.Name = "UCQuanLyDiemSinhVien";
            this.Size = new System.Drawing.Size(390, 150);
            this.ResumeLayout(false);

        }

        private void btnNhapDiem_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new QuanLyDiemSinhVien.FrmNhapDiem();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    TaiDuLieu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Lỗi mở form nhập điểm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSuaDiem_Click(object sender, EventArgs e)
        {
            string maSV, maLop;
            if (!LayDongChon(out maSV, out maLop)) return;

            try
            {
                var frm = new QuanLyDiemSinhVien.FrmNhapDiem(maSV, maLop);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    TaiDuLieu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Lỗi mở form sửa điểm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnKhoaDiem_Click(object sender, EventArgs e)
        {
            string maSV, maLop;
            if (!LayDongChon(out maSV, out maLop)) return;

            string trangThai = Convert.ToString(dgv.CurrentRow.Cells["colTrangThai"].Value);
            bool daKhoa = (trangThai == "Đã khóa");
            string hanhDong = daKhoa ? "mở khóa" : "khóa";

            var rs = MessageBox.Show(
                string.Format("Bạn có chắc chắn muốn {0} điểm cho sinh viên {1} (Lớp {2})?", hanhDong, maSV, maLop),
                string.Format("Xác nhận {0} điểm", hanhDong),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs != DialogResult.Yes) return;

            if (DungDuLieuMau)
            {
                dgv.CurrentRow.Cells["colTrangThai"].Value = daKhoa ? "Chưa khóa" : "Đã khóa";
                dgv.Invalidate();
                return;
            }

            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(
                    @"IF EXISTS (SELECT 1 FROM Diem WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP)
                      BEGIN
                          UPDATE Diem SET DaKhoa = @DaKhoa WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP;
                      END
                      ELSE
                      BEGIN
                          INSERT INTO Diem (MaSV, MaLopHP, DaKhoa) VALUES (@MaSV, @MaLopHP, @DaKhoa);
                      END", conn))
                {
                    cmd.Parameters.Add("@DaKhoa", SqlDbType.Bit).Value = !daKhoa;
                    cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 100).Value = maSV;
                    cmd.Parameters.Add("@MaLopHP", SqlDbType.VarChar, 100).Value = maLop;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(string.Format("Đã {0} điểm thành công!", hanhDong), "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TaiDuLieu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái khóa: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            TaiDuLieu();
        }
    }
}
