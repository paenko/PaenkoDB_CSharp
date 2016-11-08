using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class FileHandler
    {
        public static byte[] ReadFile(string localFilePath)
        {
            byte[] full;
            long lenght = new System.IO.FileInfo(localFilePath).Length;
            using (FileStream stream = new FileStream(localFilePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                full = reader.ReadBytes((int)lenght);
            }
            return full;
        }

        public static bool WriteFile(byte[] data, string localFilePath)
        {
            try
            {
                using (FileStream stream = new FileStream(localFilePath, FileMode.Append))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(data);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
