// See https://aka.ms/new-console-template for more information

using System.Text;
using SingleHtmlAppBundler;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args.Length < 2)
{
    Console.WriteLine("Usage: SingleHtmlAppBundler <InputFile> <OutputDir> [Parameters]");
    Console.WriteLine("");
    Console.WriteLine("Available parameters:");
    Console.WriteLine("CodePreparationFile=");
    Console.WriteLine("BundleHtmlBody=0");
    Console.WriteLine("BundleHtmlScript=0");
    Console.WriteLine("BundleHtmlLink=0");
    Console.WriteLine("BundleHtmlIframe=0");
    Console.WriteLine("BundleHtmlImg=0");
    Console.WriteLine("BundleHtmlAudio=0");
    Console.WriteLine("BundleHtmlVideo=0");
    Console.WriteLine("BundleHtmlTrack=0");
    Console.WriteLine("BundleHtmlSource=0");
    Console.WriteLine("BundleHtmlObject=0");
    Console.WriteLine("BundleHtmlEmbed=0");
    Console.WriteLine("BundleJsUrl=0");
    Console.WriteLine("BundleJsWorker=0");
    Console.WriteLine("BundleJsSharedWorker=0");
    Console.WriteLine("BundleJsFetch=0");
    Console.WriteLine("BundleJsImportScripts=0");
    Console.WriteLine("MinifyHtmlComment=0");
    Console.WriteLine("MinifyHtmlWhitespace=0");
    Console.WriteLine("MinifyJsComment=0");
    Console.WriteLine("MinifyJsWhitespace=0");
    Console.WriteLine("EncodingRead=" + Encoding.UTF8.CodePage);
    Console.WriteLine("EncodingWrite=" + Encoding.UTF8.CodePage);
    Console.WriteLine("XHtml=0");
    Console.WriteLine("MaxDepth=10");
    Console.WriteLine("MimeType=0");
    Console.WriteLine();


    Console.WriteLine("Available encodings:");

    List<string> EncodingList = new List<string>();
    foreach (EncodingInfo ei in Encoding.GetEncodings())
    {
        Encoding e = ei.GetEncoding();
        string EncName = e.CodePage.ToString().PadLeft(5) + ": " + SingleHtmlAppBundler.EncodingWork.EncodingGetName(e) + "  ";
        EncodingList.Add(EncName);
    }
    EncodingList.Sort();
    for (int I = 0; I < EncodingList.Count; I++)
    {
        Console.WriteLine(EncodingList[I]);
    }


    return;
}


SingleHtmlAppBundler.Core Core_ = new SingleHtmlAppBundler.Core();

bool MimeTypeMode = false;

int ParseMaxDepth = 10;        

string FileI = args[0];
string FileO = args[1];
for (int I = 2; I < args.Length; I++)
{
    int ParamValuePos = args[I].IndexOf('=');
    if (ParamValuePos >= 0)
    {
        {
            bool ValB = false;
            int ValI = 0;
            switch (args[I].Substring(ParamValuePos + 1).ToLowerInvariant())
            {
                case "1":
                case "y":
                case "yes":
                case "t":
                case "true":
                    ValB = true;
                    ValI = 1;
                    break;
                case "0":
                case "n":
                case "no":
                case "f":
                case "false":
                    ValB = false;
                    ValI = 0;
                    break;
                default:
                    try
                    {
                        ValI = int.Parse(args[I].Substring(ParamValuePos + 1).ToLowerInvariant());
                        ValB = true;
                    }
                    catch
                    {
                        ValI = 0;
                        ValB = false;
                    }
                    break;
            }
            string ValS = args[I].Substring(ParamValuePos + 1);
            switch (args[I].Substring(0, ParamValuePos))
            {
                case "CodePreparationFile":
                    SingleHtmlAppBundler.Core.CodePreparationFile = ValS;
                    SingleHtmlAppBundler.CodePreparation.Init(ValS);
                    break;
                case "BundleHtmlBody":
                    SingleHtmlAppBundler.Core.BundleHTML_Body = ValB;
                    break;
                case "BundleHtmlScript":
                    SingleHtmlAppBundler.Core.BundleHTML_Script = ValB;
                    break;
                case "BundleHtmlLink":
                    SingleHtmlAppBundler.Core.BundleHTML_Link = ValB;
                    break;
                case "BundleHtmlIframe":
                    SingleHtmlAppBundler.Core.BundleHTML_Iframe = ValB;
                    break;
                case "BundleHtmlImg":
                    SingleHtmlAppBundler.Core.BundleHTML_Img = ValB;
                    break;
                case "BundleHtmlAudio":
                    SingleHtmlAppBundler.Core.BundleHTML_Audio = ValB;
                    break;
                case "BundleHtmlVideo":
                    SingleHtmlAppBundler.Core.BundleHTML_Video = ValB;
                    break;
                case "BundleHtmlTrack":
                    SingleHtmlAppBundler.Core.BundleHTML_Track = ValB;
                    break;
                case "BundleHtmlSource":
                    SingleHtmlAppBundler.Core.BundleHTML_Source = ValB;
                    break;
                case "BundleHtmlObject":
                    SingleHtmlAppBundler.Core.BundleHTML_Object = ValB;
                    break;
                case "BundleHtmlEmbed":
                    SingleHtmlAppBundler.Core.BundleHTML_Embed = ValB;
                    break;
                case "BundleJsUrl":
                    SingleHtmlAppBundler.Core.BundleJS_Url = ValB;
                    break;
                case "BundleJsWorker":
                    SingleHtmlAppBundler.Core.BundleJS_Worker = ValI;
                    break;
                case "BundleJsSharedWorker":
                    SingleHtmlAppBundler.Core.BundleJS_SharedWorker = ValI;
                    break;
                case "BundleJsFetch":
                    SingleHtmlAppBundler.Core.BundleJS_Fetch = ValI;
                    break;
                case "BundleJsImportScripts":
                    SingleHtmlAppBundler.Core.BundleJS_ImportScripts = ValI;
                    break;
                case "MinifyHtmlComment":
                    SingleHtmlAppBundler.Core.MinifyHTML_Comment = ValB;
                    break;
                case "MinifyHtmlWhitespace":
                    SingleHtmlAppBundler.Core.MinifyHTML_Whitespace = ValB;
                    break;
                case "MinifyJsComment":
                    SingleHtmlAppBundler.Core.MinifyJS_Comment = ValB;
                    break;
                case "MinifyJsWhitespace":
                    SingleHtmlAppBundler.Core.MinifyJS_Whitespace = ValB;
                    break;
                case "XHtml":
                    SingleHtmlAppBundler.Core.UseXHTML = ValB;
                    break;
                case "EncodingRead":
                    SingleHtmlAppBundler.Core.WorkEncodingI = SingleHtmlAppBundler.EncodingWork.EncodingFromName(ValS);
                    break;
                case "EncodingWrite":
                    SingleHtmlAppBundler.Core.WorkEncodingO = SingleHtmlAppBundler.EncodingWork.EncodingFromName(ValS);
                    break;
                case "MaxDepth":
                    ParseMaxDepth = ValI;
                    break;
                case "MimeType":
                    MimeTypeMode = ValB;
                    break;
            }
        }
    }
}

