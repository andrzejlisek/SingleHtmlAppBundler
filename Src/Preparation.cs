using System.Diagnostics;

namespace SingleHtmlAppBundler
{
    /// <summary>
    /// Configuration file
    /// </summary>
    public class CodePreparation
    {
        static Dictionary<string, List<string>> ReplaceFrom = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> ReplaceTo = new Dictionary<string, List<string>>();
        static Dictionary<string, List<int>> ReplaceNum = new Dictionary<string, List<int>>();
        static Dictionary<string, List<bool>> ReplaceSave = new Dictionary<string, List<bool>>();
        static Dictionary<string, string> FileMimeType = new Dictionary<string, string>();
        static Dictionary<string, int> FileMimeTypeN = new Dictionary<string, int>();

        public static void Init(string FileName)
        {
            ReplaceFrom.Clear();
            ReplaceTo.Clear();
            ReplaceNum.Clear();

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
                int _TextNum = CF.ParamGetI("Replace#Number".Replace("#", I.ToString()));
                bool _TextSave = CF.ParamGetB("Replace#OnSave".Replace("#", I.ToString()));
                
                if (!("".Equals(_File)))
                {
                    if (!ReplaceFrom.ContainsKey(_File))
                    {
                        ReplaceFrom.Add(_File, new List<string>());
                        ReplaceTo.Add(_File, new List<string>());
                        ReplaceNum.Add(_File, new List<int>());
                        ReplaceSave.Add(_File, new List<bool>());
                    }

                    ReplaceFrom[_File].Add(_TextFrom);
                    ReplaceTo[_File].Add(_TextTo);
                    ReplaceNum[_File].Add(_TextNum);
                    ReplaceSave[_File].Add(_TextSave);
                }
                I++;
            }

            I = 1;
            while (!("".Equals(CF.ParamGetS("Mime#File".Replace("#", I.ToString())))))
            {
                string MimeFile_ = ConfigFile.MultilineDecode(CF.ParamGetS("Mime#File".Replace("#", I.ToString())));
                string MimeType_ = ConfigFile.MultilineDecode(CF.ParamGetS("Mime#Type".Replace("#", I.ToString())));
                int MimeTypeN_ = CF.ParamGetI("Mime#Data".Replace("#", I.ToString()));

                if (!FileMimeType.ContainsKey(MimeFile_))
                {
                    FileMimeType.Add(MimeFile_, MimeType_);
                    FileMimeTypeN.Add(MimeFile_, MimeTypeN_);
                }

                I++;
            }
        }

        public static string Prepare(string FileName, string Code, bool ProcessOnSave)
        {
            if (ReplaceFrom.ContainsKey(FileName))
            {
                for (int I = 0; I < ReplaceFrom[FileName].Count; I++)
                {
                    if (ReplaceSave[FileName][I] == ProcessOnSave)
                    {
                        int N = ReplaceNum[FileName][I];
                        if ("".Equals(ReplaceFrom[FileName][I]))
                        {
                            if ("".Equals(ReplaceTo[FileName][I]))
                            {
                                if (N > 0)
                                {
                                    if (Code.Length > N)
                                    {
                                        Code = Code.Substring(N);
                                    }
                                    else
                                    {
                                        Code = "";
                                    }
                                }
                                if (N < 0)
                                {
                                    if (Code.Length > (0 - N))
                                    {
                                        Code = Code.Substring(0, Code.Length - (0 - N));
                                    }
                                    else
                                    {
                                        Code = "";
                                    }
                                }
                            }
                            else
                            {
                                while (N > 0)
                                {
                                    Code = ReplaceTo[FileName][I] + Code;
                                    N--;
                                }
                                while (N < 0)
                                {
                                    Code = Code + ReplaceTo[FileName][I];
                                    N++;
                                }
                            }
                        }
                        else
                        {
                            string Fnd = ReplaceFrom[FileName][I];
                            string Rpl = ReplaceTo[FileName][I];
                            if (N == 0)
                            {
                                Code = Code.Replace(Fnd, Rpl);
                            }
                            while (N > 0)
                            {
                                int NN = Code.IndexOf(Fnd);
                                if (NN >= 0)
                                {
                                    string Code1 = (NN > 0) ? Code.Substring(0, NN) : "";
                                    string Code2 = (NN + Fnd.Length) < Code.Length ? Code.Substring(NN + Fnd.Length) : "";
                                    Code = Code1 + Rpl + Code2;
                                }
                                N--;
                            }
                            while (N < 0)
                            {
                                int NN = Code.LastIndexOf(Fnd);
                                if (NN >= 0)
                                {
                                    string Code1 = (NN > 0) ? Code.Substring(0, NN) : "";
                                    string Code2 = (NN + Fnd.Length) < Code.Length ? Code.Substring(NN + Fnd.Length) : "";
                                    Code = Code1 + Rpl + Code2;
                                }
                                N++;
                            }
                        }
                    }
                }
            }
            return Code;
        }

        public static string GetPreparedMimeType(string FileName)
        {
            if (FileMimeType.ContainsKey(FileName))
            {
                return FileMimeType[FileName];
            }
            else
            {
                return CoreFile.FileMimeType(FileName);
            }
        }

        public static int GetPreparedMimeTypeNum(string FileName)
        {
            if (FileMimeTypeN.ContainsKey(FileName))
            {
                return FileMimeTypeN[FileName];
            }
            else
            {
                return CoreFile.FileMimeTypeNum(FileName);
            }
        }
    }
}