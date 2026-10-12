using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Database;

namespace QuanLyDiemSinhVien
{
    public class FrmNhapDiem : Form
    {
        private ComboBox cboMaSV, cboLopHP;
        private TextBox txtMonHoc, txtGiangVien, txtChuyenCan, txtGiuaKy,
                        txtCuoiKy, txtTongKet, txtXepLoai, txtTrangThai;
        private Button btnLuu, btnHuy;
        private bool dangNapDuLieu;
        private readonly string maSVBanDau;
        private readonly string maLopHPBanDau;

        private string GetConnStr()
        {
            return DbHelper.ConnStr;
        }

        public FrmNhapDiem(string maSV = null, string maLopHP = null)
        {
            maSVBanDau = maSV;
            maLopHPBanDau = maLopHP;
            InitializeUI();
            Load += FrmNhapDiem_Load;
        }

        private void InitializeUI()
        {
            Text = "Nhập / Sửa điểm sinh viên";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(850, 570);
            MinimumSize = new Size(800, 540);
            BackColor = Color.FromArgb(250, 244, 251);
            Font = new Font("Segoe UI", 9F);

            Label title = new Label
            {
                Text = "＋  Nhập / Sửa điểm sinh viên",
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                ForeColor = Color.FromArgb(105, 20, 120),
                Dock = DockStyle.Top,
                Height = 62,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0)
            };
            Controls.Add(title);

            GroupBox grpThongTin = new GroupBox
            {
                Text = "Thông tin sinh viên và lớp học phần",
                Location = new Point(22, 78),
                Size = new Size(790, 160),
                ForeColor = Color.FromArgb(105, 20, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            Controls.Add(grpThongTin);

            AddLabel(grpThongTin, "Mã sinh viên", 18, 30);
            cboMaSV = AddCombo(grpThongTin, 18, 52, 350);
            AddLabel(grpThongTin, "Lớp học phần", 400, 30);
            cboLopHP = AddCombo(grpThongTin, 400, 52, 350);
            AddLabel(grpThongTin, "Môn học", 18, 88);
            txtMonHoc = AddText(grpThongTin, 18, 110, 350, true);
            AddLabel(grpThongTin, "Giảng viên", 400, 88);
            txtGiangVien = AddText(grpThongTin, 400, 110, 350, true);

            GroupBox grpDiem = new GroupBox
            {
                Text = "Điểm đánh giá",
                Location = new Point(22, 250),
                Size = new Size(790, 190),
                ForeColor = Color.FromArgb(105, 20, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            Controls.Add(grpDiem);

            AddLabel(grpDiem, "Điểm chuyên cần (0 - 10)", 18, 30);
            txtChuyenCan = AddText(grpDiem, 18, 52, 220, false);
            AddLabel(grpDiem, "Điểm giữa kỳ (0 - 10)", 275, 30);
            txtGiuaKy = AddText(grpDiem, 275, 52, 220, false);
            AddLabel(grpDiem, "Điểm cuối kỳ (0 - 10)", 532, 30);
            txtCuoiKy = AddText(grpDiem, 532, 52, 220, false);

            AddLabel(grpDiem, "Điểm tổng kết", 18, 94);
            txtTongKet = AddText(grpDiem, 18, 116, 220, true);
            AddLabel(grpDiem, "Xếp loại", 275, 94);
            txtXepLoai = AddText(grpDiem, 275, 116, 220, true);
            AddLabel(grpDiem, "Trạng thái khóa", 532, 94);
            txtTrangThai = AddText(grpDiem, 532, 116, 220, true);

            btnLuu = new Button
            {
                Text = "Lưu điểm",
                Location = new Point(610, 458),
                Size = new Size(100, 38),
                BackColor = Color.FromArgb(113, 23, 127),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += btnLuu_Click;
            Controls.Add(btnLuu);

            btnHuy = new Button
            {
                Text = "Hủy",
                Location = new Point(720, 458),
                Size = new Size(90, 38),
                BackColor = Color.FromArgb(244, 222, 247),
                ForeColor = Color.FromArgb(105, 20, 120),
                FlatStyle = FlatStyle.Flat
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => Close();
            Controls.Add(btnHuy);

            cboMaSV.SelectedIndexChanged += cboMaSV_SelectedIndexChanged;
            cboLopHP.SelectedIndexChanged += cboLopHP_SelectedIndexChanged;
            txtChuyenCan.TextChanged += (s, e) => TinhDiem();
            txtGiuaKy.TextChanged += (s, e) => TinhDiem();
            txtCuoiKy.TextChanged += (s, e) => TinhDiem();
        }

        private void AddLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(65, 45, 75),
                Font = new Font("Segoe UI", 9F)
            });
        }

        private ComboBox AddCombo(Control parent, int x, int y, int width)
        {
            ComboBox c = new ComboBox
            {
                Location = new Point(x, y),
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            parent.Controls.Add(c);
            return c;
        }

        private TextBox AddText(Control parent, int x, int y, int width, bool readOnly)
        {
            TextBox t = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                ReadOnly = readOnly,
                Font = new Font("Segoe UI", 9F),
                BackColor = readOnly ? Color.FromArgb(244, 240, 249) : Color.White
            };
            parent.Controls.Add(t);
            return t;
        }

        private void FrmNhapDiem_Load(object sender, EventArgs e)
        {
            dangNapDuLieu = true;
            try
            {
                LoadSinhVien();
                LoadLopHocPhan();

                if (!string.IsNullOrEmpty(maSVBanDau))
                {
                    cboMaSV.SelectedValue = maSVBanDau;
                }
                if (!string.IsNullOrEmpty(maLopHPBanDau))
                {
                    cboLopHP.SelectedValue = maLopHPBanDau;
                }
            }
            finally
            {
                dangNapDuLieu = false;
            }

            CapNhatThongTinLop();
            LoadDiemHienCo();
        }

        private void LoadSinhVien()
        {
            using (SqlConnection conn = new SqlConnection(GetConnStr()))
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT MaSV, (MaSV + ' - ' + TenSV) AS HienThi FROM SinhVien ORDER BY MaSV", conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                cboMaSV.DataSource = dt;
                cboMaSV.DisplayMember = "HienThi";
                cboMaSV.ValueMember = "MaSV";
                cboMaSV.SelectedIndex = -1;
            }
        }

        private void LoadLopHocPhan()
        {
            using (SqlConnection conn = new SqlConnection(GetConnStr()))
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT lhp.MaLopHP, (lhp.MaLopHP + ' - ' + mh.TenMon) AS HienThi, mh.TenMon, lhp.GiangVien
FROM LopHocPhan lhp
INNER JOIN MonHoc mh ON lhp.MaMon = mh.MaMon
ORDER BY lhp.MaLopHP", conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                cboLopHP.DataSource = dt;
                cboLopHP.DisplayMember = "HienThi";
                cboLopHP.ValueMember = "MaLopHP";
                cboLopHP.SelectedIndex = -1;
            }
        }