Console.WriteLine("Input file: " + FileI);
Console.WriteLine("Output directory: " + FileO);
Console.WriteLine("CodePreparationFile=" + SingleHtmlAppBundler.Core.CodePreparationFile);
Console.WriteLine("BundleHtmlBody=" + (SingleHtmlAppBundler.Core.BundleHTML_Body ? "1" : "0"));
Console.WriteLine("BundleHtmlScript=" + (SingleHtmlAppBundler.Core.BundleHTML_Script ? "1" : "0"));
Console.WriteLine("BundleHtmlLink=" + (SingleHtmlAppBundler.Core.BundleHTML_Link ? "1" : "0"));
Console.WriteLine("BundleHtmlIframe=" + (SingleHtmlAppBundler.Core.BundleHTML_Iframe ? "1" : "0"));
Console.WriteLine("BundleHtmlImg=" + (SingleHtmlAppBundler.Core.BundleHTML_Img ? "1" : "0"));
Console.WriteLine("BundleHtmlAudio=" + (SingleHtmlAppBundler.Core.BundleHTML_Audio ? "1" : "0"));
Console.WriteLine("BundleHtmlVideo=" + (SingleHtmlAppBundler.Core.BundleHTML_Video ? "1" : "0"));
Console.WriteLine("BundleHtmlTrack=" + (SingleHtmlAppBundler.Core.BundleHTML_Track ? "1" : "0"));
Console.WriteLine("BundleHtmlSource=" + (SingleHtmlAppBundler.Core.BundleHTML_Source ? "1" : "0"));
Console.WriteLine("BundleHtmlObject=" + (SingleHtmlAppBundler.Core.BundleHTML_Object ? "1" : "0"));
Console.WriteLine("BundleHtmlEmbed=" + (SingleHtmlAppBundler.Core.BundleHTML_Embed ? "1" : "0"));
Console.WriteLine("BundleJsUrl=" + (SingleHtmlAppBundler.Core.BundleJS_Url ? "1" : "0"));
Console.WriteLine("BundleJsWorker=" + SingleHtmlAppBundler.Core.BundleJS_Worker);
Console.WriteLine("BundleJsSharedWorker=" + SingleHtmlAppBundler.Core.BundleJS_SharedWorker);
Console.WriteLine("BundleJsFetch=" + SingleHtmlAppBundler.Core.BundleJS_Fetch);
Console.WriteLine("BundleJsImportScripts=" + SingleHtmlAppBundler.Core.BundleJS_ImportScripts);
Console.WriteLine("MinifyHtmlComment=" + (SingleHtmlAppBundler.Core.MinifyHTML_Comment ? "1" : "0"));
Console.WriteLine("MinifyHtmlWhitespace=" + (SingleHtmlAppBundler.Core.MinifyHTML_Whitespace ? "1" : "0"));
Console.WriteLine("MinifyJsComment=" + (SingleHtmlAppBundler.Core.MinifyJS_Comment ? "1" : "0"));
Console.WriteLine("MinifyJsWhitespace=" + (SingleHtmlAppBundler.Core.MinifyJS_Whitespace ? "1" : "0"));
Console.WriteLine("EncodingRead=" + SingleHtmlAppBundler.Core.WorkEncodingI.CodePage);
Console.WriteLine("EncodingWrite=" + SingleHtmlAppBundler.Core.WorkEncodingO.CodePage);
Console.WriteLine("XHtml=" + (SingleHtmlAppBundler.Core.UseXHTML ? "1" : "0"));
Console.WriteLine("MaxDepth=" + ParseMaxDepth);
Console.WriteLine("MimeType=" + (MimeTypeMode ? "1" : "0"));
Console.WriteLine();


if (MimeTypeMode)
{
    Core_.WriteMimes(FileI, FileO);
}
else
{
    Core_.Start(FileI, FileO, ParseMaxDepth);
}