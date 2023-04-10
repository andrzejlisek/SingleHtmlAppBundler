using System.Text;

namespace SingleHtmlAppBundler
{
    class Core
    {
        public static Encoding WorkEncodingI = Encoding.UTF8;
        public static Encoding WorkEncodingO = Encoding.UTF8;

        public static bool UseXHTML = false;

        public static bool MinifyHTML_Comment = false;
        public static bool MinifyHTML_Whitespace = false;
        public static bool MinifyJS_Comment = false;
        public static bool MinifyJS_Whitespace = false;

        public static bool BundleHTML_Body = true;
        public static bool BundleHTML_Img = true;
        public static bool BundleHTML_Script = true;
        public static bool BundleHTML_Link = true;
        public static bool BundleHTML_Iframe = true;
        public static bool BundleHTML_Embed = true;
        public static bool BundleHTML_Source = true;
        public static bool BundleHTML_Track = true;
        public static bool BundleHTML_Object = true;
        public static bool BundleHTML_Audio = true;
        public static bool BundleHTML_Video = true;

        public static bool BundleJS_Worker = true;
        public static bool BundleJS_Url = true;


        public Core()
        {
        }


        public static string BaseDirI = "";
        public static string BaseDirO = "";

        public void Start(string FileI_, string FileO_)
        {
            GlobalId.Clear();

            string UserFileName = Path.GetFileName(FileI_);
            BaseDirO = FileO_;
            BaseDirI = Path.GetDirectoryName(FileI_);

            switch (CoreFile.ValidFileName(BaseDirI, UserFileName))
            {
                case 1:
                    {
                        string ScriptText = CoreFile.GetFileS(BaseDirI, UserFileName);
                        AppObjHtml AppObjHtml_ = new AppObjHtml(ScriptText, null);
                        AppObjHtml_.Process(0, BaseDirO, UserFileName);
                    }
                    break;
                case 2:
                    {
                        string ScriptText = CoreFile.GetFileS(BaseDirI, UserFileName);
                        AppObjJS AppObjJS_ = new AppObjJS(ScriptText, null);
                        AppObjJS_.Process(0, BaseDirO, UserFileName);
                    }
                    break;
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