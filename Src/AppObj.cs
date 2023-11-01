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
        public string FileMimeType = "";
        public int FileMimeTypeN = 0;
        public bool IsBundled = false;
        public AppObj Parent = null;
        public List<AppObj> Child = new List<AppObj>();
        public List<int> ChildPtr = new List<int>();
        public List<int> ChildOffset = new List<int>();

        public static AppObj CreateAppObj(string TagInfo_, int Depth_, string FileDir_, string FileName_, AppObj Parent_)
        {
            switch (CoreFile.ValidFileName(Core.BaseDirI, FileName_))
            {
                case 0: // Binary
                    return new AppObjBinary(TagInfo_, Depth_, FileName_, CoreFile.FilePath(FileDir_, FileName_), Parent_);
                case 1: // HTML
                    return new AppObjHtml(TagInfo_, Depth_, FileName_, CodePreparation.Prepare(FileName_, CoreFile.GetFileS(FileDir_, FileName_), false), Parent_);
                case 2: // JS, CSS
                    return new AppObjJS(TagInfo_, Depth_, FileName_, CodePreparation.Prepare(FileName_, CoreFile.GetFileS(FileDir_, FileName_), false), Parent_);
                default:
                    return null;
            }
        }


        public AppObj(string TagInfo_, int Depth_, string FileName_, string Raw_, AppObj Parent_)
        {
            IsBundled = false;
            TagInfo = TagInfo_.PadRight(18);
            Depth = Depth_;
            FileName = FileName_;
            Raw = Raw_;
            Parent = Parent_;
            FileMimeType = CodePreparation.GetPreparedMimeType(FileName);
            FileMimeTypeN = CodePreparation.GetPreparedMimeTypeNum(FileName);
        }

        public virtual void Parse(int MaxDepth)
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
            string FileNameS = Core.StringNotEmpty(FileName) ? (FileName + " - " + FileMimeType + " - " + FileMimeTypeN) : "<internal>";
            string BundledS = IsBundled ? " " : "* ";
            Console.WriteLine(TagInfo + BundledS + "".PadLeft(Depth * 2, ' ') + FileNameS);
            for (int I = 0; I < N.Child.Count; I++)
            {
                N.Child[I].TreeInfo(N.Child[I]);
            }
        }

        public string Process(string Dir, int MaxDepth)
        {
            Parse(MaxDepth);
            TreeOperation(this);
            TreeInfo(this);
            SaveFiles(Dir);
            return GetProcessedRaw();
        }

        public virtual string GetProcessedRaw()
        {
            return CodePreparation.Prepare(FileName, Raw, true);
        }

        public virtual string GetProcessedRawBASE64(string DefinedType)
        {
            if (Core.StringNotEmpty(DefinedType))
            {
                return "data:" + DefinedType + ";base64," + Convert.ToBase64String(Core.WorkEncodingO.GetBytes(GetProcessedRaw()));
            }
            else
            {
                return "data:" + FileMimeType + ";base64," + Convert.ToBase64String(Core.WorkEncodingO.GetBytes(GetProcessedRaw()));
            }
        }

        public void SaveFiles(string Dir)
        {
            if (Core.StringNotEmpty(FileName))
            {
                if (!IsBundled)
                {
                    if (!Core.SavedFiles.Contains(Dir + "|" + FileName))
                    {
                        if (this.GetType() == typeof(AppObjBinary))
                        {
                            ((AppObjBinary)this).BinSave(Dir, FileName);
                        }
                        else
                        {
                            CoreFile.SetFile(Dir, FileName, GetProcessedRaw());
                        }
                        Core.SavedFiles.Add(Dir + "|" + FileName);
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