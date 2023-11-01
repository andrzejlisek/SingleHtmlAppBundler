using System;
using System.Collections.Generic;
using System.IO;

namespace SingleHtmlAppBundler
{
    /// <summary>
    /// Configuration file
    /// </summary>
    public class ConfigFile
    {
        public static string MultilineEncode(string X)
        {
            string XXX = "";
            bool Std = true;
            for (int I = 0; I < X.Length; I++)
            {
                char C = X[I];
                Std = true;
                if (C == '\\') { XXX += "\\\\"; Std = false; }
                if (C == '\"') { XXX += "\\\""; Std = false; }
                if (C == '\'') { XXX += "\\\'"; Std = false; }
                if (C == '\r') { XXX += "\\r"; Std = false; }
                if (C == '\n') { XXX += "\\n"; Std = false; }
                if (C == '\t') { XXX += "\\t"; Std = false; }
                if (C == '\v') { XXX += "\\v"; Std = false; }
                if (Std)
                {
                    XXX += C;
                }
            }
            return "\"" + XXX + "\"";
        }

        public static string MultilineDecode(string X)
        {
            string XXX = "";
            if (X.Length >= 2)
            {
                if ((X[0] == '\"') && (X[X.Length - 1] == '\"'))
                {
                    for (int I = 1; I < (X.Length - 1); I++)
                    {
                        char C = X[I];
                        if (C == '\\')
                        {
                            I++;
                            C = X[I];
                            if (C == '\\') { XXX += "\\"; }
                            if (C == '\"') { XXX += "\""; }
                            if (C == '\'') { XXX += "\'"; }
                            if (C == 'r') { XXX += "\r"; }
                            if (C == 'n') { XXX += "\n"; }
                            if (C == 't') { XXX += "\t"; }
                            if (C == 'v') { XXX += "\v"; }
                        }
                        else
                        {
                            XXX += C;
                        }
                    }
                    return XXX;
                }
                else
                {
                    return X;
                }
            }
            else
            {
                return X;
            }
        }

        private Dictionary<string, string> Raw = new Dictionary<string, string>();

        public void FileLoad(string FileName)
        {
            ParamClear();
            try
            {
                FileStream F_ = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                StreamReader F = new StreamReader(F_);
                while (!F.EndOfStream)
                {
                    string S = F.ReadLine();
                    int I = S.IndexOf("=");
                    if (I >= 0)
                    {
                        string RawK = S.Substring(0, I);
                        if (!Raw.ContainsKey(RawK))
                        {
                            if (S.Length > (I + 1))
                            {
                                Raw.Add(RawK, S.Substring(I + 1));
                            }
                            else
                            {
                                Raw.Add(RawK, "");
                            }
                        }
                    }
                }
                F.Close();
                F_.Close();
            }
            catch
            {

            }
        }

        public void FileSave(string FileName)
        {
            try
            {
                FileStream F_ = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                StreamWriter F = new StreamWriter(F_);
                foreach (KeyValuePair<string, string> item in Raw)
                {
                    F.Write(item.Key);
                    F.Write("=");
                    F.Write(item.Value);
                    F.WriteLine();
                }
                F.Close();
                F_.Close();
            }
            catch
            {

            }
        }

        public void ParamClear()
        {
            Raw.Clear();
        }

        public void ParamRemove(string Name)
        {
            if (Raw.ContainsKey(Name))
            {
                Raw.Remove(Name);
            }
        }

        public void ParamSet(string Name, string Value)
        {
            if (Raw.ContainsKey(Name))
            {
                Raw[Name] = Value;
            }
            else
            {
                Raw.Add(Name, Value);
            }
        }

        public void ParamSet(string Name, int Value)
        {
            ParamSet(Name, Value.ToString());
        }

        public void ParamSet(string Name, long Value)
        {
            ParamSet(Name, Value.ToString());
        }

        public void ParamSet(string Name, bool Value)
        {
            ParamSet(Name, Value ? "1" : "0");
        }

        public bool ParamGet(string Name, ref string Value)
        {
            if (Raw.ContainsKey(Name))
            {
                Value = Raw[Name];
                return true;
            }
            return false;
        }

        public bool ParamGet(string Name, ref int Value)
        {
            if (Raw.ContainsKey(Name))
            {
                try
                {
                    Value = int.Parse(Raw[Name]);
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }

        public bool ParamGet(string Name, ref long Value)
        {
            if (Raw.ContainsKey(Name))
            {
                try
                {
                    Value = long.Parse(Raw[Name]);
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }

        public bool ParamGet(string Name, ref bool Value)
        {
            if (Raw.ContainsKey(Name))
            {
                if ((Raw[Name] == "1") || (Raw[Name].ToUpperInvariant() == "TRUE") || (Raw[Name].ToUpperInvariant() == "YES") || (Raw[Name].ToUpperInvariant() == "T") || (Raw[Name].ToUpperInvariant() == "Y"))
                {
                    Value = true;
                    return true;
                }
                if ((Raw[Name] == "0") || (Raw[Name].ToUpperInvariant() == "FALSE") || (Raw[Name].ToUpperInvariant() == "NO") || (Raw[Name].ToUpperInvariant() == "F") || (Raw[Name].ToUpperInvariant() == "N"))
                {
                    Value = false;
                    return true;
                }
            }
            return false;
        }

        public string ParamGetS(string Name, string X)
        {
            ParamGet(Name, ref X);
            return X;
        }

        public int ParamGetI(string Name, int X)
        {
            ParamGet(Name, ref X);
            return X;
        }

        public long ParamGetL(string Name, long X)
        {
            ParamGet(Name, ref X);
            return X;
        }

        public bool ParamGetB(string Name, bool X)
        {
            ParamGet(Name, ref X);
            return X;
        }

        public string ParamGetS(string Name)
        {
            string X = "";
            ParamGet(Name, ref X);
            return X;
        }

        public int ParamGetI(string Name)
        {
            int X = 0;
            ParamGet(Name, ref X);
            return X;
        }

        public long ParamGetL(string Name)
        {
            long X = 0;
            ParamGet(Name, ref X);
            return X;
        }

        public bool ParamGetB(string Name)
        {
            bool X = false;
            ParamGet(Name, ref X);
            return X;
        }

        public bool ParamExists(string Name)
        {
            return Raw.ContainsKey(Name);
        }
    }
}