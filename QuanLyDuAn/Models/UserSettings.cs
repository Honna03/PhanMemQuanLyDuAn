using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyDuAn.Models
{
    public class UserSettings
    {
        private const string EncryptionKey = "123"; // Khóa mã hóa (nên lưu ở nơi an toàn)

        public string Username { get; set; } // Tên người dùng (sẽ được mã hóa khi lưu)
       // public bool RememberMe { get; set; } // Trạng thái "Ghi nhớ tôi"
        public DateTime ExpiryDate { get; set; } // Thời gian hết hạn của "Ghi nhớ tôi"

        // Thuộc tính để lưu trữ tên người dùng đã mã hóa
        [Newtonsoft.Json.JsonIgnore] // Không lưu trực tiếp vào JSON
        private string _encryptedUsername;

        // Constructor mặc định
        public UserSettings()
        {
            Username = string.Empty;
           // RememberMe = false;
            ExpiryDate = DateTime.MinValue;
        }

        // Phương thức mã hóa tên người dùng
        public void EncryptUsername()
        {
            if (string.IsNullOrEmpty(Username))
            {
                _encryptedUsername = string.Empty;
                return;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32, '\0')); // Đảm bảo khóa dài 32 byte
                aes.IV = new byte[16]; // IV mặc định (có thể cải tiến thêm)

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] usernameBytes = Encoding.UTF8.GetBytes(Username);
                        cs.Write(usernameBytes, 0, usernameBytes.Length);
                    }
                    _encryptedUsername = Convert.ToBase64String(ms.ToArray());
                }
            }
            Username = _encryptedUsername; // Lưu giá trị đã mã hóa vào Username để serialize
        }

        // Phương thức giải mã tên người dùng
        public void DecryptUsername()
        {
            if (string.IsNullOrEmpty(Username))
            {
                _encryptedUsername = string.Empty;
                return;
            }

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32, '\0'));
                    aes.IV = new byte[16];

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(Username)))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new System.IO.StreamReader(cs))
                            {
                                _encryptedUsername = Username; // Lưu giá trị mã hóa
                                Username = sr.ReadToEnd(); // Giải mã và gán lại cho Username
                            }
                        }
                    }
                }
            }
            catch
            {
                Username = string.Empty; // Nếu giải mã thất bại, đặt lại Username
            }
        }

        // Kiểm tra xem "Ghi nhớ tôi" có còn hợp lệ không
       /* public bool IsValid()
        {
            if (!RememberMe)
                return false;

            return ExpiryDate > DateTime.Now;
        }*/

        // Đặt thời gian hết hạn (mặc định là 30 ngày)
        public void SetExpiryDate(int days = 30)
        {
            ExpiryDate = DateTime.Now.AddDays(days);
        }
    }
}