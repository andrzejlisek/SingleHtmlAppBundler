using System.Text;

namespace SingleHtmlAppBundler
{
    class AppObjGraph : AppObj
    {
        byte[] RawX;

        public AppObjGraph(string FileName_, AppObj Parent_) : base(FileName_, Parent_)
        {
            RawX = CoreFile.GetFile(FileName_);
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