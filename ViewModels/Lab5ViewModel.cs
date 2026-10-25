using System.ComponentModel.DataAnnotations;

namespace Nawarut_8250_L005.ViewModels
{
    // ==========================================
    // 1. Login ViewModel (สำหรับหน้า Lab5)
    // ==========================================
    public class LoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    // ==========================================
    // 2. Register ViewModel (สำหรับหน้า Lab5_2)
    // ==========================================
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อ")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท์")]
        [Phone(ErrorMessage = "รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public string? Role { get; set; } = "Renter";
        public string? SlotNo { get; set; } = "A01";
        public string? Status { get; set; } = "Active";
    }

    // ==========================================
    // 3. User ViewModel (สำหรับแสดงผลผู้ใช้แต่ละคน)
    // ==========================================
    public class UserViewModel
    {
        public uint UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; } = "Active";
        public string? SlotNo { get; set; }
        public string? Role { get; set; }

        public string FullName => string.Join(" ", new[] { Firstname, Lastname }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    // ==========================================
    // 4. UserList ViewModel (สำหรับหน้า Lab5_3)
    // ==========================================
    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();

        private static UserListViewModel? _instance;
        private static readonly object _lock = new();

        public static UserListViewModel UsersDataList()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserListViewModel
                        {
                            Users = new List<UserViewModel>
                            {
                                new UserViewModel
                                {
                                    Firstname = "Anan",
                                    Lastname = "Sukjai",
                                    Email = "anan@example.com",
                                    Phone = "081-111-1111",
                                    Role = "Renter",
                                    SlotNo = "A01",
                                    Status = "Active"
                                },
                                new UserViewModel
                                {
                                    Firstname = "Mali",
                                    Lastname = "Jairak",
                                    Email = "mali@example.com",
                                    Phone = "082-222-2222",
                                    Role = "Staff",
                                    SlotNo = "-",
                                    Status = "Active"
                                },
                                new UserViewModel
                                {
                                    Firstname = "Narin",
                                    Lastname = "Dee",
                                    Email = "narin@example.com",
                                    Phone = "083-333-3333",
                                    Role = "Accountant",
                                    SlotNo = "-",
                                    Status = "Active"
                                },
                                new UserViewModel
                                {
                                    Firstname = "Supphanat",
                                    Lastname = "Deawwanich",
                                    Email = "supphanat@2.com",
                                    Phone = "084-444-4444",
                                    Role = "Renter",
                                    SlotNo = "-",
                                    Status = "Active"
                                }
                            }
                        };
                    }
                }
            }
            return _instance;
        }
    }

    // ==========================================
    // 5. Lab5ViewModel (คงไว้เพื่อความเข้ากันได้)
    // ==========================================
    public class Lab5ViewModel
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
    }

    // ==========================================
    // 6. EditUser ViewModel (สำหรับหน้าแก้ไขข้อมูลผู้ใช้ - ประยุกต์จาก Lab 10)
    // ==========================================
    public class EditUserViewModel
    {
        [Required]
        public uint UserId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อ")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง")]
        public string? Phone { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Role { get; set; } = "Renter";
        public string? SlotNo { get; set; } = "-";
        public string? Status { get; set; } = "active";
    }
}