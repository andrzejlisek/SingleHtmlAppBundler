# Purpose

The main purpose of the **SingleHtmlAppBundler** is bundle resources \(images, scripts etc\.\) into single HTML file\. You can also perform partially bundling, for example, the scripts and styles will be bundled, but images will not be bundled into HTML\.

The HTML bundling can be very useful in cases, such as:


* Preparing HTML/JS application for run from other computer\. Single application will be as single file insteat of bunch of files\. Some mobile browsers copies local HTML file into temporary directory before run, without required resources\.
* Convert saved page as **complete web page** from file and folder \(usually named as **SomePageTitle\_files**\) into single file\.
* Solve CORS problems while running saved page or HTML/JS application from file as **file:///directory/file**\. In some browsers, the **file://** protocol does not follow the same\-origin policy, even if the policy is theoretically relaxed using apropriate browser plug\-in\.
* Transcode the HTML file encoding\.

**SingleHtmlAppBundler** is not intended for bundle server\-side scripts and templates\. It is intended to process the final document and scripts, which can be transfered to browser\.

# File types

The file type must be distinguished by two aspects:


* Distinguish HTML, JS/CSS and other files while processing\. The JS and CSS files are treated as file of the same type\.
* Determine the MIME string, which is used for embedding file contents in HTML\.

In the application directory, there is the **mime\.txt** text file, which defines MIME string and file type for some file extensions\. Definition line consists of three elements, separated by tab or space:


* File extension without dot character\.
* MIME string or asterisk for default\.\.
* File type as following:
  * 0 \- Any data file\.
  * 1 \- HTML file\.
  * 2 \- JS or CSS file\.

The definition with the asterisk character, determines the MIME string and file type for every file with extension, which is not listed in the **mime\.txt**\. The blank lines of **mime\.txt** and lines consisting less that three elements, are ignored\. 

The HtmlAgilityPack defines the type for many popular extensions\. The type in **mime\.txt** overrides the default mime type for this extension\.\.

# Application running

There is the command\-line application\. This application requires at least two arguments:


* Input HTML/JS/CSS file\.
* Output directory\.

The further arguments are parameters from following:


* **BundleHtml\*** \- **0** or **1** \- The HTML tag, which will be included in bundling\. Details in the **Html bundle** chapter\.
* **BundleJS\*** \- **0** or **1** \- The JavaScript or CSS expression, which will be bundles\. Details in the **JavaScript/CSS bundle** chapter\.
* **Minify\*** \- **0** or **1** \- Remove comments and resuce whitespaces from file, without changing the application control flow\. Details in the **Minification** chapter\.
* **EncodingRead** \- integer number \- Encoding used for read all text files\.
* **EncodingWrite** \- integer number \- Encoding used for write all text files\.
* **XHtml** \- **0** or **1** \- Generate HTML file as XHTML\.
* **MaxDepth** \- integer number \- Maximum recurence depth within file tree building\.
* **MimeType** \- **0** or **1** \- Displays MIME type for provided extensions separated with comma\. Described in **MIME types** chapter\.

If you run the application without arguments, there will be displayed all possible parameters with default value and available encoding list\.

## Bundling steps

The **SingleHtmlAppBundler** performs the following steps:


* **Step 0** \- Print parameters with currently used values\.
* **Step 1** \- Analyze provided file and create the dependency tree\. All HTML and JS files will be analyzed recursively, limited by **MaxDepth** parameter\.\.
* **Step 2** \- Perform bundling starting from the tree leaves according parameters\.
* **Step 3** \- Save necessary files in the output directory\. There will be saved the root file and only the files, which is not bundled\. At this step, the file tree will be printed and saved files will be indicated by asterisk\.

The **SingleHtmlAppBundler** assumes, that all input HTML/JS files are syntactically correct and not analyzes the syntax of the JS and CSS\. The parameter **MaxDepth** avoids the application hangs if there are cycle references in files\.

## Execution examples

Copy files without bundling and minifying:

```
dotnet SingleHtmlAppBundler.dll InputDir/file.html OutputDir
```

Bundle only scripts and minify JS files:

```
dotnet SingleHtmlAppBundler.dll InputDir/file.html OutputDir BundleHtmlScript=1 MinifyJsComment=1 MinifyJsWhitespace=1
```

Bundle only scripts and workers, use **Prepare\.txt** preparation file:

```
dotnet SingleHtmlAppBundler.dll InputDir/file.html OutputDir BundleHtmlScript=1 BundleJsWorker=1 CodePreparationFile=Prepare.txt
```

