using System.Text;

namespace SingleHtmlAppBundler
{
    class CoreFile
    {
        public static string FileExt(string FilePath)
        {
            int Pos = FilePath.LastIndexOf('.');
            if (Pos >= 0)
            {
                return FilePath.Substring(Pos + 1).ToLowerInvariant();
            }
            else
            {
                return "";
            }
        }

        public static int ValidFileName(string FileDir, string FileName)
        {
            if ((FileName == null) || (FileName == ""))
            {
                return -1;
            }
            if (File.Exists(Path.Combine(FileDir, FileName)))
            {
                return CodePreparation.GetPreparedMimeTypeNum(FileName);
            }
            return -1;
        }

        public static string FilePath(string FileDir, string FileName)
        {
            return Path.Combine(FileDir, FileName);
        }


        public static void SetFile(string FilePath, byte[] Contents)
        {
            string FilePath_D = Path.GetDirectoryName(FilePath);
            if (!("".Equals(FilePath_D)))
            {
                if (!Directory.Exists(FilePath_D))
                {
                    Directory.CreateDirectory(FilePath_D);
                }
            }
            File.WriteAllBytes(FilePath, Contents);
        }

        public static void SetFile(string FileDir, string FileName, byte[] Contents)
        {
            SetFile(Path.Combine(FileDir, FileName), Contents);
        }

        public static void SetFile(string FileDir, string FileName, string Contents)
        {
            SetFile(FileDir, FileName, Core.WorkEncodingO.GetBytes(Contents));
        }


        public static byte[] GetFile(string FilePath)
        {
            return File.ReadAllBytes(FilePath);
        }

        public static byte[] GetFile(string FileDir, string FileName)
        {
            return File.ReadAllBytes(Path.Combine(FileDir, FileName));
        }

        public static string GetFileS(string FilePath)
        {
            byte[] Raw = GetFile(FilePath);
            if ((Raw[0] == 0xEF) && (Raw[1] == 0xBB) && (Raw[2] == 0xBF))
            {
                byte[] Raw_ = new byte[Raw.Length - 3];
                for (int i = 0; i < Raw_.Length; i++)
                {
                    Raw_[i] = Raw[i + 3];
                }
                Raw = Raw_;
            }
            return Core.WorkEncodingI.GetString(Raw);
        }

        public static string GetFileS(string FileDir, string FileName)
        {
            byte[] Raw = GetFile(FileDir, FileName);
            if ((Raw[0] == 0xEF) && (Raw[1] == 0xBB) && (Raw[2] == 0xBF))
            {
                byte[] Raw_ = new byte[Raw.Length - 3];
                for (int i = 0; i < Raw_.Length; i++)
                {
                    Raw_[i] = Raw[i + 3];
                }
                Raw = Raw_;
            }
            return Core.WorkEncodingI.GetString(Raw);
        }

        public static int FileMimeTypeNum(string FileName)
        {
            if (MimeTypeNum.ContainsKey(FileExt(FileName)))
            {
                return MimeTypeNum[FileExt(FileName)];
            }
            return 0;
        }

        public static string FileMimeType(string FileName)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
            string T = "*";
            if (MimeTypeString.ContainsKey(FileExt(FileName)))
            {
                T = MimeTypeString[FileExt(FileName)];
            }

            if ("*".Equals(T))
            {
                return HtmlAgilityPack.HtmlWeb.GetContentTypeForExtension(FileExt(FileName),  MimeTypeString["*"]);
            }
            else
            {
                return T;
            }
        }

        static Dictionary<string, string> MimeTypeString = new Dictionary<string, string>();
        static Dictionary<string, int> MimeTypeNum = new Dictionary<string, int>();

        static CoreFile()
        {
            MimeTypeString.Clear();
            MimeTypeNum.Clear();
            try
            {
                string FileRaw = GetFileS(AppDir() + "mime.txt");
                FileRaw = FileRaw.Replace("\r\n", "\n");
                FileRaw = FileRaw.Replace("\r", "\n");
                string[] FileRaw_ = FileRaw.Split('\n');
                for (int I = 0; I < FileRaw_.Length; I++)
                {
                    FileRaw_[I] = FileRaw_[I].Trim().Replace(" ", "\t");
                    int L = FileRaw_[I].Length;
                    while (true)
                    {
                        FileRaw_[I] = FileRaw_[I].Replace("\t\t", "\t");
                        if (L == FileRaw_[I].Length)
                        {
                            break;
                        }
                        else
                        {
                            L = FileRaw_[I].Length;
                        }
                    }
                    string[] Item = FileRaw_[I].Split('\t');
                    if (Item.Length >= 3)
                    {
                        MimeTypeString.Add(Item[0].Trim(), Item[1].Trim());
                        MimeTypeNum.Add(Item[0].Trim(), int.Parse(Item[2].Trim()));
                    }
                }
            }
            catch (Exception R)
            {
            }
            if (!MimeTypeString.ContainsKey("*"))
            {
                MimeTypeString.Add("*", "application/octet-stream");
                MimeTypeNum.Add("*", 0);
            }
        }

        public static string AppDir()
        {
            string Dir = AppDomain.CurrentDomain.BaseDirectory;
            if (!Dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                Dir = Dir + Path.DirectorySeparatorChar.ToString();
            }
            return Dir;
        }
    }
}