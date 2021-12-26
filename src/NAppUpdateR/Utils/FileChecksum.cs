using System.Security.Cryptography;

namespace NAppUpdateR.Utils
{
    public static class FileChecksum
    {
        public static string GetSHA256Checksum(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                using var sha = SHA256.Create();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty);
            }
        }

        public static string GetSHA256Checksum(byte[] fileData)
        {
            using var sha = SHA256.Create();
            byte[] checksum = sha.ComputeHash(fileData);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }
    }
}
