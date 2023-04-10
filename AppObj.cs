using System.Text;

namespace SingleHtmlAppBundler
{
    class AppObj
    {
        static bool MergeHtml = false;

        public string Raw = "";
        public bool IsBundled = false;
        public AppObj Parent = null;
        public List<AppObj> Child = new List<AppObj>();
        public List<string> ChildName = new List<string>();
        public List<int> ChildPtr = new List<int>();

        public AppObj(string Raw_, AppObj Parent_)
        {
            IsBundled = false;
            Raw = Raw_;
            Parent = Parent_;
        }

        public virtual void Parse(int Depth, string Name)
        {
            if (Name != null)
            {
                Console.WriteLine("".PadLeft(Depth * 4, ' ') + Name);
            }
            else
            {
                Console.WriteLine("".PadLeft(Depth * 4, ' ') + "<internal>");
            }
        }


        public void TreeOperation()
        {
            TreeOperation(0, this);
        }

        public void TreeOperation(int Depth, AppObj N)
        {
            for (int I = 0; I < N.Child.Count; I++)
            {
                N.Child[I].TreeOperation(Depth + 1, N.Child[I]);
            }
            N.TreeOperationData(Depth);
        }

        public virtual void TreeOperationData(int Depth)
        {

        }


        public string Process(int Depth, string Dir, string Name)
        {
            Parse(Depth, Name);
            TreeOperation();
            SaveFiles(Depth, Dir, Name);
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

        public void SaveFiles(int Depth, string Dir, string Name)
        {
            if (IsBundled)
            {
                return;
            }
            if (Name != null)
            {
                if (this.GetType() == typeof(AppObjGraph))
                {
                    ((AppObjGraph)this).BinSave(Dir, Name);
                }
                else
                {
                    CoreFile.SetFile(Dir, Name, GetProcessedRaw());
                }
            }
            else
            {
                Depth--;
            }
            for (int I = 0; I < Child.Count; I++)
            {
                Child[I].SaveFiles(Depth + 1, Dir, ChildName[I]);
            }
        }
    }
}