        private void cboMaSV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dangNapDuLieu) return;
            LoadDiemHienCo();
        }

        private void cboLopHP_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatThongTinLop();
            if (dangNapDuLieu) return;
            LoadDiemHienCo();
        }

        private void CapNhatThongTinLop()
        {
            if (cboLopHP.SelectedItem is DataRowView row)
            {
                txtMonHoc.Text = Convert.ToString(row["TenMon"]);
                txtGiangVien.Text = Convert.ToString(row["GiangVien"]);
            }
            else
            {
                txtMonHoc.Clear();
                txtGiangVien.Clear();
            }
        }

        private void LoadDiemHienCo()
        {
            if (cboMaSV.SelectedValue == null || cboLopHP.SelectedValue == null)
            {
                txtTrangThai.Text = "Chưa chọn đủ thông tin";
                return;
            }

            string maSV = cboMaSV.SelectedValue.ToString();
            string maLopHP = cboLopHP.SelectedValue.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnStr()))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT DiemCC, DiemGK, DiemCK, DiemTongKet, XepLoai, DaKhoa FROM Diem WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSV", maSV);
                    cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dangNapDuLieu = true;
                        if (reader.Read())
                        {
                            object valCC = reader["DiemCC"];
                            object valGK = reader["DiemGK"];
                            object valCK = reader["DiemCK"];
                            bool daKhoa = reader["DaKhoa"] != DBNull.Value && Convert.ToBoolean(reader["DaKhoa"]);

                            txtChuyenCan.Text = valCC != DBNull.Value ? Convert.ToDecimal(valCC).ToString("0.00") : "";
                            txtGiuaKy.Text = valGK != DBNull.Value ? Convert.ToDecimal(valGK).ToString("0.00") : "";
                            txtCuoiKy.Text = valCK != DBNull.Value ? Convert.ToDecimal(valCK).ToString("0.00") : "";

                            txtTrangThai.Text = daKhoa ? "Đã khóa" : "Chưa khóa";

                            txtChuyenCan.ReadOnly = daKhoa;
                            txtGiuaKy.ReadOnly = daKhoa;
                            txtCuoiKy.ReadOnly = daKhoa;
                            btnLuu.Enabled = !daKhoa;
                        }
                        else
                        {
                            txtChuyenCan.Clear();
                            txtGiuaKy.Clear();
                            txtCuoiKy.Clear();
                            txtTongKet.Clear();
                            txtXepLoai.Clear();
                            txtTrangThai.Text = "Chưa có điểm / Chưa khóa";

                            txtChuyenCan.ReadOnly = false;
                            txtGiuaKy.ReadOnly = false;
                            txtCuoiKy.ReadOnly = false;
                            btnLuu.Enabled = true;
                        }
                        dangNapDuLieu = false;
                    }
                }
                TinhDiem();
            }
            catch (Exception ex)
            {
                dangNapDuLieu = false;
                MessageBox.Show("Lỗi tải thông tin điểm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool DocDiem(out decimal cc, out decimal gk, out decimal ck)
        {
            cc = gk = ck = 0;
            string sCC = (txtChuyenCan.Text ?? "").Trim().Replace(',', '.');
            string sGK = (txtGiuaKy.Text ?? "").Trim().Replace(',', '.');
            string sCK = (txtCuoiKy.Text ?? "").Trim().Replace(',', '.');

            if (!decimal.TryParse(sCC, NumberStyles.Number, CultureInfo.InvariantCulture, out cc))
                return false;
            if (!decimal.TryParse(sGK, NumberStyles.Number, CultureInfo.InvariantCulture, out gk))
                return false;
            if (!decimal.TryParse(sCK, NumberStyles.Number, CultureInfo.InvariantCulture, out ck))
                return false;

            return cc >= 0 && cc <= 10 && gk >= 0 && gk <= 10 && ck >= 0 && ck <= 10;
        }

        private void TinhDiem()
        {
            if (dangNapDuLieu) return;
            decimal cc, gk, ck;
            if (DocDiem(out cc, out gk, out ck))
            {
                decimal tong = Math.Round(cc * 0.1m + gk * 0.3m + ck * 0.6m, 2);
                txtTongKet.Text = tong.ToString("0.00");
                if (tong >= 8.5m) txtXepLoai.Text = "Giỏi";
                else if (tong >= 7m) txtXepLoai.Text = "Khá";
                else if (tong >= 5m) txtXepLoai.Text = "Trung bình";
                else txtXepLoai.Text = "Yếu";
            }
            else
            {
                txtTongKet.Clear();
                txtXepLoai.Clear();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboMaSV.SelectedIndex < 0 || cboLopHP.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên và lớp học phần.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal cc, gk, ck;
            if (!DocDiem(out cc, out gk, out ck))
            {
                MessageBox.Show("Nhập đủ 3 điểm hợp lệ trong khoảng 0 đến 10.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tong = Math.Round(cc * 0.1m + gk * 0.3m + ck * 0.6m, 2);
            string xepLoai = tong >= 8.5m ? "Giỏi" :
                             tong >= 7m ? "Khá" :
                             tong >= 5m ? "Trung bình" : "Yếu";

            string maSV = cboMaSV.SelectedValue?.ToString() ?? "";
            string maLopHP = cboLopHP.SelectedValue?.ToString() ?? "";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnStr()))
                using (SqlCommand cmd = new SqlCommand(@"
IF EXISTS (
    SELECT 1 FROM Diem
    WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP
)
BEGIN
    IF EXISTS (
        SELECT 1 FROM Diem
        WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP
          AND DaKhoa = 1
    )
        THROW 50001, N'Điểm đã khóa, không thể sửa.', 1;

    UPDATE Diem
    SET DiemCC = @CC,
        DiemGK = @GK,
        DiemCK = @CK,
        DiemTongKet = @TongKet,
        XepLoai = @XepLoai
    WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP;
END
ELSE
BEGIN
    INSERT INTO Diem
        (MaSV, MaLopHP, DiemCC, DiemGK, DiemCK,
         DiemTongKet, XepLoai, DaKhoa)
    VALUES
        (@MaSV, @MaLopHP, @diemcc, @diemgk, @diemck
         @diemTongKet, @XepLoai, 0);
END
", conn))
                {
                    cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 100).Value = maSV;
                    cmd.Parameters.Add("@MaLopHP", SqlDbType.VarChar, 100).Value = maLopHP;
                    cmd.Parameters.Add("@diemcc", SqlDbType.Decimal).Value = cc;
                    cmd.Parameters.Add("@diemgk", SqlDbType.Decimal).Value = gk;
                    cmd.Parameters.Add("@diemck", SqlDbType.Decimal).Value = ck;
                    cmd.Parameters.Add("@diemTongKet", SqlDbType.Decimal).Value = tong;
                    cmd.Parameters.Add("@XepLoai", SqlDbType.NVarChar, 100).Value = xepLoai;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lưu điểm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không lưu được điểm.\n" + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
