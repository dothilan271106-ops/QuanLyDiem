using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Database;
using Guna.UI2.WinForms;

namespace UI.UserControls
{
    public class UcBaoCao : UserControl
    {
        private ComboBox cboLopHP;
        private Label lblTieuDeBaoCao;
        private DataGridView dgvBaoCao;
        private Guna2Button btnBangDiem;
        private Guna2Button btnThongKe;
        private Guna2Button btnXuatFile;
        private bool cheDoThongKe = false;

        private static readonly Color MauChinh = Color.FromArgb(94, 22, 110);
        private static readonly Color MauNhat = Color.FromArgb(243, 229, 250);
        private static readonly Color MauNen = Color.FromArgb(250, 243, 253);
        private static readonly Color MauChu = Color.FromArgb(60, 20, 70);

        public UcBaoCao()
        {
            XayDungGiaoDien();
            Load += UcBaoCao_Load;
        }

        private void XayDungGiaoDien()
        {
            SuspendLayout();
            BackColor = MauNen;
            Padding = new Padding(24, 16, 24, 20);
            Font = new Font("Segoe UI", 9.5f);

            var lblTieuDe = new Label
            {
                Text = "Tổng hợp & Báo cáo",
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(16)
            };

            // Thanh công cụ chọn lớp và các nút chức năng
            var pnlTool = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.White
            };

            var lblChonLop = new Label
            {
                Text = "Lớp học phần:",
                Dock = DockStyle.Left,
                Width = 105,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = MauChu,
                Font = new Font("Segoe UI Semibold", 9.5f)
            };

            cboLopHP = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cboLopHP.SelectedIndexChanged += (s, e) => { if (cheDoThongKe) HienThiThongKe(); else HienThiBangDiem(); };

