namespace TfsApi.Utilities
{
    #region

    using System;
    using System.IO;

    #endregion

    internal static class StreamUtility
    {
        #region Public Methods and Operators

        public static byte[] ConvertStreamToByteArray(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static byte[] ReadByteArrayFromFilename(string filename)
        {
            byte[] result = null;

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                result = ConvertStreamToByteArray(fileStream);
            }
            catch (Exception ex)
            {
                throw new Exception("Writing to the path '" + filename + "' failed.", ex);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            return result;
        }

        public static void WriteToFileSystem(string filename, byte[] input)
        {
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                bw = new BinaryWriter(fs);
                using (var ms = new MemoryStream(input))
                {
                    bw.Write(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Writing to the path '" + filename + "' failed.", ex);
            }
            finally
            {
                if (bw != null)
                {
                    bw.Close();
                    bw.Dispose();
                }

                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static void WriteToFileSystem(string filename, Stream input)
        {
            WriteToFileSystem(filename, ConvertStreamToByteArray(input));
        }

        #endregion
    }
}