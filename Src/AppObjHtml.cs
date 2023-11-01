using System.Text;
using HtmlAgilityPack;

namespace SingleHtmlAppBundler
{
    class AppObjHtml : AppObj
    {
        HtmlDocument HDoc;


        static List<string> ExceptionNoProcessText = new List<string>();
        static List<string> ExceptionScript = new List<string>();
        static List<string> ExceptionSingleTag = new List<string>();

        static AppObjHtml()
        {
            ExceptionSingleTag.Add("area");
            ExceptionSingleTag.Add("base");
            ExceptionSingleTag.Add("basefont");
            ExceptionSingleTag.Add("br");
            ExceptionSingleTag.Add("col");
            ExceptionSingleTag.Add("command");
            ExceptionSingleTag.Add("embed");
            ExceptionSingleTag.Add("frame");
            ExceptionSingleTag.Add("hr");
            ExceptionSingleTag.Add("img");
            ExceptionSingleTag.Add("input");
            ExceptionSingleTag.Add("keygen");
            ExceptionSingleTag.Add("link");
            ExceptionSingleTag.Add("meta");
            ExceptionSingleTag.Add("param");
            ExceptionSingleTag.Add("source");
            ExceptionSingleTag.Add("track");
            ExceptionSingleTag.Add("wbr");
            ExceptionNoProcessText.Add("pre");
            ExceptionNoProcessText.Add("textarea");
            ExceptionScript.Add("script");
            ExceptionScript.Add("style");
        }

        public AppObjHtml(string TagInfo_, int Depth_, string FileName_, string Raw_, AppObj Parent_) : base(TagInfo_, Depth_, FileName_, Raw_, Parent_)
        {
        }

        List<HtmlNode> ChildNode = new List<HtmlNode>();

        public override void Parse(int MaxDepth)
        {
            base.Parse(MaxDepth);
            if (MaxDepth == 0)
            {
                Console.WriteLine("The depth limit is reached, the " + FileName + " will not be analyzed.");
                return;
            }
            HDoc = new HtmlDocument();
            HDoc.LoadHtml(Raw);
            ParseWork(HDoc.DocumentNode, 0, MaxDepth);
        }

        void ParseWork(HtmlAgilityPack.HtmlNode N, int TextMode, int MaxDepth)
        {
            string NodeFileName = null;
            string TagName = "???";
            switch (N.NodeType)
            {
                case HtmlAgilityPack.HtmlNodeType.Document:
                    for (int i = 0; i < N.ChildNodes.Count; i++)
                    {
                        ParseWork(N.ChildNodes[i], 0, MaxDepth);
                    }
                    break;
                case HtmlAgilityPack.HtmlNodeType.Element:

                    switch (N.Name.ToLowerInvariant())
                    {
                        case "img":
                        case "script":
                        case "iframe":
                        case "embed":
                        case "source":
                        case "track":
                        case "audio":
                        case "video":
                            {
                                if ((N.Attributes["srcset"] != null) && (N.Attributes["src"] == null))
                                {
                                    NodeFileName = N.Attributes["srcset"].Value;
                                    TagName = N.Name.ToLowerInvariant();
                                }
                                if (N.Attributes["src"] != null)
                                {
                                    NodeFileName = N.Attributes["src"].Value;
                                    TagName = N.Name.ToLowerInvariant();
                                }
                            }
                            break;
                        case "body":
                            {
                                if (N.Attributes["background"] != null)
                                {
                                    NodeFileName = N.Attributes["background"].Value;
                                    TagName = N.Name.ToLowerInvariant();
                                }
                            }
                            break;
                        case "link": // <link rel="stylesheet" type="text/css" href="style.css"/>
                            {
                                if ((N.Attributes["rel"] != null) && (N.Attributes["href"] != null))
                                {
                                    NodeFileName = N.Attributes["href"].Value;
                                    TagName = N.Name.ToLowerInvariant();
                                }
                            }
                            break;
                        case "object":
                            {
                                if (N.Attributes["data"] != null)
                                {
                                    NodeFileName = N.Attributes["data"].Value;
                                    TagName = N.Name.ToLowerInvariant();
                                }
                            }
                            break;
                    }
                    break;
                case HtmlAgilityPack.HtmlNodeType.Text:
                    int ProcessText = 1;
                    if (N.ParentNode != null)
                    {
                        if (AppObjHtml.ExceptionNoProcessText.Contains(N.ParentNode.Name))
                        {
                            ProcessText = 0;
                        }
                        if (AppObjHtml.ExceptionScript.Contains(N.ParentNode.Name))
                        {
                            ProcessText = 2;
                        }
                    }

                    string Txt = N.InnerText;
                    switch (ProcessText)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            {
                                if ((Txt != null) && (Txt.Trim() != ""))
                                {
                                    TagName = N.ParentNode.Name;
                                    AppObj AppObj_ = new AppObjJS("HTML:" + TagName, Depth + 1, "", Txt, null);
                                    Child.Add(AppObj_);
                                    ChildNode.Add(N);
                                    AppObj_.Parse(MaxDepth - 1);
                                }
                            }
                            break;
                    }
                    break;
                case HtmlNodeType.Comment:
                    break;
            }