## Text replace

The **SingleHtmlAppBundler** assumes that any file name is explicity written\. In some cases, wspecially in scripts, the file name can be defined as string constant\. This application does not analyze the script control flow\. You can perform text replacement in any files\. For example, this file will not be embedded\.

```
const fileName = "somefile.js"
Worker wrk = new Worker(FileName);
```

You can do replace from `Worker(FileName);` to `Worker("somefile.js");` and you will get this script, which is equivalend and the additional file can be bundled:

```
const fileName = "somefile.js"
Worker wrk = new Worker("somefile.js");
```

The replacements can be define in preparation text file, which defines the code preparation, so the replacement will be done an the text file load moment\. The file has bunch of the parameters beginning from **1**\.


* **ReplaceXFile** \- The file name\.
* **ReplaceXTextFrom** \- Text, which will be searched\.
* **ReplaceXTextTo** \- Text, to which there will be replaced\.
* **ReplaceXNumber** \- The number of replacement occurences as following:
  * **0** \- Replace all occurences\. The default value\.
  * **Positive** \- Replaces the first N occurences or insert text at the begin\.
  * **Negative** \- Replaces the last N occurences or append text at the end\.

Instead of **X**, you have to place the replacement number\. Assuming, that mentioned example is the **script\.js** file, you have to define the first replacement:

```
Replace1File=script.js
Replace1TextFrom=Worker(FileName);
Replace1TextTo=Worker("somefile.js");
```

In the **From** and **To** parts, you can place the text encoded as one\-line string\. You can use the escape characters if the text is inside double quotes\. The same replacement can be written as following:

```
Replace1File=script.js
Replace1TextFrom="Worker(FileName);"
Replace1TextTo="Worker(\"somefile.js\");"
```

If you want to add some additional text, the **ReplaceXTextFrom** must be blank and the **ReplaceXNumber** must be defined, for example:

```
Replace1File=index.html
Replace1TextFrom=""
Replace1TextTo="<!-- Additional header -->"
Replace1Number=1

Replace2File=index.html
Replace2TextFrom=""
Replace2TextTo="<!-- Additional footer -->"
Replace2Number=-1
```

When you want to define two replacements in one file, one replacement in another file, you can write following:

```
Replace1File=script.js
Replace1TextFrom=Worker(wrk1);
Replace1TextTo=Worker("somefile1.js");

Replace2File=script.js
Replace2TextFrom=Worker(wrk2);
Replace2TextTo=Worker("somefile2.js");

Replace3File=info.js
Replace3TextFrom=fetch(infofile);
Replace3TextTo=fetch("info.txt");
```

The number of defined replacements will be automatically detected\. This type can be used for search or replace text occupying several lines or several leading/trailing spaces\. The special characters can be uded:

| Character | Code in file |
| --- | --- |
| Backslack | \\\\ |
| Double quote | \\" |
| Single quote | \\' |
| Carriage return | \\r |
| Line feed | \\n |
| Tabulator | \\t |
| Vertical tab | \\v |

## Cycle references

Some HTML\+JavaScript projects can have cycle references between bunch of some files\. The file analysis depths is always limited by **MaxDepth** parameter\. In most such cases, the bundling is also possible under the following circumstances:


* Using the code preparation file, use the text replacements to break these cycles\.
* The bundling process may require several steps, each steps uses another code preparation file and other root file\.

## File type force

Generally, the file MIME type and data type \(binary, HTML, JS\) is detected by the file extension\. In some cases, the file extension can be incorrect, for example, the JavaScript file can have name **script\.js\.php** due to enforce to send additional HTTP headers defined inside the file, using PHP language, for example:

```
<?php
header("Cross-Origin-Embedder-Policy: require-corp");
header("Cross-Origin-Opener-Policy: same-origin");
header("Content-Type: text/javascript");
?>
```

The file will be treated as JavaScript file despite a different extension\. You can enforce desired type in preparation file using following triples:


* **FileXName** \- File name
* **FileXMime** \- File MIME text type
* **FileXType** \- File type for **SingleHtmlAppBundler** as following:
  * **0** \- Binary file\.
  * **1** \- HTML file\.
  * **2** \- JavaScript or CSS file\.

In place of **X**, you have to place the number starting of **1**\. There is example for enforcing type for three files:

```
File1Name=index.php
File1Mime=text/html
File1Type=1

File2Name=script.txt
File2Mime=text/javascript
File2Type=2

File3Name=media/picture.bin
File3Mime=image/jpeg
File3Type=0
```

