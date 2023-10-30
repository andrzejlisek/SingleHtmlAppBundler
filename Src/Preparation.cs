namespace SingleHtmlAppBundler
{
    /// <summary>
    /// Configuration file
    /// </summary>
    public class CodePreparation
    {
        static Dictionary<string, List<string>> ReplaceFrom = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> ReplaceTo = new Dictionary<string, List<string>>();


        public static void Init(string FileName)
        {
            ReplaceFrom.Clear();
            ReplaceTo.Clear();

            ConfigFile CF = new ConfigFile();
            if (!("".Equals(FileName)))
            {
                CF.FileLoad(FileName);
            }

            int I = 1;
            while (!("".Equals(CF.ParamGetS("Replace#File".Replace("#", I.ToString())))))
            {
                string _File = CF.ParamGetS("Replace#File".Replace("#", I.ToString()));

                string _TextFrom = ConfigFile.MultilineDecode(CF.ParamGetS("Replace#TextFrom".Replace("#", I.ToString())));
                string _TextTo = ConfigFile.MultilineDecode(CF.ParamGetS("Replace#TextTo".Replace("#", I.ToString())));
                if ((!("".Equals(_File))) && (!("".Equals(_TextFrom))))
                {
                    if (!ReplaceFrom.ContainsKey(_File))
                    {
                        ReplaceFrom.Add(_File, new List<string>());
                        ReplaceTo.Add(_File, new List<string>());
                    }

                    ReplaceFrom[_File].Add(_TextFrom);
                    ReplaceTo[_File].Add(_TextTo);
                }
                I++;
            }
        }

        public static string Prepare(string FileName, string Code)
        {
            if (ReplaceFrom.ContainsKey(FileName))
            {
                for (int I = 0; I < ReplaceFrom[FileName].Count; I++)
                {
                    Code = Code.Replace(ReplaceFrom[FileName][I], ReplaceTo[FileName][I]);
                }
            }
            return Code;
        }
    }
}