            if (CoreFile.ValidFileName(Core.BaseDirI, NodeFileName) >= 0)
            {
                AppObj AppObj_ = CreateAppObj("HTML:" + TagName, Depth + 1, Core.BaseDirI, NodeFileName, this);
                if (AppObj_ != null)
                {
                    Child.Add(AppObj_);
                    ChildNode.Add(N);
                    AppObj_.Parse(MaxDepth - 1);
                }
            }

            if (N.NodeType == HtmlNodeType.Element)
            {
                if (!ExceptionSingleTag.Contains(N.Name.ToLowerInvariant()))
                {
                    if (N.HasChildNodes)
                    {
                        for (int i = 0; i < N.ChildNodes.Count; i++)
                        {
                            ParseWork(N.ChildNodes[i], 0, MaxDepth);
                        }
                    }
                }
            }
        }


        public override string GetProcessedRaw()
        {
            StringBuilder SB = new StringBuilder();
            CreateHtmlText(SB, HDoc.DocumentNode, 0);
            return CodePreparation.Prepare(FileName, SB.ToString(), true);
        }


        public override void TreeOperationData()
        {
            for (int I = 0; I < Child.Count; I++)
            {
                switch (ChildNode[I].Name)
                {
                    case "body":
                        if (Core.BundleHTML_Body)
                        {
                            ChildNode[I].Attributes["background"].Value = Child[I].GetProcessedRawBASE64("");
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "img":
                        if (Core.BundleHTML_Img)
                        {
                            ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64("");
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "iframe":
                        if (Core.BundleHTML_Iframe)
                        {
                            ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64("");
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "embed":
                        if (Core.BundleHTML_Embed)
                        {
                            string T = "";
                            if (ChildNode[I].Attributes["type"] != null)
                            {
                                T = ChildNode[I].Attributes["type"].Value;
                            }
                            ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64(T);
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "source":
                        if (Core.BundleHTML_Source)
                        {
                            string T = "";
                            if (ChildNode[I].Attributes["type"] != null)
                            {
                                T = ChildNode[I].Attributes["type"].Value;
                            }
                            if ((ChildNode[I].Attributes["srcset"] != null) && (ChildNode[I].Attributes["src"] == null))
                            {
                                ChildNode[I].Attributes["srcset"].Value = Child[I].GetProcessedRawBASE64(T);
                                Child[I].IsBundled = true;
                            }
                            if (ChildNode[I].Attributes["src"] != null)
                            {
                                ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64(T);
                                Child[I].IsBundled = true;
                            }
                        }
                        break;
                    case "track":
                        if (Core.BundleHTML_Track)
                        {
                            string T = "";
                            if (ChildNode[I].Attributes["type"] != null)
                            {
                                T = ChildNode[I].Attributes["type"].Value;
                            }
                            ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64(T);
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "object":
                        if (Core.BundleHTML_Object)
                        {
                            ChildNode[I].Attributes["data"].Value = Child[I].GetProcessedRawBASE64("");
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "audio":
                        if (Core.BundleHTML_Audio)
                        {
                            ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64("");
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "video":
                        if (Core.BundleHTML_Video)
                        {
                            ChildNode[I].Attributes["src"].Value = Child[I].GetProcessedRawBASE64("");
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "script":
                        if (Core.BundleHTML_Script)
                        {
                            ChildNode[I].Attributes.Remove("src");
                            HtmlTextNode NT = HDoc.CreateTextNode(Child[I].GetProcessedRaw());
                            while (ChildNode[I].ChildNodes.Count > 0)
                            {
                                ChildNode[I].ChildNodes.RemoveAt(0);
                            }
                            ChildNode[I].ChildNodes.Add(NT);
                            Child[I].IsBundled = true;
                        }
                        break;
                    case "link":
                        {
                            if (Core.BundleHTML_Link)
                            {
                                if (ChildNode[I].Attributes["rel"].Value == "stylesheet")
                                {
                                    switch (ChildNode[I].OriginalName)
                                    {
                                        default:
                                            ChildNode[I].Name = "style";
                                            break;
                                        case "Link":
                                            ChildNode[I].Name = "Style";
                                            break;
                                        case "LINK":
                                            ChildNode[I].Name = "STYLE";
                                            break;
                                    }
                                    ChildNode[I].Attributes.RemoveAll();
                                    HtmlTextNode NT = HDoc.CreateTextNode(Child[I].GetProcessedRaw());
                                    ChildNode[I].ChildNodes.Add(NT);
                                }
                                else
                                {
                                    ChildNode[I].Attributes["href"].Value = Child[I].GetProcessedRawBASE64("");
                                }
                                Child[I].IsBundled = true;
                            }
                        }
                        break;
                    case "#text":
                        {
                            ((HtmlTextNode)ChildNode[I]).Text = Child[I].GetProcessedRaw();
                            Child[I].IsBundled = true;
                        }
                        break;
                }
            }
        }

        void CreateHtmlText(StringBuilder SB, HtmlAgilityPack.HtmlNode N, int TextMode)
        {
            switch (N.NodeType)
            {
                case HtmlAgilityPack.HtmlNodeType.Document:
                    for (int i = 0; i < N.ChildNodes.Count; i++)
                    {
                        CreateHtmlText(SB, N.ChildNodes[i], 0);
                    }
                    break;
                case HtmlAgilityPack.HtmlNodeType.Element:

                    {
                        SB.Append("<");
                        SB.Append(N.OriginalName);
                        foreach (HtmlAgilityPack.HtmlAttribute item in N.Attributes)
                        {
                            SB.Append(" ");
                            SB.Append(item.OriginalName);
                            if ((item.ValueStartIndex > 0) || (Core.StringNotEmpty(item.Value)))
                            {
                                SB.Append("=\"");
                                string V = item.Value;
                                /*V = V.Replace("&", "&amp;");
                                V = V.Replace("\"", "&quot;");
                                V = V.Replace("'", "&apos;");
                                V = V.Replace("<", "&lt;");
                                V = V.Replace(">", "&gt;");*/
                                SB.Append(V);
                                SB.Append("\"");
                            }
                            else
                            {
                                if (Core.UseXHTML)
                                {
                                    SB.Append("=\"");
                                    SB.Append(item.OriginalName);
                                    SB.Append("\"");
                                }
                            }
                        }
                        if (AppObjHtml.ExceptionSingleTag.Contains(N.Name.ToLowerInvariant()))
                        {
                            if (Core.UseXHTML)
                            {
                                SB.Append(" />");
                            }
                            else
                            {
                                SB.Append(">");
                            }
                        }
                        else
                        {
                            SB.Append(">");
                            if (N.HasChildNodes)
                            {
                                for (int i = 0; i < N.ChildNodes.Count; i++)
                                {
                                    CreateHtmlText(SB, N.ChildNodes[i], 0);
                                }
                            }
                            SB.Append("</");
                            SB.Append(N.OriginalName);
                            SB.Append(">");
                        }
                    }
                    break;
                case HtmlAgilityPack.HtmlNodeType.Text:
                    int ProcessText = 1;
                    if (N.ParentNode != null)
                    {
                        if (AppObjHtml.ExceptionNoProcessText.Contains(N.ParentNode.Name))
                        {
                            ProcessText = 0;
                        }
                        if (AppObjHtml.ExceptionScript.Contains(N.ParentNode.Name))
                        {
                            ProcessText = 2;
                        }
                    }

                    string Txt = N.InnerText;
                    switch (ProcessText)
                    {
                        case 0:
                            {
                                SB.Append(Txt);
                            }
                            break;
                        case 1:
                            {
                                if (Core.MinifyHTML_Whitespace)
                                {
                                    Txt = Txt.Replace("\r\n", "\n");
                                    Txt = Txt.Replace("\r", "\n");
                                    Txt = Txt.Replace("\n", " ");
                                    Txt = Txt.Replace("\t", " ");
                                    while (Txt.Contains("  "))
                                    {
                                        Txt = Txt.Replace("  ", " ");
                                    }
                                }
                                SB.Append(Txt);
                            }
                            break;
                        case 2:
                            {
                                SB.Append(Txt);
                            }
                            break;
                    }
                    break;
                case HtmlNodeType.Comment:
                    {
                        Txt = ((HtmlCommentNode)N).Comment;
                        if ((!Core.MinifyHTML_Comment) || (Txt.ToUpperInvariant().StartsWith("<!DOCTYPE")))
                        {
                            SB.Append(Txt);
                        }
                    }
                    break;
            }
        }
    }
}