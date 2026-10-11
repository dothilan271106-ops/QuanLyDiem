using Class_lop;

namespace CodeLogic
{
    /// <summary>Lưu người dùng đang đăng nhập.</summary>
    public static class PhienLamViec
    {
        public static TaiKhoanDTO NguoiDung { get; private set; }

        public static void DangNhap(TaiKhoanDTO tk) { NguoiDung = tk; }
        public static void DangXuat() { NguoiDung = null; }
    }
}
