using System.Text;

namespace SingleHtmlAppBundler
{
    class EncodingWork
    {
        static Dictionary<int, string> SystemEncodingNames;

        static EncodingWork()
        {
            SystemEncodingNames = new Dictionary<int, string>();
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                Encoding e = ei.GetEncoding();
                string EncName = "";
                List<string> EncNameL = new List<string>();
                EncNameL.Add(e.CodePage.ToString());
                if ((!EncNameL.Contains(ei.Name)) && (EncodingWork.EncodingCheckName(e, ei.Name)))
                {
                    EncName = EncName + ((EncNameL.Count == 1) ? "" : ", ") + ei.Name;
                    EncNameL.Add(ei.Name);
                }
                if ((!EncNameL.Contains(e.WebName)) && (EncodingWork.EncodingCheckName(e, e.WebName)))
                {
                    EncName = EncName + ((EncNameL.Count == 1) ? "" : ", ") + e.WebName;
                    EncNameL.Add(e.WebName);
                }
                if (EncName.Equals("") && (e.CodePage != 0))
                {
                    EncName = e.CodePage.ToString();
                }
                SystemEncodingNames.Add(e.CodePage, EncName);
            }
        }


        public static string EncodingGetName(Encoding E)
        {
            {
                if (SystemEncodingNames.ContainsKey(E.CodePage))
                {
                    return SystemEncodingNames[E.CodePage];
                }
                else
                {
                    return "UNKNOWN";
                }
            }
        }

        public static bool EncodingCheckName(Encoding E0, string Name)
        {
            try
            {
                Encoding E = Encoding.GetEncoding(Name);
                if (E0.Equals(E))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static Encoding EncodingFromName(string Name)
        {
            if ("".Equals(Name))
            {
                return Encoding.Default;
            }
            bool DigitOnly = true;
            for (int i = 0; i < Name.Length; i++)
            {
                if ((Name[i] < '0') || (Name[i] > '9'))
                {
                    DigitOnly = false;
                }
            }
            try
            {
                if (DigitOnly)
                {
                    return Encoding.GetEncoding(int.Parse(Name));
                }
                else
                {
                    return Encoding.GetEncoding(Name);
                }
            }
            catch
            {
                return Encoding.Default;
            }
        }
    }
}