            btnBangDiem = new Guna2Button
            {
                Text = "Bảng điểm chi tiết",
                Dock = DockStyle.Right,
                Width = 145,
                BorderRadius = 8,
                FillColor = MauChinh,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btnBangDiem.Click += (s, e) => { cheDoThongKe = false; DoiTrangThaiNut(); HienThiBangDiem(); };

            btnThongKe = new Guna2Button
            {
                Text = "Thống kê tổng hợp",
                Dock = DockStyle.Right,
                Width = 145,
                BorderRadius = 8,
                FillColor = MauNhat,
                ForeColor = MauChinh,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btnThongKe.Click += (s, e) => { cheDoThongKe = true; DoiTrangThaiNut(); HienThiThongKe(); };

            btnXuatFile = new Guna2Button
            {
                Text = "Xuất Excel / CSV",
                Dock = DockStyle.Right,
                Width = 135,
                BorderRadius = 8,
                FillColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9f),
                Cursor = Cursors.Hand
            };
            btnXuatFile.Click += BtnXuatFile_Click;

            pnlTool.Controls.Add(cboLopHP);
            pnlTool.Controls.Add(lblChonLop);
            pnlTool.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White });
            pnlTool.Controls.Add(btnBangDiem);
            pnlTool.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White });
            pnlTool.Controls.Add(btnThongKe);
            pnlTool.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White });
            pnlTool.Controls.Add(btnXuatFile);

            lblTieuDeBaoCao = new Label
            {
                Dock = DockStyle.Top,
                Height = 38,
                Font = new Font("Segoe UI Semibold", 12f),
                ForeColor = MauChinh,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White
            };

            dgvBaoCao = new DataGridView
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

            var hs = dgvBaoCao.ColumnHeadersDefaultCellStyle;
            hs.BackColor = MauChinh;
            hs.ForeColor = Color.White;
            hs.SelectionBackColor = MauChinh;
            hs.SelectionForeColor = Color.White;
            hs.Font = new Font("Segoe UI Semibold", 9.5f);
            hs.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var cs = dgvBaoCao.DefaultCellStyle;
            cs.BackColor = Color.White;
            cs.ForeColor = MauChu;
            cs.Font = new Font("Segoe UI", 9.5f);
            cs.SelectionBackColor = Color.FromArgb(236, 214, 247);
            cs.SelectionForeColor = MauChu;
            cs.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs.NullValue = "—";

            dgvBaoCao.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 242, 252);
            dgvBaoCao.AlternatingRowsDefaultCellStyle.SelectionBackColor = cs.SelectionBackColor;
            dgvBaoCao.AlternatingRowsDefaultCellStyle.SelectionForeColor = cs.SelectionForeColor;

            card.Controls.Add(dgvBaoCao);
            card.Controls.Add(lblTieuDeBaoCao);
            card.Controls.Add(pnlTool);

            Controls.Add(card);
            Controls.Add(lblTieuDe);

            ResumeLayout(false);
        }

        private void DoiTrangThaiNut()
        {
            if (cheDoThongKe)
            {
                btnThongKe.FillColor = MauChinh;
                btnThongKe.ForeColor = Color.White;
                btnBangDiem.FillColor = MauNhat;
                btnBangDiem.ForeColor = MauChinh;
            }
            else
            {
                btnBangDiem.FillColor = MauChinh;
                btnBangDiem.ForeColor = Color.White;
                btnThongKe.FillColor = MauNhat;
                btnThongKe.ForeColor = MauChinh;
            }
        }

        private void UcBaoCao_Load(object sender, EventArgs e)
        {
            TaiDanhSachLopHP();
        }

        private void TaiDanhSachLopHP()
        {
            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var da = new SqlDataAdapter(@"
SELECT l.malophp AS MaLopHP, 
       (l.malophp + ' - ' + m.tenmon + ' (' + l.giangvien + ')') AS HienThi,
       m.tenmon AS TenMon,
       l.giangvien AS GiangVien
FROM lophocphan l
JOIN monhoc m ON m.mamon = l.mamon
ORDER BY l.malophp", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    cboLopHP.DataSource = dt;
                    cboLopHP.DisplayMember = "HienThi";
                    cboLopHP.ValueMember = "MaLopHP";

                    if (dt.Rows.Count > 0)
                    {
                        cboLopHP.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lớp học phần: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiBangDiem()
        {
            if (cboLopHP.SelectedValue == null) return;
            string maLopHP = cboLopHP.SelectedValue.ToString();
            string tenLop = cboLopHP.Text;

            lblTieuDeBaoCao.Text = "BẢNG ĐIỂM CHI TIẾT: " + tenLop;

            dgvBaoCao.Columns.Clear();
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "STT", HeaderText = "STT", FillWeight = 45 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSV", HeaderText = "Mã SV", FillWeight = 75 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ tên", FillWeight = 120 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LopSH", HeaderText = "Lớp SH", FillWeight = 75 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemCC", HeaderText = "Chuyên cần (10%)", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemGK", HeaderText = "Giữa kỳ (30%)", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemCK", HeaderText = "Cuối kỳ (60%)", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiemTongKet", HeaderText = "Điểm tổng kết", FillWeight = 85, DefaultCellStyle = { Format = "0.00" } });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "XepLoai", HeaderText = "Xếp loại", FillWeight = 80 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KetQua", HeaderText = "Kết quả", FillWeight = 80 });

            try
            {
                string sql = @"
SELECT ROW_NUMBER() OVER (ORDER BY sv.masv) AS STT,
       sv.masv AS MaSV,
       sv.tensv AS HoTen,
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
WHERE d.malophp = @maLopHP
ORDER BY sv.masv";

                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maLopHP", maLopHP);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        dgvBaoCao.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị bảng điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiThongKe()
        {
            if (cboLopHP.SelectedValue == null) return;
            string maLopHP = cboLopHP.SelectedValue.ToString();
            string tenLop = cboLopHP.Text;

            lblTieuDeBaoCao.Text = "BÁO CÁO THỐNG KÊ TỔNG HỢP: " + tenLop;

            dgvBaoCao.Columns.Clear();
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ChiTieu", HeaderText = "Chỉ tiêu đánh giá", FillWeight = 160 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GiaTri", HeaderText = "Số lượng / Điểm số", FillWeight = 80 });
            dgvBaoCao.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TyLe", HeaderText = "Tỷ lệ (%)", FillWeight = 80 });

            try
            {
                using (var conn = new SqlConnection(DbHelper.ConnStr))
                using (var cmd = new SqlCommand("SELECT masv, diemtongket, xeploai FROM diem WHERE malophp = @maLopHP", conn))
                {
                    cmd.Parameters.AddWithValue("@maLopHP", maLopHP);
                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);

                    int tongSV = dt.Rows.Count;
                    var rowsCoDiem = dt.AsEnumerable().Where(r => r["diemtongket"] != DBNull.Value).ToList();
                    int coDiem = rowsCoDiem.Count;
                    int chuaDiem = tongSV - coDiem;

                    int soDat = rowsCoDiem.Count(r => Convert.ToDecimal(r["diemtongket"]) >= 5.0m);
                    int soKhongDat = coDiem - soDat;

                    int xuatSac = rowsCoDiem.Count(r => Convert.ToString(r["xeploai"]) == "Xuất sắc");
                    int gioi = rowsCoDiem.Count(r => Convert.ToString(r["xeploai"]) == "Giỏi");
                    int kha = rowsCoDiem.Count(r => Convert.ToString(r["xeploai"]) == "Khá");
                    int trungBinh = rowsCoDiem.Count(r => Convert.ToString(r["xeploai"]) == "Trung bình");
                    int yeu = rowsCoDiem.Count(r => Convert.ToString(r["xeploai"]) == "Yếu");

                    decimal diemTB = coDiem > 0 ? rowsCoDiem.Average(r => Convert.ToDecimal(r["diemtongket"])) : 0;
                    decimal diemMax = coDiem > 0 ? rowsCoDiem.Max(r => Convert.ToDecimal(r["diemtongket"])) : 0;
                    decimal diemMin = coDiem > 0 ? rowsCoDiem.Min(r => Convert.ToDecimal(r["diemtongket"])) : 0;

                    var tableTK = new DataTable();
                    tableTK.Columns.Add("ChiTieu", typeof(string));
                    tableTK.Columns.Add("GiaTri", typeof(string));
                    tableTK.Columns.Add("TyLe", typeof(string));

                    Func<int, int, string> tinhTyLe = (n, tong) => tong > 0 ? (100.0 * n / tong).ToString("0.0") + "%" : "0%";

                    tableTK.Rows.Add("1. Tổng số sinh viên trong lớp", tongSV.ToString(), "100%");
                    tableTK.Rows.Add("2. Số sinh viên đã hoàn thành điểm", coDiem.ToString(), tinhTyLe(coDiem, tongSV));
                    tableTK.Rows.Add("3. Số sinh viên chưa có điểm", chuaDiem.ToString(), tinhTyLe(chuaDiem, tongSV));
                    tableTK.Rows.Add("--- KẾT QUẢ HỌC TẬP ---", "", "");
                    tableTK.Rows.Add("4. Số sinh viên ĐẠT (Tổng kết >= 5.0)", soDat.ToString(), tinhTyLe(soDat, coDiem));
                    tableTK.Rows.Add("5. Số sinh viên KHÔNG ĐẠT (Tổng kết < 5.0)", soKhongDat.ToString(), tinhTyLe(soKhongDat, coDiem));
                    tableTK.Rows.Add("--- XẾP LOẠI CHI TIẾT ---", "", "");
                    tableTK.Rows.Add("6. Loại Xuất sắc (>= 9.0)", xuatSac.ToString(), tinhTyLe(xuatSac, coDiem));
                    tableTK.Rows.Add("7. Loại Giỏi (8.0 - 8.9)", gioi.ToString(), tinhTyLe(gioi, coDiem));
                    tableTK.Rows.Add("8. Loại Khá (7.0 - 7.9)", kha.ToString(), tinhTyLe(kha, coDiem));
                    tableTK.Rows.Add("9. Loại Trung bình (5.0 - 6.9)", trungBinh.ToString(), tinhTyLe(trungBinh, coDiem));
                    tableTK.Rows.Add("10. Loại Yếu (< 5.0)", yeu.ToString(), tinhTyLe(yeu, coDiem));
                    tableTK.Rows.Add("--- THÔNG SỐ ĐIỂM SỐ ---", "", "");
                    tableTK.Rows.Add("11. Điểm trung bình cả lớp", diemTB.ToString("0.00"), "—");
                    tableTK.Rows.Add("12. Điểm cao nhất", diemMax.ToString("0.00"), "—");
                    tableTK.Rows.Add("13. Điểm thấp nhất", diemMin.ToString("0.00"), "—");

                    dgvBaoCao.DataSource = tableTK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính toán thống kê: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXuatFile_Click(object sender, EventArgs e)
        {
            if (dgvBaoCao.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất file.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*";
                string loai = cheDoThongKe ? "ThongKe" : "BangDiem";
                string maLop = cboLopHP.SelectedValue != null ? cboLopHP.SelectedValue.ToString() : "LopHP";
                sfd.FileName = string.Format("{0}_{1}_{2:yyyyMMdd_HHmm}.csv", loai, maLop, DateTime.Now);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();

                        // Thêm tiêu đề báo cáo
                        sb.AppendLine(lblTieuDeBaoCao.Text);
                        sb.AppendLine("Thời gian xuất: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        sb.AppendLine();

                        // Header cột
                        var headers = dgvBaoCao.Columns.Cast<DataGridViewColumn>().Select(c => "\"" + c.HeaderText.Replace("\"", "\"\"") + "\"");
                        sb.AppendLine(string.Join(",", headers));

                        // Dữ liệu dòng
                        foreach (DataGridViewRow row in dgvBaoCao.Rows)
                        {
                            var cells = row.Cells.Cast<DataGridViewCell>().Select(c =>
                            {
                                string val = Convert.ToString(c.Value ?? "");
                                return "\"" + val.Replace("\"", "\"\"") + "\"";
                            });
                            sb.AppendLine(string.Join(",", cells));
                        }

                        // Ghi file với UTF-8 BOM để Excel mở hiển thị đúng tiếng Việt
                        File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));

                        MessageBox.Show("Xuất file báo cáo thành công!\n" + sfd.FileName, "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
