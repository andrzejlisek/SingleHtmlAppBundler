using System.Diagnostics;
using System.Security.Principal;
using System.Text;

namespace SingleHtmlAppBundler
{
    class AppObjJS : AppObj
    {
        bool CorrectTokenTypes = true;
        bool PrintTokenList = false;

        string ScriptCharsSpace = " \t\r\n";
        string ScriptCharsDigit = "0123456789";
        string ScriptCharsLetter = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
        string ScriptCharsOperatorRegEx = ",=({[!+-*/|&%^?<>";
        string ScriptCharsOperatorOther = ".:;)}]";

        public AppObjJS(string TagInfo_, int Depth_, string FileName_, string Raw_, AppObj Parent_) : base(TagInfo_, Depth_, FileName_, Raw_, Parent_)
        {

        }

        const string FetchPar1 = "_fetch_rsv_";
        const string FetchPar2 = "_fetch_rej_";

        const string FetchVar1 = "_fetch_a_";
        const string FetchVar2 = "_fetch_c_";
        const string FetchVar3 = "_fetch_i_";

        enum TokenTypeDef { Blank, Whitespace, ValueNum,
            ValueQ1, ValueQ2, ValueQ3,
            ValueQ1Esc, ValueQ2Esc, ValueQ3Esc,
            ValueQ1Exit, ValueQ2Exit, ValueQ3Exit,
            ValueRegEx, ValueRegExEnter, ValueRegExEsc, ValueRegExExit,
            Identifier, Operator,
            CommentEnter, CommentExit, CommentExit_, Comment1, Comment2 };

        List<string> TokenV = new List<string>();
        List<TokenTypeDef> TokenT = new List<TokenTypeDef>();


        List<string> TokenWorkV = new List<string>();
        List<TokenTypeDef> TokenWorkT = new List<TokenTypeDef>();
        List<int> TokenWorkN = new List<int>();


