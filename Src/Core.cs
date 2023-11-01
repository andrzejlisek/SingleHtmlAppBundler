using System.Runtime.CompilerServices;
using System.Text;

namespace SingleHtmlAppBundler
{
    class Core
    {
        public static Encoding WorkEncodingI = Encoding.UTF8;
        public static Encoding WorkEncodingO = Encoding.UTF8;

        public static string CodePreparationFile = "";

        public static bool UseXHTML = false;

        public static bool MinifyHTML_Comment = false;
        public static bool MinifyHTML_Whitespace = false;
        public static bool MinifyJS_Comment = false;
        public static bool MinifyJS_Whitespace = false;

        public static bool BundleHTML_Body = false;
        public static bool BundleHTML_Script = false;
        public static bool BundleHTML_Link = false;
        public static bool BundleHTML_Iframe = false;
        public static bool BundleHTML_Img = false;
        public static bool BundleHTML_Audio = false;
        public static bool BundleHTML_Video = false;
        public static bool BundleHTML_Source = false;
        public static bool BundleHTML_Track = false;
        public static bool BundleHTML_Object = false;
        public static bool BundleHTML_Embed = false;

        public static bool BundleJS_Url = false;
        public static int BundleJS_Worker = 0;
        public static int BundleJS_SharedWorker = 0;
        public static int BundleJS_Fetch = 0;
        public static int BundleJS_ImportScripts = 0;


        public Core()
        {
        }


        public static string BaseDirI = "";
        public static string BaseDirO = "";


        public void WriteMimes(string FileI_, string FileO_)
        {
            string[] ExtList = (FileI_ + "," + FileO_).Replace(";", ",").Split(',');
            int ExtL = 0;
            int TypeL = 0;
            for (int I = 0; I < ExtList.Length; I++)
            {
                ExtL = Math.Max(ExtL, ExtList[I].Length);
                TypeL = Math.Max(TypeL, CoreFile.FileMimeType("X." + ExtList[I]).Length);
            }
            for (int I = 0; I < ExtList.Length; I++)
            {
                Console.Write(ExtList[I].PadRight(ExtL));
                Console.Write("  ");
                Console.Write(CoreFile.FileMimeType("X." + ExtList[I]).PadRight(TypeL));
                Console.Write("  ");
                switch (CoreFile.FileMimeTypeNum("X." + ExtList[I]))
                {
                    case 0: Console.Write("Binary"); break;
                    case 1: Console.Write("HTML"); break;
                    case 2: Console.Write("JS or CSS"); break;
                }
                Console.WriteLine();
            }
        }

        public static List<string> SavedFiles = new List<string>();

        public void Start(string FileI_, string FileO_, int MaxDepth)
        {
            GlobalId.Clear();

            string UserFileName = Path.GetFileName(FileI_);
            BaseDirO = FileO_;
            BaseDirI = Path.GetDirectoryName(FileI_);

            AppObj AppObj_ = AppObj.CreateAppObj("ROOT", 0, BaseDirI, UserFileName, null);
            if (AppObj_ != null)
            {
                SavedFiles.Clear();
                AppObj_.Process(BaseDirO, MaxDepth);
            }
        }

        public static bool StringNotEmpty(string S)
        {
            if (S == null)
            {
                return false;
            }
            if (S.Trim() == "")
            {
                return false;
            }
            return true;
        }
    }
}