# Html bundle

The HTML bundling is performed by analyzing the tags in the following table\. In the **Parameter** column there is parameter name, which affects bundling the tag\. If the tag is excluded by parameter, the file will be save into the output directory preserving the subdirectory\. If the tags does not relate to the other file or the file is unreadable, the tag will be ignored regardless the according parameter value\.

| Parameter | Tag | Attribute | Description |
| --- | --- | --- | --- |
| BundleHtmlBody | `body` | `background` | Page background image\. |
| BundleHtmlScript | `script` | `src` | Script file\. |
| BundleHtmlLink | `link` | `href` | Additional file \(style, icon\)\. If `rel`=`stylesheet`, the tag will be replace with `style` tag\. |
| BundleHtmlIframe | `iframe` | `src` | Media or document inside frame\. |
| BundleHtmlImg | `img` | `src` | Image file\. |
| BundleHtmlAudio | `audio` | `src` | Audio file\. |
| BundleHtmlVideo | `video` | `src` | Video file\. |
| BundleHtmlTrack | `track` | `src` | Additional file for `audio` or `video` tag, uses the `type` attribute to determine file type\. |
| BundleHtmlSource | `source` | `src` or `srcset` | Alternative file for `picture`, `audio`, `video` tag, uses the `type` attribute to determine file type\. |
| BundleHtmlObject | `object` | `data` | Media or document inside frame\. |
| BundleHtmlEmbed | `embed` | `src` | Media or document inside frame, uses the `type` attribute to determine file type\. |

## File type and BASE64

Bundling media or other document files requires to be converted to the textual file contents with the file MIME string\. Some tags defines the MIME string ad the string will be copied into the file text contents\. Otherwise, the file type will be determines based on the file extension\. It is recommended to have defined all used file extensions in the **mime\.txt** file\.

## HTML and XHTML

There are standards of writing HTML documents\. The first standard is HTML, the second is XHTML\. The **SingleHtmlAppBundler** can write HTML files with both standards, by default, uses the HTML standard\. Application does not autmatically make HTML fully compliant with one of the standard and does not recognize the standard\. You can choose the writing standard by using the parameter **XHtml**, which means:


* **0** \- Use HTML standard\.
* **1** \- Use XHTML standard\.

If you set the **XHtml**=**1**, there are only two differences in writing HTML file compared with **XHtml**=**0**:


* Every attribute without value will be written as attribute with the same value as attribute name, for example `attrib="attrib"`\.
* Singleton tags \(the tag, which can not be open and closed\) will be written as XML\-fashion self\-closing tag, for example `<br />` instead of `<br>`\.

# JavaScript/CSS bundle

The **SingleHtmlAppBundler** has simplified token analyzer used for bundling other files and for minifing\. The algorithm can be the same for both JS and CSS files\. The tokens are the following:


* **Whitespace** \- String consisting of the whitespace characters only\. The token can not be blank\.
* **Operator** \- Single character operator\. In this application, operators consisting of several characters, such as `&&` or `>=` can be treated as double independed operators\. In this approach, the bracket, comma, semicolon characters are also a operators\.
* **Identifier** \- Single word without quote, consisting of characters other than whitespace, operator, quote\. The keyword are not distinguished\.
* **ValueNum** \- Integer number\. The fraction number can be treated as sequence of numbers and operators\.
* **ValueQ1** \- String value within single quote characters\.
* **ValueQ2** \- String value within double quote characters\.
* **ValueQ3** \- String value within apostrophe characters\.
* **ValueRegExp** \- Regular expression string\.
* **Comment1** \- Comment from `//` to end of line\.
* **Comment2** \- Comment from `/*` to `*/`\.

After splitting into tokens, every two identifiers separated by `.` token are converted into single identifier token\.

Bundling JS/CSS involves the following elements:


* **Url** \- binary file in style, comonly image used as background\.
* **Worker** \- JavaScript Worker or SharedWorker object, which commonly relates to other JavaScript file\.
* **Fetch** \- JavaScript fetch data from other file\.

Application assumes, that the file name is written directly\. The cases, where the file name is provided via constant or variable are very rare and the file must be changed manually before bundling\.

JavaScript supports the modules, which the **SingleHtmlAppBundler** not supports\. The module bundling is more complex than **Url** and **Worker**, and there are several free applications, which is specialized in module bundling\. In this case, you have to primarly bundle the modules using appropriate application, then you can bundle whole HTML or JS using the **SingleHtmlAppBundler**\.

