using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Pocoyo
{
    public static class Utility
    {
        public static string AssemblyName => Assembly.GetEntryAssembly().GetName().Name;
        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static string MakeRelativeUrl(string filePath, string referencePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath + "\\.");
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }

        public static string MakeRelativePath(string filePath, string referencePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var relativeUrl = MakeRelativeUrl(filePath, referencePath);
            return relativeUrl.Replace("file://", "").Replace("/", "\\");
        }
    }

    public static class Log
    {
        public static bool VerbosMode { get; set; }
        public static bool SilentMode { get; set; }

        public static void Info(string message, params object[] args)
        {
            if (SilentMode) return;

            try
            {
                var argList = new List<object>(args);
                var msg = string.Format(message, argList.ToArray());
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                var msg = message.Replace("{", "[").Replace("}", "[");
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
        }

        public static void Info(string message)
        {
            if (SilentMode) return;

            try
            {
                var msg = $"{message}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
        }

        public static void Error(string message, params object[] args)
        {
            if (SilentMode) return;

            try
            {
                var argList = new List<object>(args);
                var msg = string.Format("ERROR:" + message, argList.ToArray());
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                var msg = "ERROR:" + message.Replace("{", "[").Replace("}", "[");
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public static void Error(string message)
        {
            if (SilentMode) return;

            try
            {
                var msg = $"ERROR: {message}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public static void Exception(string message, Exception ex)
        {
            if (SilentMode) return;

            try
            {
                var msg = $"EXCEPTION: {message} {ex}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public static void Exception(Exception ex)
        {
            if (SilentMode) return;

            try
            {
                var msg = $"EXCEPTION: {ex}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public static void Warn(string message, params object[] args)
        {
            if (SilentMode) return;

            try
            {
                var argList = new List<object>(args);
                var msg = string.Format("WARNING:" + message, argList.ToArray());
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                var msg = "WARNING:" + message.Replace("{", "[").Replace("}", "[");
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public static void Warn(string message)
        {
            if (SilentMode) return;

            try
            {
                var msg = $"WARNING: {message}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        public static void Verbose(string message, params object[] args)
        {
            if (SilentMode) return;

            try
            {
                if (!VerbosMode)
                    return;

                var argList = new List<object>(args);
                var msg = string.Format(message, argList.ToArray());
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                var msg = message.Replace("{", "[").Replace("}", "[");
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
        }

        public static void Verbose(string message)
        {
            if (SilentMode) return;

            try
            {
                if (!VerbosMode)
                    return;

                var msg = $"{message}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
        }
    }

    public static class FileLog
    {
        public static void Info(string filePath, string message)
        {
            try
            {
                var msg = $"{message}";
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                // Ignored
            }
        }

        public static void Info(string filePath, string message, params object[] args)
        {
            try
            {
                var argList = new List<object>(args);
                var msg = string.Format(message, argList.ToArray());
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            catch
            {
                var msg = message.Replace("{", "[").Replace("}", "[");
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
        }
    }

    public static class SharedFile
    {
        public static byte[] ReadAllBytes(string filePath, FileAccess fileAccess, FileShare fileShare)
        {
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, fileAccess, fileShare))
            {
                var buffer = new byte[fs.Length];
                var read = fs.Read(buffer, 0, (int)fs.Length);
                if (read != (int)fs.Length)
                    throw new Exception(
                        $"Read bytes error. Expected: {read} Actual: {fs.Length} from {filePath}");
                return buffer;
            }
        }

        public static string ReadAllText(string filePath, FileAccess fileAccess = FileAccess.ReadWrite, FileShare fileShare = FileShare.ReadWrite)
        {
            var buffer = ReadAllBytes(filePath, fileAccess, fileShare);
            return Encoding.UTF8.GetString(buffer);
        }

        public static bool WriteAllBytes(string filePath, byte[] buffer, SeekOrigin seek = SeekOrigin.Begin)
        {
            var folder = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            DateTime? createTime = null;
            try
            {
                if (!File.Exists(filePath))
                    createTime = DateTime.Now;
            }
            catch
            {
            }

            for (var idx = 0; idx < 3; idx++)
            {
                try
                {
                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        fs.Seek(0, seek);
                        fs.Write(buffer, 0, buffer.Length);
                        if (seek == SeekOrigin.Begin)
                            fs.SetLength(buffer.Length);
                        fs.Flush();
                    }

                    if (createTime != null)
                    {
                        // Set creation time correctly for new files
                        var fi = new FileInfo(filePath)
                        {
                            CreationTime = createTime.Value
                        };
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    string msg;
                    try
                    {
                        msg = Encoding.UTF8.GetString(buffer);
                    }
                    catch
                    {
                        msg = "BYTES";
                    }

                    Log.Info($"Exception: {filePath} {ex.Message} {buffer.Length} {msg.Length} {msg.Substring(0, Math.Min(100, msg.Length))}");
                }
            }

            return false;
        }

        public static bool WriteAllText(string filePath, string text, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var buffer = encoding.GetBytes(text);
            return WriteAllBytes(filePath, buffer);
        }

        public static void AppendAllText(string filePath, string text, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var buffer = encoding.GetBytes(text);
            WriteAllBytes(filePath, buffer, SeekOrigin.End);
        }

        public static void AppendLine(string filePath, string text, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var buffer = encoding.GetBytes(text + "\r\n");
            WriteAllBytes(filePath, buffer, SeekOrigin.End);
        }
    }

}
