using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Database;
using Guna.UI2.WinForms;

namespace UI.UserControls
{
    /// <summary>
    /// UserControl Quản lý điểm được chuyển đổi từ ucDiem.axaml (Avalonia) sang Windows Forms (WinForms).
    /// Giữ nguyên toàn bộ logic, luồng thao tác trực tiếp trên một màn hình:
    /// Chọn lớp học phần -> Chọn sinh viên trong bảng -> Nhập / Sửa điểm -> Khóa / Mở khóa điểm.
    /// </summary>
    public class UcDiem : UserControl
    {
        private ComboBox cblhp;
        private Label lbltrangthai;
        private Guna2TextBox txttim;
        private Guna2Button btntim;

        private Label lblsv;
        private Guna2TextBox txtcc;
        private Guna2TextBox txtgk;
        private Guna2TextBox txtck;
        private Guna2Button btnnhap;
        private Guna2Button btnsua;
        private Guna2Button btnkhoa;

        private DataGridView dgdiem;

        // Trạng thái hiện tại
        private string _maLopHPChon = "";
        private string _maSVChon = "";
        private string _tenSVChon = "";
        private bool _svDaKhoa = false;
        private bool _lopDaKhoa = false;

        private static readonly Color MauChinh = Color.FromArgb(30, 58, 138); // #1E3A8A xanh navy như bản Avalonia
        private static readonly Color MauNen = Color.FromArgb(241, 245, 249);  // #F1F5F9
        private static readonly Color MauChu = Color.FromArgb(30, 41, 59);

        public UcDiem()
        {
            XayDungGiaoDien();
            Load += UcDiem_Load;
        }

        private void XayDungGiaoDien()
        {
            SuspendLayout();
            BackColor = MauNen;
            Padding = new Padding(20);
            Font = new Font("Segoe UI", 9.5f);
            AutoScroll = true;

            // ===== 1. BANNER TIÊU ĐỀ =====
            var pnlBanner = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                FillColor = Color.White,
                BorderRadius = 12,
                Padding = new Padding(16, 10, 16, 10),
                Margin = new Padding(0, 0, 0, 14)
            };

            var lblTieuDe = new Label
            {
                Text = "QUẢN LÝ ĐIỂM (Bản chuyển từ ucDiem.axaml)",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblMoTa = new Label
            {
                Text = "Chọn lớp học phần, chọn sinh viên rồi nhập hoặc sửa điểm trực tiếp trên bảng",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlBanner.Controls.Add(lblMoTa);
            pnlBanner.Controls.Add(lblTieuDe);

            // ===== 2. CARD CHỌN LỚP HỌC PHẦN =====
            var cardChonLop = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                FillColor = Color.White,
                BorderRadius = 12,
                Padding = new Padding(16, 12, 16, 12),
                Margin = new Padding(0, 0, 0, 14)
            };

            var lblCard1Header = new Label
            {
                Text = "🔍  CHỌN LỚP HỌC PHẦN",
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Segoe UI Semibold", 10.5f),
                ForeColor = MauChinh
            };

            var pnlHang1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.White
            };

            var lblLhpText = new Label
            {
                Text = "Lớp học phần:",
                Dock = DockStyle.Left,
                Width = 100,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = MauChu
            };

            cblhp = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cblhp.SelectedIndexChanged += Cblhp_SelectedIndexChanged;