## Style Url

The **Url** can be bundled for every **Url** token occurence, which is followed by opening bracket and string value\. In the CSS style, the file name can be provided without quote characters, and the **SingleHtmlAppBundler** will concatenate all token to closing bracket\.

There are correct examples, which can be bundled:

```
Url("media/background.png")
Url('media/background.png')
Url(media/background.png)
```

## JavaScript Worker, SharedWorker, importScripts

The Worker object or SharedWorker object usually is constructed by providing other JavaScript file\. Bundler creates the fake JavaScript function containing the contents of provided file and creates the blob from the function\.

There can be bundled every **Worker** or **SharedWorker** token occurence, which is the preceded by **new** token and followef by bracket open and directly provided file name\. The **importScripts** token is always without the **new** token and can contain several URLs at once\.

In such cases, the **Worker** can be bundled:

```
x = new Worker("file1.js");
y = new Worker('file2.js');
```

In the examples, the **Worked** can not be bundled:

```
const jsfile = "file1.js";
x = new Worker(jsfile);
y = Worker("file2.js");
```

But, this example can be bundled, if you use the following replacement:

```
Replace1File=script.js
Replace1TextFrom="x = new Worker(jsfile);"
Replace1To="x = new Worker(\"file1.js\");"
```

The range of replacing text should be quite large for explicitly point single occurence, when you are using several workers\. The **SingleHtmlAppBundler** does not analyze the JavaScript semantics and the flow control, but creating the **Worker** object without the **new** token is not correct\.

## JavaScript Fetch

The **fetch** function is used for get additional files\. The **SingleHtmlAppBundler** searches for just **fetch** token and searches for the file name\. The work is similar to JavaScript Worker\.

## Alternative algorithms

The bundling way in the **SingleHtmlAppBundler** are good in most cases, but the **Worker**, **SharedWorker**, **importScripts** and **fetch** offers bundling using different algorithm, but the effect is the same\. There parameters for appropriate parameters can be as following:


* **0** \- Do not bundle\.
* **1** \- Use **data:mimetype;base64,data** URL form\.
* **2** \- Depends on parameter:
  * **BundleJsWorker**, **BundleJsSharedWorker**, **BundleJsImportScripts** \- use **Blob** object created from text\.
  * **BundleJsFetch** \- Replace the **fetch** function with anonymous function creating the **Response** object directly\.

For other parameters, if you try the value **2**, there will be bundled using standard \(only implemented\) algorithm\.

# Minification

The **SingleHtmlAppBundler** can minify the HTML and JS/CSS files\. The minification is very simple, the literals in the JavaScript files will not be shortened\. The minification can be controlled by the following parameters:


* **MinifyHtmlComment** \- Remove comments from HTML files\.
* **MinifyHtmlWhitespace** \- Reduce whitespace sequence to single space character in HTML files\.
* **MinifyJsComment** \- Remove comments from JavaScript and CSS files\.
* **MinifyJsWhitespace** \- Change every whitespace token to single space in JS and CSS files\. The token can be removed in the following circumstances:
  * Leading and trailing whilespace in whole script\.
  * Whitespace before comment or after comment if `MinifyJsComment`=`0`\.
  * Whitespace between operator and value\.
  * Whitespace between operators when the operator after whitespace is not the `-`, which can represent the negative number literal\.

By default, the minification will not be applied, but you can enable minification using parameters described above\.

# MIME types

You can print the MIME types for specified extension using definitions inside HtmlAgilityPack library and in the **mime\.txt** file\.

You have to specify file extensions or \* both in place of input file and output directory\. These followin examples gives exactly the same result:

```
dotnet SingleHtmlAppBundler.dll html,htm,jpg,gif,png css,js,* MimeType=1
dotnet SingleHtmlAppBundler.dll html,htm jpg,gif,png,css,js,* MimeType=1
dotnet SingleHtmlAppBundler.dll html,htm,jpg,gif,png,css,js * MimeType=1
```

This calls is incorrect:

```
dotnet SingleHtmlAppBundler.dll txt MimeType=1
dotnet SingleHtmlAppBundler.dll * MimeType=1
dotnet SingleHtmlAppBundler.dll txt,* MimeType=1
dotnet SingleHtmlAppBundler.dll txt,js MimeType=1
```

For check single extension, just add any additional extension\. The parameters being **param=value** must be from a third command argument\.




