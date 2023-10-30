using System.Text;

namespace SingleHtmlAppBundler
{
    class AppObj
    {
        static bool MergeHtml = false;

        public string TagInfo;
        public int Depth;
        public string FileName = "";
        public string Raw = "";
        public bool IsBundled = false;
        public AppObj Parent = null;
        public List<AppObj> Child = new List<AppObj>();
        public List<string> ChildName = new List<string>();
        public List<int> ChildPtr = new List<int>();

        public static AppObj CreateAppObj(string TagInfo_, int Depth_, string FileDir_, string FileName_, AppObj Parent_)
        {
            switch (CoreFile.ValidFileName(Core.BaseDirI, FileName_))
            {
                case 0: // Binary
                    return new AppObjGraph(TagInfo_, Depth_, FileName_, CoreFile.FilePath(FileDir_, FileName_), Parent_);
                case 1: // HTML
                    return new AppObjHtml(TagInfo_, Depth_, FileName_, CoreFile.GetFileS(FileDir_, FileName_), Parent_);
                case 2: // JS, CSS
                    return new AppObjJS(TagInfo_, Depth_, FileName_, CoreFile.GetFileS(FileDir_, FileName_), Parent_);
                default:
                    return null;
            }
        }

        public static string CodeToString(string Code)
        {
            Code = Code.Replace("\\", "\\\\");
            Code = Code.Replace("\r", "\\r");
            Code = Code.Replace("\n", "\\n");
            Code = Code.Replace("\t", "\\t");
            Code = Code.Replace("\v", "\\v");
            Code = Code.Replace("\"", "\\\"");
            Code = Code.Replace("\'", "\\\'");
            return Code;
        }

        public AppObj(string TagInfo_, int Depth_, string FileName_, string Raw_, AppObj Parent_)
        {
            IsBundled = false;
            TagInfo = TagInfo_.PadRight(15);
            Depth = Depth_;
            FileName = FileName_;
            Raw = Raw_;
            Parent = Parent_;
        }

        public virtual void Parse()
        {

        }


        public void TreeOperation(AppObj N)
        {
            for (int I = 0; I < N.Child.Count; I++)
            {
                N.Child[I].TreeOperation(N.Child[I]);
            }
            N.TreeOperationData();
        }

        public virtual void TreeOperationData()
        {

        }

        public void TreeInfo(AppObj N)
        {
            string FileNameS = Core.StringNotEmpty(FileName) ? FileName : "<internal>";
            string BundledS = IsBundled ? "" : " *";
            Console.WriteLine(TagInfo + "".PadLeft(Depth * 4, ' ') + FileNameS + BundledS);
            for (int I = 0; I < N.Child.Count; I++)
            {
                N.Child[I].TreeInfo(N.Child[I]);
            }
        }

        public string Process(string Dir)
        {
            Parse();
            TreeOperation(this);
            TreeInfo(this);
            SaveFiles(Dir);
            return GetProcessedRaw();
        }

        public virtual string GetProcessedRaw()
        {
            return Raw;
        }

        public virtual string GetProcessedRawBASE64(string FileExt, string DefinedType)
        {
            if (!Core.StringNotEmpty(DefinedType))
            {
                DefinedType = CoreFile.FileMimeType(FileExt);
            }
            return "data:" + DefinedType + ";base64," + Convert.ToBase64String(Core.WorkEncodingO.GetBytes(GetProcessedRaw()));
        }

        public void SaveFiles(string Dir)
        {
            if (Core.StringNotEmpty(FileName))
            {
                if (!IsBundled)
                {
                    if (this.GetType() == typeof(AppObjGraph))
                    {
                        ((AppObjGraph)this).BinSave(Dir, FileName);
                    }
                    else
                    {
                        CoreFile.SetFile(Dir, FileName, GetProcessedRaw());
                    }
                }
            }
            else
            {
                Depth--;
            }
            for (int I = 0; I < Child.Count; I++)
            {
                Child[I].SaveFiles(Dir);
            }
        }
    }
}