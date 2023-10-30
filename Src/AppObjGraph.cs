using System.Text;

namespace SingleHtmlAppBundler
{
    class AppObjGraph : AppObj
    {
        byte[] RawX;

        public AppObjGraph(string TagInfo_, int Depth_, string FileName_, string Raw_, AppObj Parent_) : base(TagInfo_, Depth_, FileName_, Raw_, Parent_)
        {
            RawX = CoreFile.GetFile(Raw_);
        }

        public override string GetProcessedRaw()
        {
            return "data:" + CoreFile.FileMimeType(Raw) + ";base64," + Convert.ToBase64String(RawX);
        }

        public override string GetProcessedRawBASE64(string FileExt, string DefinedType)
        {
            if (!Core.StringNotEmpty(DefinedType))
            {
                DefinedType = CoreFile.FileMimeType(FileExt);
            }
            return "data:" + DefinedType + ";base64," + Convert.ToBase64String(RawX);
        }

        public void BinSave(string FileDir_, string FileName_)
        {
            CoreFile.SetFile(FileDir_, FileName_, RawX);
        }
    }
}