            lbltrangthai = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 9f, FontStyle.Italic)
            };

            pnlHang1.Controls.Add(lbltrangthai);
            pnlHang1.Controls.Add(cblhp);
            pnlHang1.Controls.Add(lblLhpText);

            var pnlHang2 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34,
                Padding = new Padding(0, 4, 0, 0),
                BackColor = Color.White
            };

            txttim = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Tìm sinh viên theo mã hoặc họ tên...",
                BorderRadius = 6,
                BorderColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 9f)
            };
            txttim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TaiDiem(); } };

            btntim = new Guna2Button
            {
                Text = "Tìm kiếm",
                Dock = DockStyle.Right,
                Width = 95,
                BorderRadius = 6,
                FillColor = MauChinh,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btntim.Click += (s, e) => TaiDiem();

            pnlHang2.Controls.Add(txttim);
            pnlHang2.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White });
            pnlHang2.Controls.Add(btntim);

            cardChonLop.Controls.Add(pnlHang2);
            cardChonLop.Controls.Add(pnlHang1);
            cardChonLop.Controls.Add(lblCard1Header);

            // ===== 3. CARD NHẬP / SỬA ĐIỂM =====
            var cardNhapDiem = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                FillColor = Color.White,
                BorderRadius = 12,
                Padding = new Padding(16, 12, 16, 12),
                Margin = new Padding(0, 0, 0, 14)
            };

            lblsv = new Label
            {
                Text = "👤  Chọn sinh viên trong bảng bên dưới để nhập hoặc sửa điểm",
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Segoe UI Semibold", 10.5f),
                ForeColor = MauChinh
            };

            var pnlNhapDiemInput = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 44,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White
            };

            pnlNhapDiemInput.Controls.Add(TaoInputDiem("Chuyên cần (10%):", out txtcc));
            pnlNhapDiemInput.Controls.Add(TaoInputDiem("Giữa kỳ (30%):", out txtgk));
            pnlNhapDiemInput.Controls.Add(TaoInputDiem("Cuối kỳ (60%):", out txtck));

            var pnlNutBam = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 44,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Padding = new Padding(0, 6, 0, 0)
            };

            btnnhap = new Guna2Button
            {
                Text = "💾  Nhập điểm",
                Width = 120,
                Height = 36,
                BorderRadius = 6,
                FillColor = Color.FromArgb(22, 163, 74), // Green #16A34A
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnnhap.Click += (s, e) => GhiDiem(true);

            btnsua = new Guna2Button
            {
                Text = "✏️  Sửa điểm",
                Width = 110,
                Height = 36,
                BorderRadius = 6,
                FillColor = Color.FromArgb(37, 99, 235), // Blue #2563EB
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnsua.Click += (s, e) => GhiDiem(false);

            btnkhoa = new Guna2Button
            {
                Text = "🔒  Khoá điểm",
                Width = 135,
                Height = 36,
                BorderRadius = 6,
                FillColor = Color.FromArgb(220, 38, 38), // Red #DC2626
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnkhoa.Click += BtnKhoa_Click;

            pnlNutBam.Controls.AddRange(new Control[] { btnnhap, btnsua, btnkhoa });

            cardNhapDiem.Controls.Add(pnlNutBam);
            cardNhapDiem.Controls.Add(pnlNhapDiemInput);
            cardNhapDiem.Controls.Add(lblsv);

            // ===== 4. CARD BẢNG ĐIỂM SINH VIÊN =====
            var cardBangDiem = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 12,
                Padding = new Padding(16, 12, 16, 12)
            };

            var lblCard3Header = new Label
            {
                Text = "📋  BẢNG ĐIỂM SINH VIÊN TRONG LỚP",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI Semibold", 10.5f),
                ForeColor = MauChinh
            };

            dgdiem = new DataGridView
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
                GridColor = Color.FromArgb(226, 232, 240),
                RowTemplate = { Height = 36 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var hs = dgdiem.ColumnHeadersDefaultCellStyle;
            hs.BackColor = MauChinh;
            hs.ForeColor = Color.White;
            hs.SelectionBackColor = MauChinh;
            hs.SelectionForeColor = Color.White;
            hs.Font = new Font("Segoe UI Semibold", 9.5f);
            hs.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var cs = dgdiem.DefaultCellStyle;
            cs.BackColor = Color.White;
            cs.ForeColor = MauChu;
            cs.Font = new Font("Segoe UI", 9.5f);
            cs.SelectionBackColor = Color.FromArgb(219, 234, 254);
            cs.SelectionForeColor = MauChu;
            cs.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs.NullValue = "—";

            dgdiem.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgdiem.AlternatingRowsDefaultCellStyle.SelectionBackColor = cs.SelectionBackColor;
            dgdiem.AlternatingRowsDefaultCellStyle.SelectionForeColor = cs.SelectionForeColor;

            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "STT", HeaderText = "STT", FillWeight = 45 });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSV", HeaderText = "Mã SV", FillWeight = 85 });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenSV", HeaderText = "Họ và tên", FillWeight = 140 });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaLP", HeaderText = "Lớp", FillWeight = 75 });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemCC", HeaderText = "Chuyên cần", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemGK", HeaderText = "Giữa kỳ", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemCK", HeaderText = "Cuối kỳ", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemTongKet", HeaderText = "Tổng kết", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "XepLoai", HeaderText = "Xếp loại", FillWeight = 90 });
            dgdiem.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });

            dgdiem.SelectionChanged += Dgdiem_SelectionChanged;

            cardBangDiem.Controls.Add(dgdiem);
            cardBangDiem.Controls.Add(lblCard3Header);

            // Thứ tự Add controls (Fill trước, Top sau)
            Controls.Add(cardBangDiem);
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            Controls.Add(cardNhapDiem);
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            Controls.Add(cardChonLop);
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            Controls.Add(pnlBanner);

            ResumeLayout(false);
        }

        private Control TaoInputDiem(string nhan, out Guna2TextBox txt)
        {
            var pnl = new Panel { Width = 200, Height = 40, Margin = new Padding(0, 0, 14, 0) };
            var lbl = new Label
            {
                Text = nhan,
                Dock = DockStyle.Left,
                Width = 115,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = MauChu,
                Font = new Font("Segoe UI", 9f)
            };

            txt = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                BorderRadius = 6,
                BorderColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 9.5f)
            };

            pnl.Controls.Add(txt);
            pnl.Controls.Add(lbl);
            return pnl;
        }

        private void UcDiem_Load(object sender, EventArgs e)
        {
            TaiDanhSachLHP();
        }

        private void TaiDanhSachLHP()
        {
            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var da = new SqlDataAdapter(@"
SELECT l.malophp AS MaLopHP,
       (l.malophp + ' - ' + m.tenmon) AS HienThi,
       m.tenmon AS TenMon
FROM lophocphan l
JOIN monhoc m ON m.mamon = l.mamon
ORDER BY l.malophp", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    cblhp.DataSource = dt;
                    cblhp.DisplayMember = "HienThi";
                    cblhp.ValueMember = "MaLopHP";

                    if (dt.Rows.Count > 0)
                    {
                        cblhp.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lớp học phần: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Cblhp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cblhp.SelectedValue == null) return;
            _maLopHPChon = cblhp.SelectedValue.ToString();
            txttim.Clear();
            TaiDiem();
        }

        private void XoaForm()
        {
            _maSVChon = "";
            _tenSVChon = "";
            _svDaKhoa = false;
            lblsv.Text = "👤  Chọn sinh viên trong bảng bên dưới để nhập hoặc sửa điểm";
            txtcc.Clear();
            txtgk.Clear();
            txtck.Clear();
        }

        private void TaiDiem()
        {
            XoaForm();
            if (string.IsNullOrEmpty(_maLopHPChon))
            {
                dgdiem.DataSource = null;
                lbltrangthai.Text = "";
                btnkhoa.Text = "🔒  Khoá điểm";
                return;
            }

            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                {
                    conn.Open();

                    // Kiểm tra trạng thái khóa của cả lớp
                    using (var cmd = new SqlCommand("SELECT ISNULL(MIN(CAST(dakhoa AS INT)), 0) FROM diem WHERE malophp = @l", conn))
                    {
                        cmd.Parameters.AddWithValue("@l", _maLopHPChon);
                        int kh = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        _lopDaKhoa = (kh == 1);
                    }

                    lbltrangthai.Text = _lopDaKhoa
                        ? "⚠️ Điểm của lớp học phần này đã khoá."
                        : "✅ Điểm đang mở, có thể nhập và sửa.";
                    btnkhoa.Text = _lopDaKhoa ? "🔓  Mở khoá điểm" : "🔒  Khoá điểm";

                    string kw = (txttim.Text ?? "").Trim();
                    string sql = @"
SELECT ROW_NUMBER() OVER (ORDER BY sv.masv) AS STT,
       sv.masv AS MaSV,
       sv.tensv AS TenSV,
       sv.malp AS MaLP,
       d.diemcc AS DiemCC,
       d.diemgk AS DiemGK,
       d.diemck AS DiemCK,
       d.diemtongket AS DiemTongKet,
       d.xeploai AS XepLoai,
       d.dakhoa AS DaKhoa,
       CASE WHEN d.dakhoa = 1 THEN N'Đã khoá' ELSE N'' END AS TrangThai
FROM diem d
JOIN sinhvien sv ON sv.masv = d.masv
WHERE d.malophp = @l AND (@kw = '' OR sv.masv LIKE @likeKw OR sv.tensv LIKE @likeKw)
ORDER BY sv.masv";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@l", _maLopHPChon);
                        cmd.Parameters.AddWithValue("@kw", kw);
                        cmd.Parameters.AddWithValue("@likeKw", "%" + kw + "%");

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            dgdiem.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgdiem_SelectionChanged(object sender, EventArgs e)
        {
            if (dgdiem.CurrentRow == null) return;

            var r = dgdiem.CurrentRow;
            _maSVChon = Convert.ToString(r.Cells[1].Value);
            _tenSVChon = Convert.ToString(r.Cells[2].Value);
            _svDaKhoa = Convert.ToBoolean(r.Cells["DaKhoa"] != null && r.Cells["DaKhoa"].Value != DBNull.Value ? r.Cells["DaKhoa"].Value : false);

            lblsv.Text = string.Format("👤  {0} - {1}{2}", _maSVChon, _tenSVChon, _svDaKhoa ? " (điểm đã khoá)" : "");

            txtcc.Text = r.Cells[4].Value != DBNull.Value ? Convert.ToDecimal(r.Cells[4].Value).ToString("0.00") : "";
            txtgk.Text = r.Cells[5].Value != DBNull.Value ? Convert.ToDecimal(r.Cells[5].Value).ToString("0.00") : "";
            txtck.Text = r.Cells[6].Value != DBNull.Value ? Convert.ToDecimal(r.Cells[6].Value).ToString("0.00") : "";
        }

        private static bool DocDiem(string t, out decimal? v)
        {
            v = null;
            string s = (t ?? "").Trim().Replace(',', '.');
            if (string.IsNullOrEmpty(s)) return true; // để trống là hợp lệ

            decimal x;
            if (!decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                return false;
            if (x < 0 || x > 10)
                return false;

            v = Math.Round(x, 2);
            return true;
        }

        private void GhiDiem(bool nhapMoi)
        {
            if (string.IsNullOrEmpty(_maLopHPChon) || string.IsNullOrEmpty(_maSVChon))
            {
                MessageBox.Show("Vui lòng chọn một lớp học phần và chọn một sinh viên trong bảng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_svDaKhoa)
            {
                MessageBox.Show("Điểm của sinh viên này đã khoá, không thể nhập hoặc sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? cc, gk, ck;
            bool hopLe = DocDiem(txtcc.Text, out cc) && DocDiem(txtgk.Text, out gk) && DocDiem(txtck.Text, out ck);
            if (!hopLe)
            {
                MessageBox.Show("Điểm phải là số từ 0 đến 10 (có thể để trống nếu chưa có).", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nhapMoi && cc == null && gk == null && ck == null)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một điểm thành phần.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!nhapMoi)
            {
                var xn = MessageBox.Show(string.Format("Bạn có chắc chắn muốn sửa điểm của sinh viên {0}?", _tenSVChon),
                    "Xác nhận sửa điểm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (xn != DialogResult.Yes) return;
            }

            decimal? tk = null;
            string xl = null;
            if (cc.HasValue && gk.HasValue && ck.HasValue)
            {
                tk = Math.Round(cc.Value * 0.1m + gk.Value * 0.3m + ck.Value * 0.6m, 2);
                if (tk >= 9.0m) xl = "Xuất sắc";
                else if (tk >= 8.0m) xl = "Giỏi";
                else if (tk >= 7.0m) xl = "Khá";
                else if (tk >= 5.0m) xl = "Trung bình";
                else xl = "Yếu";
            }

            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM diem WHERE masv = @s AND malophp = @l)
BEGIN
    UPDATE diem 
    SET diemcc = @cc, diemgk = @gk, diemck = @ck, diemtongket = @tk, xeploai = @xl
    WHERE masv = @s AND malophp = @l AND dakhoa = 0;
END
ELSE
BEGIN
    INSERT INTO diem (masv, malophp, diemcc, diemgk, diemck, diemtongket, xeploai, dakhoa)
    VALUES (@s, @l, @cc, @gk, @ck, @tk, @xl, 0);
END", conn))
                {
                    cmd.Parameters.Add("@s", SqlDbType.VarChar, 100).Value = _maSVChon;
                    cmd.Parameters.Add("@l", SqlDbType.VarChar, 100).Value = _maLopHPChon;
                    cmd.Parameters.Add("@cc", SqlDbType.Decimal).Value = (object)cc ?? DBNull.Value;
                    cmd.Parameters.Add("@gk", SqlDbType.Decimal).Value = (object)gk ?? DBNull.Value;
                    cmd.Parameters.Add("@ck", SqlDbType.Decimal).Value = (object)ck ?? DBNull.Value;
                    cmd.Parameters.Add("@tk", SqlDbType.Decimal).Value = (object)tk ?? DBNull.Value;
                    cmd.Parameters.Add("@xl", SqlDbType.NVarChar, 100).Value = (object)xl ?? DBNull.Value;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Lưu điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TaiDiem();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnKhoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_maLopHPChon))
            {
                MessageBox.Show("Vui lòng chọn lớp học phần.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool khoa = !_lopDaKhoa;
            string hoi = khoa
                ? string.Format("Khoá điểm của cả lớp học phần {0}?\nSau khi khoá sẽ không thể nhập hoặc sửa điểm.", _maLopHPChon)
                : string.Format("Mở khoá điểm của lớp học phần {0}?", _maLopHPChon);

            var xn = MessageBox.Show(hoi, "Xác nhận khoá / mở khoá", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xn != DialogResult.Yes) return;

            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand("UPDATE diem SET dakhoa = @k WHERE malophp = @l", conn))
                {
                    cmd.Parameters.Add("@k", SqlDbType.Bit).Value = khoa;
                    cmd.Parameters.Add("@l", SqlDbType.VarChar, 100).Value = _maLopHPChon;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(string.Format("Đã {0} điểm của lớp thành công!", khoa ? "khoá" : "mở khoá"),
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TaiDiem();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái khoá: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