        public override void TreeOperationData()
        {
            for (int I = 0; I < Child.Count; I++)
            {
                int TokenPos = ChildPtr[I];
                int InsPos = 0;
                switch (TokenV[TokenPos])
                {
                    case "import":
                        {
                            // Module bundle is not implemented
                        }
                        break;
                    case "Worker":
                    case "SharedWorker":
                    case "importScripts":
                        {
                            // 1 - set BASE64 data as URL
                            // 2 - set text as URL object
                            int BundleType = 0;
                            switch (TokenV[TokenPos])
                            {
                                case "Worker": BundleType = Core.BundleJS_Worker; break;
                                case "SharedWorker": BundleType = Core.BundleJS_SharedWorker; break;
                                case "importScripts": BundleType = Core.BundleJS_ImportScripts; break;
                            }
                            if ((BundleType == 1) || (BundleType == 2))
                            {
                                InsPos = TokenPos + ChildOffset[I];
                                TokenRem(InsPos);
                                if (BundleType == 1)
                                {
                                    TokenIns(InsPos, "\"" + Child[I].GetProcessedRawBASE64("") + "\""); InsPos++;
                                }
                                if (BundleType == 2)
                                {
                                    TokenIns(InsPos, "URL"); InsPos++;
                                    TokenIns(InsPos, "."); InsPos++;
                                    TokenIns(InsPos, "createObjectURL"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "new"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, "Blob"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "["); InsPos++;
                                    TokenIns(InsPos, ConfigFile.MultilineEncode(Child[I].GetProcessedRaw())); InsPos++;
                                    TokenIns(InsPos, "]"); InsPos++;
                                    TokenIns(InsPos, ","); InsPos++;
                                    TokenIns(InsPos, "{"); InsPos++;
                                    TokenIns(InsPos, "type"); InsPos++;
                                    TokenIns(InsPos, ":"); InsPos++;
                                    TokenIns(InsPos, ConfigFile.MultilineEncode(Child[I].FileMimeType)); InsPos++;
                                    TokenIns(InsPos, "}"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                }
                                Child[I].IsBundled = true;
                            }
                        }
                        break;
                    case "url":
                        {
                            if (Core.BundleJS_Url)
                            {
                                if (ChildOffset[I] >= 0)
                                {
                                    InsPos = TokenPos + ChildOffset[I];
                                    TokenRem(InsPos);
                                }
                                else
                                {
                                    InsPos = TokenPos;
                                    int N = 0 - ChildOffset[I] - 2;
                                    while (TokenV[InsPos] != "(")
                                    {
                                        InsPos++;
                                        N--;
                                    }
                                    InsPos++;
                                    while (N > 0)
                                    {
                                        TokenRem(InsPos);
                                        N--;
                                    }
                                }
                                string RawBase64 = Child[I].GetProcessedRawBASE64("");
                                TokenIns(InsPos, "\"" + RawBase64 + "\"");
                                Child[I].IsBundled = true;
                            }
                        }
                        break;
                    case "fetch":
                        {
                            // 1 - set BASE64 data as URL
                            // 2 - replace fetch into building function
                            if ((Core.BundleJS_Fetch == 1) || (Core.BundleJS_Fetch == 2))
                            {
                                string RawBase64 = Child[I].GetProcessedRawBASE64("");

                                InsPos = TokenPos + ChildOffset[I];

                                if (Core.BundleJS_Fetch == 1)
                                {
                                    TokenRem(InsPos);
                                    TokenIns(InsPos, "\"" + RawBase64 + "\"");
                                }

                                if (Core.BundleJS_Fetch == 2)
                                {
                                    TokenRem(TokenPos);
                                    TokenPos++;
                                    while (TokenV[TokenPos] != ")")
                                    {
                                        TokenRem(TokenPos);
                                    }

                                    RawBase64 = RawBase64.Substring(RawBase64.IndexOf(";base64,") + 8);

                                    InsPos = TokenPos;

                                    TokenIns(InsPos, "new"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, "Promise"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, FetchPar1); InsPos++;
                                    TokenIns(InsPos, ","); InsPos++;
                                    TokenIns(InsPos, FetchPar2); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, "="); InsPos++;
                                    TokenIns(InsPos, ">"); InsPos++;
                                    TokenIns(InsPos, "{"); InsPos++;

                                    TokenIns(InsPos, "let"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, FetchVar2); InsPos++;
                                    TokenIns(InsPos, "="); InsPos++;
                                    TokenIns(InsPos, "atob"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "\"" + RawBase64 + "\""); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ";"); InsPos++;
                                    TokenIns(InsPos, "const"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, FetchVar1); InsPos++;
                                    TokenIns(InsPos, "="); InsPos++;
                                    TokenIns(InsPos, "new"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, "Array"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, FetchVar2); InsPos++;
                                    TokenIns(InsPos, "."); InsPos++;
                                    TokenIns(InsPos, "length"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ";"); InsPos++;

                                    TokenIns(InsPos, "for"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "let"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, FetchVar3); InsPos++;
                                    TokenIns(InsPos, "="); InsPos++;
                                    TokenIns(InsPos, "0"); InsPos++;
                                    TokenIns(InsPos, ";"); InsPos++;
                                    TokenIns(InsPos, FetchVar3); InsPos++;
                                    TokenIns(InsPos, "<"); InsPos++;
                                    TokenIns(InsPos, FetchVar2); InsPos++;
                                    TokenIns(InsPos, "."); InsPos++;
                                    TokenIns(InsPos, "length"); InsPos++;
                                    TokenIns(InsPos, ";"); InsPos++;
                                    TokenIns(InsPos, FetchVar3); InsPos++;
                                    TokenIns(InsPos, "+"); InsPos++;
                                    TokenIns(InsPos, "+"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, "{"); InsPos++;
                                    TokenIns(InsPos, FetchVar1); InsPos++;
                                    TokenIns(InsPos, "["); InsPos++;
                                    TokenIns(InsPos, FetchVar3); InsPos++;
                                    TokenIns(InsPos, "]"); InsPos++;
                                    TokenIns(InsPos, "="); InsPos++;
                                    TokenIns(InsPos, FetchVar2); InsPos++;
                                    TokenIns(InsPos, "."); InsPos++;
                                    TokenIns(InsPos, "charCodeAt"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, FetchVar3); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ";"); InsPos++;
                                    TokenIns(InsPos, "}"); InsPos++;

                                    TokenIns(InsPos, FetchPar1); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "new"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, "Response"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "new"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, "Blob"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, "["); InsPos++;
                                    TokenIns(InsPos, "new"); InsPos++;
                                    TokenIns(InsPos, " "); InsPos++;
                                    TokenIns(InsPos, "Uint8Array"); InsPos++;
                                    TokenIns(InsPos, "("); InsPos++;
                                    TokenIns(InsPos, FetchVar1); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, "]"); InsPos++;
                                    TokenIns(InsPos, ","); InsPos++;
                                    TokenIns(InsPos, "{"); InsPos++;
                                    TokenIns(InsPos, "type"); InsPos++;
                                    TokenIns(InsPos, ":"); InsPos++;
                                    TokenIns(InsPos, "\"" + Child[I].FileMimeType + "\""); InsPos++;
                                    TokenIns(InsPos, "}"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                    TokenIns(InsPos, ";"); InsPos++;

                                    TokenIns(InsPos, "}"); InsPos++;
                                    TokenIns(InsPos, ")"); InsPos++;
                                }
                                Child[I].IsBundled = true;
                            }
                        }
                        break;
                }
            }
        }


        public override void Parse(int MaxDepth)
        {
            base.Parse(MaxDepth);
            if (MaxDepth == 0)
            {
                Console.WriteLine("The depth limit is reached, the " + FileName + " will not be analyzed.");
                return;
            }

            char C = ' ';
            string TokenBuf = "";
            TokenTypeDef TokenBuf_ = TokenTypeDef.Blank;
            TokenTypeDef TokenBuf_0 = TokenTypeDef.Blank;

            int RegExStateClass = 0;
            int RegExStateCtrl = 0;
            int RegExStateQuote = 0;
            int RegExStateComment = 0;
            string TokenLast = "          ";

            for (int i = 0; i < Raw.Length; i++)
            {
                C = Raw[i];
                char C_ = ' ';
                if ((i + 1) < (Raw.Length))
                {
                    C_ = Raw[i + 1];
                }

                switch (TokenBuf_)
                {
                    case TokenTypeDef.Blank:
                        if (ScriptCharsDigit.Contains(C)) { TokenBuf_ = TokenTypeDef.ValueNum; }
                        if (ScriptCharsSpace.Contains(C)) { TokenBuf_ = TokenTypeDef.Whitespace; }
                        if (ScriptCharsOperatorRegEx.Contains(C) || ScriptCharsOperatorOther.Contains(C)) { TokenBuf_ = TokenTypeDef.Operator; }
                        if (C == '\'') { TokenBuf_ = TokenTypeDef.ValueQ1; }
                        if (C == '\"') { TokenBuf_ = TokenTypeDef.ValueQ2; }
                        if (C == '`') { TokenBuf_ = TokenTypeDef.ValueQ3; }
                        if (C == '/')
                        {
                            int IsRegExI = TokenT.Count - 1;
                            while ((IsRegExI > 0) && (TokenT[IsRegExI] == TokenTypeDef.Whitespace))
                            {
                                IsRegExI--;
                            }
                            if ((IsRegExI >= 0) && (TokenT[IsRegExI] == TokenTypeDef.Operator) && (ScriptCharsOperatorRegEx.Contains(TokenV[IsRegExI])))
                            {
                                RegExStateClass = 0;
                                RegExStateCtrl = 0;
                                RegExStateQuote = 0;
                                RegExStateComment = 0;
                                TokenBuf_ = TokenTypeDef.ValueRegExEnter;
                            }
                            else
                            {
                                TokenBuf_ = TokenTypeDef.CommentEnter;
                            }
                        }
                        if (TokenBuf_ == TokenTypeDef.Blank)
                        {
                            TokenBuf_ = TokenTypeDef.Identifier;
                        }
                        break;
                    case TokenTypeDef.Operator:
                        if ((!ScriptCharsOperatorRegEx.Contains(C)) && (!ScriptCharsOperatorOther.Contains(C))) { TokenBuf_ = TokenTypeDef.Blank; }
                        if (C == '/') { TokenBuf_ = TokenTypeDef.ValueRegEx; }
                        break;
                    case TokenTypeDef.ValueNum:
                        if ((!ScriptCharsDigit.Contains(C)) && (!ScriptCharsLetter.Contains(C)) && (C != '+') && (C != '-'))
                        {
                            TokenBuf_ = TokenTypeDef.Blank;
                        }
                        break;
                    case TokenTypeDef.ValueQ1:
                        if (C == '\\') { TokenBuf_ = TokenTypeDef.ValueQ1Esc; }
                        if (C == '\'') { TokenBuf_ = TokenTypeDef.ValueQ1Exit; }
                        break;
                    case TokenTypeDef.ValueQ2:
                        if (C == '\\') { TokenBuf_ = TokenTypeDef.ValueQ2Esc; }
                        if (C == '\"') { TokenBuf_ = TokenTypeDef.ValueQ2Exit; }
                        break;
                    case TokenTypeDef.ValueQ3:
                        if (C == '\\') { TokenBuf_ = TokenTypeDef.ValueQ3Esc; }
                        if (C == '`') { TokenBuf_ = TokenTypeDef.ValueQ3Exit; }
                        break;
                    case TokenTypeDef.ValueQ1Exit:
                    case TokenTypeDef.ValueQ2Exit:
                    case TokenTypeDef.ValueQ3Exit:
                        TokenBuf_ = TokenTypeDef.Blank;
                        break;
                    case TokenTypeDef.ValueQ1Esc:
                        TokenBuf_ = TokenTypeDef.ValueQ1;
                        break;
                    case TokenTypeDef.ValueQ2Esc:
                        TokenBuf_ = TokenTypeDef.ValueQ2;
                        break;
                    case TokenTypeDef.ValueQ3Esc:
                        TokenBuf_ = TokenTypeDef.ValueQ3;
                        break;
                    case TokenTypeDef.ValueRegExEnter:
                        if (C == '/')
                        {
                            TokenBuf_ = TokenTypeDef.Comment1;
                        }
                        if (C == '*')
                        {
                            TokenBuf_ = TokenTypeDef.Comment2;
                        }
                        if ((C != '/') && (C != '*'))
                        {
                            TokenBuf_ = TokenTypeDef.ValueRegEx;
                        }
                        break;
                    case TokenTypeDef.ValueRegEx:
                        if (RegExStateCtrl > 0)
                        {
                            RegExStateCtrl = 0;
                        }
                        else
                        {
                            switch (C)
                            {
                                case '\\':
                                    TokenBuf_ = TokenTypeDef.ValueRegExEsc;
                                    break;
                                case '/':
                                    if ((RegExStateClass == 0) && (RegExStateQuote == 0) && (RegExStateComment == 0))
                                    {
                                        TokenBuf_ = TokenTypeDef.ValueRegExExit;
                                    }
                                    break;
                                case '[':
                                    RegExStateClass++;
                                    break;
                                case ']':
                                    RegExStateClass--;
                                    break;
                                case '#':
                                    if (TokenLast.EndsWith("(?"))
                                    {
                                        RegExStateComment = 1;
                                    }
                                    break;
                                case ')':
                                    RegExStateComment = 0;
                                    break;
                            }
                        }
                        break;
                    case TokenTypeDef.ValueRegExEsc:
                        switch (C)
                        {
                            case 'Q':
                                RegExStateQuote++;
                                break;
                            case 'E':
                                RegExStateQuote--;
                                break;
                            case 'c':
                                RegExStateCtrl = 1;
                                break;
                        }
                        TokenBuf_ = TokenTypeDef.ValueRegEx;
                        break;
                    case TokenTypeDef.ValueRegExExit:
                        if (!ScriptCharsLetter.Contains(C))
                        {
                            TokenBuf_ = TokenTypeDef.Blank;
                        }
                        break;

                    case TokenTypeDef.CommentEnter:
                        if (C == '/')
                        {
                            TokenBuf_ = TokenTypeDef.Comment1;
                        }
                        if (C == '*')
                        {
                            TokenBuf_ = TokenTypeDef.Comment2;
                        }
                        if ((C != '/') && (C != '*'))
                        {
                            TokenBuf_ = TokenTypeDef.Blank;
                        }
                        break;
                    case TokenTypeDef.CommentExit:
                        if (C == '/')
                        {
                            TokenBuf_ = TokenTypeDef.CommentExit_;
                        }
                        else
                        {
                            if (C != '*')
                            {
                                TokenBuf_ = TokenTypeDef.Comment2;
                            }
                        }
                        break;
                    case TokenTypeDef.CommentExit_:
                        TokenBuf_ = TokenTypeDef.Blank;
                        break;
                    case TokenTypeDef.Comment1:
                        if ((C == '\r') || (C == '\n'))
                        {
                            TokenBuf_ = TokenTypeDef.Blank;
                        }
                        break;
                    case TokenTypeDef.Comment2:
                        if (C == '*')
                        {
                            TokenBuf_ = TokenTypeDef.CommentExit;
                        }
                        break;
                    case TokenTypeDef.Identifier:

                        if (ScriptCharsSpace.Contains(C)) { TokenBuf_ = TokenTypeDef.Blank; }
                        if ((ScriptCharsOperatorRegEx.Contains(C)) || (ScriptCharsOperatorOther.Contains(C))) { TokenBuf_ = TokenTypeDef.Blank; }
                        if (C == '\'') { TokenBuf_ = TokenTypeDef.Blank; }
                        if (C == '\"') { TokenBuf_ = TokenTypeDef.Blank; }
                        if (C == '`') { TokenBuf_ = TokenTypeDef.Blank; }

                        break;
                    case TokenTypeDef.Whitespace:
                        if (!ScriptCharsSpace.Contains(C))
                        {
                            TokenBuf_ = TokenTypeDef.Blank;
                        }
                        break;
                }

                if (TokenBuf_0 != TokenBuf_)
                {
                    if (TokenBuf_0 != TokenTypeDef.Blank)
                    {
                        if (CorrectTokenTypes)
                        {
                            switch (TokenBuf_0)
                            {
                                case TokenTypeDef.ValueRegExEnter:
                                case TokenTypeDef.CommentEnter:
                                    if ((TokenBuf_ != TokenTypeDef.Comment1) && (TokenBuf_ != TokenTypeDef.Comment2))
                                    {
                                        TokenBuf_0 = TokenTypeDef.Operator;
                                    }
                                    else
                                    {
                                        if (TokenBuf_ == TokenTypeDef.Comment1) TokenBuf_0 = TokenTypeDef.Comment1;
                                        if (TokenBuf_ == TokenTypeDef.Comment2) TokenBuf_0 = TokenTypeDef.Comment2;
                                    }
                                    break;
                                case TokenTypeDef.CommentExit:
                                case TokenTypeDef.CommentExit_:
                                    TokenBuf_0 = TokenTypeDef.Comment2;
                                    TokenBuf_0 = TokenTypeDef.Comment2;
                                    break;
                                case TokenTypeDef.ValueQ1Esc:
                                case TokenTypeDef.ValueQ1Exit:
                                    TokenBuf_0 = TokenTypeDef.ValueQ1;
                                    break;
                                case TokenTypeDef.ValueQ2Esc:
                                case TokenTypeDef.ValueQ2Exit:
                                    TokenBuf_0 = TokenTypeDef.ValueQ2;
                                    break;
                                case TokenTypeDef.ValueQ3Esc:
                                case TokenTypeDef.ValueQ3Exit:
                                    TokenBuf_0 = TokenTypeDef.ValueQ3;
                                    break;
                                case TokenTypeDef.ValueRegExEsc:
                                case TokenTypeDef.ValueRegExExit:
                                    TokenBuf_0 = TokenTypeDef.ValueRegEx;
                                    break;
                            }
                        }
                        TokenAdd(TokenBuf, TokenBuf_0);
                    }
                    TokenBuf = "";
                    TokenBuf_0 = TokenBuf_;
                }
                
                if (TokenBuf_ == TokenTypeDef.Blank)
                {
                    i--;
                }
                TokenBuf = TokenBuf + Raw[i];
                TokenLast = TokenLast.Substring(1) + Raw[i];
            }
            TokenAdd(TokenBuf, TokenBuf_);

            StringBuilder Sb = new StringBuilder();
            for (int I = 0; I < TokenV.Count; I++)
            {
                Sb.Append(TokenV[I]);
            }

            if (Raw.Length < Sb.Length)
            {
                throw new Exception("Token length error: " + Sb.ToString());
            }
            if (Raw.Substring(0, Sb.Length) != Sb.ToString())
            {
                throw new Exception("Token error: " + Sb.ToString());
            }

            if (PrintTokenList)
            {
                Console.WriteLine("Token list - " + FileName + " - START");
                for (int I = 0; I < TokenV.Count; I++)
                {
                    Console.WriteLine(I + "____" + TokenV[I] + "____" + TokenT[I]);
                }
                Console.WriteLine("Token list - " + FileName + " - STOP");
            }

            TokenWorkV.Clear();
            TokenWorkT.Clear();
            TokenWorkN.Clear();

            // Add blank at the begin of file
            TokenWorkV.Add("");
            TokenWorkT.Add(TokenTypeDef.Blank);
            TokenWorkN.Add(-1);

            // Create token list for working - without whitespaces and comments
            for (int I = 0; I < TokenV.Count; I++)
            {
                if ((TokenT[I] != TokenTypeDef.Whitespace) && (TokenT[I] != TokenTypeDef.Comment1) && (TokenT[I] != TokenTypeDef.Comment2))
                {
                    TokenWorkV.Add(TokenV[I]);
                    TokenWorkT.Add(TokenT[I]);
                    TokenWorkN.Add(I);
                }
            }

            // Add blank at the end of file
            TokenWorkV.Add("");
            TokenWorkT.Add(TokenTypeDef.Blank);
            TokenWorkN.Add(-1);

            // Combine every identifier-dot-identifier occurence
            bool DotChanged = true;
            int DotChangedPass = 0;
            while (DotChanged)
            {
                DotChanged = false;
                for (int I = 1; I < (TokenWorkV.Count - 1); I++)
                {
                    if ((TokenWorkT[I] == TokenTypeDef.Operator) && (TokenWorkV[I] == "."))
                    {
                        if ((TokenWorkT[I - 1] == TokenTypeDef.Identifier) && (TokenWorkT[I + 1] == TokenTypeDef.Identifier))
                        {
                            TokenWorkV[I - 1] = TokenWorkV[I - 1] + TokenWorkV[I] + TokenWorkV[I + 1];

                            TokenWorkV.RemoveRange(I, 2);
                            TokenWorkT.RemoveRange(I, 2);
                            TokenWorkN.RemoveRange(I, 2);
                            DotChanged = true;
                            I--;
                        }
                    }
                }
            }

            for (int I = 0; I < TokenWorkV.Count; I++)
            {
                // Search for module imports
                if ((TokenWorkT[I] == TokenTypeDef.Identifier) && (TokenWorkV[I] == "import"))
                {
                    string FileNameFound = null;
                    int II = I + 1;
                    bool ImportPossible = true;
                    int I_ = 0;
                    if ((TokenWorkT[II] == TokenTypeDef.Operator) && (TokenWorkV[II] == "{"))
                    {
                        while (ImportPossible)
                        {
                            II++;
                            if ((TokenWorkT[II] == TokenTypeDef.Operator) && ((TokenWorkV[II] != "}") && (TokenWorkV[II] != ",")))
                            {
                                ImportPossible = false;
                            }
                            if ((TokenWorkT[II] != TokenTypeDef.Identifier) && ((TokenWorkT[II] != TokenTypeDef.Operator)))
                            {
                                ImportPossible = false;
                            }
                            if ((TokenWorkT[II] == TokenTypeDef.Operator) && ((TokenWorkV[II] == "}")))
                            {
                                ImportPossible = false;
                                II++;
                                if ((TokenWorkT[II] == TokenTypeDef.Identifier) && ((TokenWorkV[II] == "from")))
                                {
                                    II++;
                                    if ((TokenWorkT[II] == TokenTypeDef.ValueQ1) || (TokenWorkT[II] == TokenTypeDef.ValueQ2))
                                    {
                                        FileNameFound = TokenWorkV[II];
                                        I_ = II;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((TokenWorkT[II] == TokenTypeDef.Identifier))
                        {
                            II++;
                            if ((TokenWorkT[II] == TokenTypeDef.Identifier) && ((TokenWorkV[II] == "from")))
                            {
                                II++;
                                FileNameFound = TokenWorkV[II];
                                I_ = II;
                            }
                        }
                    }
                    if (FileNameFound != null)
                    {
                        FileNameFound = FileNameFound.Substring(1, FileNameFound.Length - 2);
                        if (CoreFile.ValidFileName(Core.BaseDirI, FileNameFound) >= 0)
                        {
                            AppObj AppObj_ = new AppObjJS("JS:import", Depth + 1, FileNameFound, CoreFile.GetFileS(Core.BaseDirI, FileNameFound), this);
                            Child.Add(AppObj_);
                            ChildPtr.Add(TokenWorkN[I]);
                            ChildOffset.Add(TokenWorkN[I_] - TokenWorkN[I]);
                            AppObj_.Parse(MaxDepth - 1);
                        }
                    }
                }

                // Search for Worker objects
                // Search for image CSS url
                // Search for file fetch url
                // Search for worker importScripts
                if ((TokenWorkT[I] == TokenTypeDef.Identifier) && ((TokenWorkV[I] == "Worker") || (TokenWorkV[I] == "SharedWorker") || (TokenWorkV[I] == "url") || (TokenWorkV[I] == "fetch") || (TokenWorkV[I] == "importScripts")))
                {
                    string TagName = TokenWorkV[I];
                    bool TagGood = true;
                    bool UrlByTokens = false;
                    if ("url".Equals(TagName))
                    {
                        UrlByTokens = true;
                    }
                    if ("Worker".Equals(TagName) || "SharedWorker".Equals(TagName))
                    {
                        TagGood = false;
                        if ((TokenWorkT[I - 1] == TokenTypeDef.Identifier) && (TokenWorkV[I - 1] == "new"))
                        {
                            TagGood = true;
                        }
                    }
                    if (TagGood && (TokenWorkT[I + 1] == TokenTypeDef.Operator) && (TokenWorkV[I + 1] == "("))
                    {
                        int TokenN = I + 2;
                        int ArgDepth = 1;
                        List<string> FileNameList = new List<string>();
                        List<int> FileNamePos = new List<int>();
                        string FileNameTok = "";
                        while (ArgDepth > 0)
                        {
                            bool Std = false;
                            if (TokenWorkT[TokenN] == TokenTypeDef.Operator)
                            {
                                Std = true;
                                switch (TokenWorkV[TokenN])
                                {
                                    case "(":
                                        ArgDepth++;
                                        Std = false;
                                        break;
                                    case ")":
                                        ArgDepth--;
                                        Std = false;
                                        break;
                                }
                            }
                            if ((TokenWorkT[TokenN] == TokenTypeDef.Identifier))
                            {
                                Std = true;
                            }

                            if ((TokenWorkT[TokenN] == TokenTypeDef.ValueQ1) || (TokenWorkT[TokenN] == TokenTypeDef.ValueQ2))
                            {
                                FileNameList.Add(ConfigFile.MultilineDecode(TokenWorkV[TokenN]));
                                FileNamePos.Add(TokenN);
                                UrlByTokens = false;
                            }

                            if (Std)
                            {
                                FileNameTok = FileNameTok + TokenWorkV[TokenN];
                            }

                            TokenN++;
                        }

                        if (UrlByTokens)
                        {
                            FileNameList.Add(FileNameTok);
                            FileNamePos.Add(TokenWorkN[I] - TokenWorkN[TokenN]);
                        }

                        for (int I0 = 0; I0 < FileNameList.Count; I0++)
                        {
                            AppObj AppObj_ = CreateAppObj("JS:" + TagName, Depth + 1, Core.BaseDirI, FileNameList[I0], this);
                            if (AppObj_ != null)
                            {
                                Child.Add(AppObj_);
                                ChildPtr.Add(TokenWorkN[I]);
                                if (FileNamePos[I0] >= 0)
                                {
                                    ChildOffset.Add(TokenWorkN[FileNamePos[I0]] - TokenWorkN[I]);
                                }
                                else
                                {
                                    ChildOffset.Add(FileNamePos[I0]);
                                }
                                AppObj_.Parse(MaxDepth - 1);
                            }
                        }
                    }
                }
            }
        }

        void TokenIns(int Pos, List<string> Token, List<TokenTypeDef> Token_)
        {
            if (Pos <= TokenV.Count)
            {
                TokenV.InsertRange(Pos, Token);
                TokenT.InsertRange(Pos, Token_);
                for (int I = 0; I < ChildPtr.Count; I++)
                {
                    if (ChildPtr[I] >= Pos)
                    {
                        ChildPtr[I] += Token.Count;
                    }
                    else
                    {
                        if ((ChildPtr[I] + ChildOffset[I]) >= Pos)
                        {
                            ChildOffset[I] += Token.Count;
                        }
                    }
                }
            }
            else
            {
                TokenV.AddRange(Token);
                TokenT.AddRange(Token_);
            }
        }

        void TokenIns(int Pos, string Token)
        {
            if ("".Equals(Token))
            {
                return;
            }
            if (Token[0] == ' ')
            {
                TokenIns(Pos, Token, TokenTypeDef.Whitespace);
            }
            else
            {
                if ((ScriptCharsOperatorRegEx.IndexOf(Token) >= 0) || (ScriptCharsOperatorOther.IndexOf(Token) >= 0))
                {
                    TokenIns(Pos, Token, TokenTypeDef.Operator);
                }
                else
                {
                    if (Token[0] == '\"')
                    {
                        TokenIns(Pos, Token, TokenTypeDef.ValueQ2);
                    }
                    else
                    {
                        TokenIns(Pos, Token, TokenTypeDef.Identifier);
                    }
                }
            }
        }

        void TokenIns(int Pos, string Token, TokenTypeDef Token_)
        {
            if (Pos <= TokenV.Count)
            {
                TokenV.Insert(Pos, Token);
                TokenT.Insert(Pos, Token_);
                for (int I = 0; I < ChildPtr.Count; I++)
                {
                    if (ChildPtr[I] >= Pos)
                    {
                        ChildPtr[I]++;
                    }
                    else
                    {
                        if ((ChildPtr[I] + ChildOffset[I]) >= Pos)
                        {
                            ChildOffset[I]++;
                        }
                    }
                }
            }
            else
            {
                TokenV.Add(Token);
                TokenT.Add(Token_);
            }
        }

        void TokenRem(int Pos)
        {
            TokenV.RemoveAt(Pos);
            TokenT.RemoveAt(Pos);
            for (int I = 0; I < ChildPtr.Count; I++)
            {
                if (ChildPtr[I] > Pos)
                {
                    ChildPtr[I]--;
                }
                else
                {
                    if ((ChildPtr[I] + ChildOffset[I]) > Pos)
                    {
                        ChildOffset[I]--;
                    }
                }
            }
        }

        void TokenAdd(string Token, TokenTypeDef Token_)
        {
            switch (Token_)
            {
                case TokenTypeDef.Operator:
                    for (int I = 0; I < Token.Length; I++)
                    {
                        TokenV.Add(Token[I].ToString());
                        TokenT.Add(Token_);
                    }
                    break;
                case TokenTypeDef.Comment1:
                case TokenTypeDef.Comment2:
                case TokenTypeDef.ValueQ1:
                case TokenTypeDef.ValueQ2:
                case TokenTypeDef.ValueQ3:
                case TokenTypeDef.ValueRegEx:
                case TokenTypeDef.Whitespace:
                    {
                        bool NewToken = true;
                        if (TokenV.Count > 0)
                        {
                            if (TokenT[TokenV.Count - 1] == Token_)
                            {
                                TokenV[TokenV.Count - 1] = TokenV[TokenV.Count - 1] + Token;
                                NewToken = false;
                            }
                        }
                        if (NewToken)
                        {
                            TokenV.Add(Token);
                            TokenT.Add(Token_);
                        }
                    }
                    break;
                default:
                    TokenV.Add(Token);
                    TokenT.Add(Token_);
                    break;
            }
        }

        public override string GetProcessedRaw()
        {
            List<string> TokenToOutV = new List<string>();
            List<TokenTypeDef> TokenToOutT = new List<TokenTypeDef>();
            
            // Create output token list, remove comments
            for (int I = 0; I < TokenV.Count; I++)
            {
                TokenTypeDef T = TokenT[I];
                string V = TokenV[I];
                if ((TokenT[I] == TokenTypeDef.Comment1) || (TokenT[I] == TokenTypeDef.Comment2))
                {
                    if (!Core.MinifyJS_Comment)
                    {
                        // Transform comment1 into comment2
                        if (Core.MinifyJS_Whitespace)
                        {
                            if ((T == TokenTypeDef.Comment1))
                            {
                                V = "/*" + (V.Substring(2).Trim()) + "*/";
                                T = TokenTypeDef.Comment2;
                            }
                        }

                        TokenToOutV.Add(V);
                        TokenToOutT.Add(T);
                    }
                }
                else
                {
                    if ((TokenToOutT.Count > 0) && (TokenToOutT[TokenToOutT.Count - 1] == TokenT[I]) && (TokenT[I] == TokenTypeDef.Whitespace))
                    {
                        TokenToOutV[TokenToOutV.Count - 1] = TokenToOutV[TokenToOutV.Count - 1] + TokenV[I];
                    }
                    else
                    {
                        // For cleaning whitespace, there is not need to distinguish value type
                        switch (T)
                        {
                            case TokenTypeDef.ValueQ1:
                            case TokenTypeDef.ValueQ2:
                            case TokenTypeDef.ValueRegEx:
                            case TokenTypeDef.Identifier:
                                T = TokenTypeDef.ValueNum;
                                break;
                        }

                        TokenToOutV.Add(V);
                        TokenToOutT.Add(T);
                    }
                }
            }

            // Remove whitespaces
            if (Core.MinifyJS_Whitespace)
            {
                bool UseEOL = true;

                // Remove leading whitespace
                if ((TokenToOutV.Count > 0) && (TokenToOutT[0] == TokenTypeDef.Whitespace))
                {
                    TokenToOutT.RemoveAt(0);
                    TokenToOutV.RemoveAt(0);
                }

                // Remove trailing whitespace
                if ((TokenToOutV.Count > 0) && (TokenToOutT[TokenToOutV.Count - 1] == TokenTypeDef.Whitespace))
                {
                    TokenToOutT.RemoveAt(TokenToOutT.Count - 1);
                    TokenToOutV.RemoveAt(TokenToOutV.Count - 1);
                }

                // Shrink or remove other whitespace
                string NewLineOp = "})";
                for (int I = 1; I < (TokenToOutV.Count - 1); I++)
                {
                    if (TokenToOutT[I] == TokenTypeDef.Whitespace)
                    {
                        if (UseEOL)
                        {
                            if (TokenToOutV[I].Contains('\r') || TokenToOutV[I].Contains('\n'))
                            {
                                TokenToOutV[I] = "\n";
                            }
                            else
                            {
                                TokenToOutV[I] = " ";
                            }
                        }
                        else
                        {
                            TokenToOutV[I] = " ";
                        }

                        // Whitespace between comment and something other is not necessary
                        if ((TokenToOutT[I - 1] == TokenTypeDef.Comment2) || (TokenToOutT[I + 1] == TokenTypeDef.Comment2))
                        {
                            TokenToOutV[I] = "";
                        }

                        // Whitespace between operator and value is not necessary
                        if ((TokenToOutT[I - 1] == TokenTypeDef.ValueNum) && (TokenToOutT[I + 1] == TokenTypeDef.Operator))
                        {
                            TokenToOutV[I] = "";
                        }
                        if ((TokenToOutT[I - 1] == TokenTypeDef.Operator) && (TokenToOutT[I + 1] == TokenTypeDef.ValueNum))
                        {
                            if ("\n".Equals(TokenToOutV[I]))
                            {
                                if (!NewLineOp.Contains(TokenToOutV[I - 1]))
                                {
                                    TokenToOutV[I] = "";
                                }
                            }
                            else
                            {
                                TokenToOutV[I] = "";
                            }
                        }

                        // Whitespace between operators is not necessary
                        if ((TokenToOutT[I - 1] == TokenTypeDef.Operator) && (TokenToOutT[I + 1] == TokenTypeDef.Operator))
                        {
                            // Exception: if the second operator is '-', the space may be needed
                            if (TokenToOutV[I + 1] != "-")
                            {
                                TokenToOutV[I] = "";
                            }
                        }
                    }
                }
            }


            // Create output text
            StringBuilder Sb = new StringBuilder();
            for (int I = 0; I < TokenToOutV.Count; I++)
            {
                Sb.Append(TokenToOutV[I]);
            }
            return CodePreparation.Prepare(FileName, Sb.ToString(), true);
        }
    }
}