// See https://aka.ms/new-console-template for more information

using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args.Length < 2)
{
    Console.WriteLine("Usage: SingleHtmlAppBundler <InputFile> <OutputDir> [Options]");
    Console.WriteLine("");
    Console.WriteLine("Available parameters:");
    Console.WriteLine("BundleHtmlBody=1");
    Console.WriteLine("BundleHtmlScript=1");
    Console.WriteLine("BundleHtmlLink=1");
    Console.WriteLine("BundleHtmlImg=1");
    Console.WriteLine("BundleHtmlAudio=1");
    Console.WriteLine("BundleHtmlVideo=1");
    Console.WriteLine("BundleHtmlSource=1");
    Console.WriteLine("BundleHtmlTrack=1");
    Console.WriteLine("BundleHtmlIframe=1");
    Console.WriteLine("BundleHtmlEmbed=1");
    Console.WriteLine("BundleHtmlObject=1");
    Console.WriteLine("BundleJsUrl=1");
    Console.WriteLine("BundleJsWorker=1");
    Console.WriteLine("MinifyHtmlComment=0");
    Console.WriteLine("MinifyHtmlWhitespace=0");
    Console.WriteLine("MinifyJsComment=0");
    Console.WriteLine("MinifyJsWhitespace=0");
    Console.WriteLine("EncodingRead=" + Encoding.UTF8.CodePage);
    Console.WriteLine("EncodingWrite=" + Encoding.UTF8.CodePage);
    Console.WriteLine("XHtml=0");
    Console.WriteLine();


    Console.WriteLine("File types:");
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

string FileI = args[0];
string FileO = args[1];
for (int I = 2; I < args.Length; I++)
{
    int ParamValuePos = args[I].IndexOf('=');
    if (ParamValuePos >= 0)
    {
        try
        {
            bool ValB = false;
            switch (args[I].Substring(ParamValuePos + 1).ToLowerInvariant())
            {
                case "1":
                case "y":
                case "yes":
                case "t":
                case "true":
                    ValB = true;
                    break;
            }
            string ValS = args[I].Substring(ParamValuePos + 1);
            switch (args[I].Substring(0, ParamValuePos))
            {
                case "XHtml":
                    SingleHtmlAppBundler.Core.UseXHTML = ValB;
                    break;
                case "BundleHtmlBody":
                    SingleHtmlAppBundler.Core.BundleHTML_Body = ValB;
                    break;
                case "BundleHtmlImg":
                    SingleHtmlAppBundler.Core.BundleHTML_Img = ValB;
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
                case "BundleHtmlEmbed":
                    SingleHtmlAppBundler.Core.BundleHTML_Embed = ValB;
                    break;
                case "BundleHtmlSource":
                    SingleHtmlAppBundler.Core.BundleHTML_Source = ValB;
                    break;
                case "BundleHtmlObject":
                    SingleHtmlAppBundler.Core.BundleHTML_Object = ValB;
                    break;
                case "BundleHtmlAudio":
                    SingleHtmlAppBundler.Core.BundleHTML_Audio = ValB;
                    break;
                case "BundleHtmlVideo":
                    SingleHtmlAppBundler.Core.BundleHTML_Video = ValB;
                    break;
                case "BundleJsUrl":
                    SingleHtmlAppBundler.Core.BundleJS_Url = ValB;
                    break;
                case "BundleJsWorker":
                    SingleHtmlAppBundler.Core.BundleJS_Worker = ValB;
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
                case "EncodingRead":
                    SingleHtmlAppBundler.Core.WorkEncodingI = SingleHtmlAppBundler.EncodingWork.EncodingFromName(ValS);
                    break;
                case "EncodingWrite":
                    SingleHtmlAppBundler.Core.WorkEncodingO = SingleHtmlAppBundler.EncodingWork.EncodingFromName(ValS);
                    break;
            }
        }
        catch
        {
              
        }
    }
}

Console.WriteLine("Input file: " + FileI);
Console.WriteLine("Output directory: " + FileO);
Console.WriteLine("BundleHtmlBody=" + SingleHtmlAppBundler.Core.BundleHTML_Body);
Console.WriteLine("BundleHtmlScript=" + SingleHtmlAppBundler.Core.BundleHTML_Script);
Console.WriteLine("BundleHtmlLink=" + SingleHtmlAppBundler.Core.BundleHTML_Link);
Console.WriteLine("BundleHtmlImg=" + SingleHtmlAppBundler.Core.BundleHTML_Img);
Console.WriteLine("BundleHtmlAudio=" + SingleHtmlAppBundler.Core.BundleHTML_Audio);
Console.WriteLine("BundleHtmlVideo=" + SingleHtmlAppBundler.Core.BundleHTML_Video);
Console.WriteLine("BundleHtmlSource=" + SingleHtmlAppBundler.Core.BundleHTML_Source);
Console.WriteLine("BundleHtmlTrack=" + SingleHtmlAppBundler.Core.BundleHTML_Track);
Console.WriteLine("BundleHtmlIframe=" + SingleHtmlAppBundler.Core.BundleHTML_Iframe);
Console.WriteLine("BundleHtmlEmbed=" + SingleHtmlAppBundler.Core.BundleHTML_Embed);
Console.WriteLine("BundleHtmlObject=" + SingleHtmlAppBundler.Core.BundleHTML_Object);
Console.WriteLine("BundleJsUrl=" + SingleHtmlAppBundler.Core.BundleJS_Url);
Console.WriteLine("BundleJsWorker=" + SingleHtmlAppBundler.Core.BundleJS_Worker);
Console.WriteLine("MinifyHtmlComment=" + SingleHtmlAppBundler.Core.MinifyHTML_Comment);
Console.WriteLine("MinifyHtmlWhitespace=" + SingleHtmlAppBundler.Core.MinifyHTML_Whitespace);
Console.WriteLine("MinifyJsComment=" + SingleHtmlAppBundler.Core.MinifyJS_Comment);
Console.WriteLine("MinifyJsWhitespace=" + SingleHtmlAppBundler.Core.MinifyJS_Whitespace);
Console.WriteLine("EncodingRead=" + SingleHtmlAppBundler.Core.WorkEncodingI.CodePage);
Console.WriteLine("EncodingWrite=" + SingleHtmlAppBundler.Core.WorkEncodingO.CodePage);
Console.WriteLine("XHtml=" + SingleHtmlAppBundler.Core.UseXHTML);
Console.WriteLine();


Core_.Start(FileI, FileO);