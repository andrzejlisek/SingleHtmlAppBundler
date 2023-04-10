# Purpose

The main purpose of the **SingleHtmlAppBundler** is bundle resources \(images, scripts etc\.\) into single HTML file\. You can also perform partially bundling, for example, the scripts and styles will be bundled, but images will not be bundled into HTML\.

The HTML bundling can be very useful in cases, such as:


* Preparing HTML/JS application for run from other computer\. Single application will be as single file insteat of bunch of files\. Some mobile browsers copies local HTML file into temporary directory before run, without required resources\.
* Convert saved page as **complete web page** from file and folder \(usually named as **SomePageTitle\_files**\) into single file\.
* Solve CORS problems while running saved page or HTML/JS application from file as **file:///directory/file**\. In some browsers, the **file://** protocol does not follow the same\-origin policy, even if the policy is theoretically relaxed using apropriate browser plug\-in\.
* Transcode the HTML file encoding\.

**SingleHtmlAppBundler** is not intended for bundle server\-side scripts and templates\. It is intended to process the final document and scripts, which can be transfered to browser\.

# File types

The file type must be distinguished th two aspects:


* Distinguish HTML, JS/CSS and other files while processing\. The JS and CSS files are treated as file of the same type\.
* Determine the MIME string, which is used for embedding file contents in HTML\.

In the application directory, there is the **mime\.txt** text file, which defines MIME string and file type for some file extensions\. Definition line consists of three elements, separated by tab or space:


* File extension without dot character\.
* MIME string\.
* File type as following:
  * 0 \- Binary file\.
  * 1 \- HTML file\.
  * 2 \- JS or CSS file\.

The definition with the asterisk character, determines the MIME string and file type for every file with extension, which is not listed in the **mime\.txt**\. The blank lines of **mime\.txt** and lines consisting less that three elements, are ignored\.

There are defined the most common extensions\. You can add other extensions\.

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

If you run the application without arguments, there will be displayed all possible parameters with default value and available encoding list\.

## Bundling steps

The **SingleHtmlAppBundler** performs thefollowing steps:


* **Step 0** \- Print parameters with currently used values\.
* **Step 1** \- Analyze provided file and create the dependency tree\. All HTML and JS files will be analyzed recursively\. The file tree will be printed\.
* **Step 2** \- Perform bundling starting from the tree leaves according parameters\.
* **Step 3** \- Save necessary files in the output directory\. There will be savet the root file and only the files, which is not bundled\.

The **SingleHtmlAppBundler** assumes, that all input HTML/JS files are syntactically correct and not analyzes the syntax of the JS and CSS\.

## Execution examples

Bundle with default parameters:

```
dotnet SingleHtmlAppBundler.dll InputDir/file.html OutputDir
```

Do not bundle scripts and minify JS files:

```
dotnet SingleHtmlAppBundler.dll InputDir/file.html OutputDir BundleHtmlScript=1 MinifyJsComment=1 MinifyJsWhitespace=1
```

# Html bundle

The HTML bundling is performed by analyzing the tags in the following table\. In the **Parameter** column there is parameter name, which affects bundling the tag\. If the tag is excluded by parameter, the file will be save into the output directory preserving the subdirectory\. If the tags does not relate to the other file or the file is unreadable, the tag will be ignored regardless the according parameter value\.

| Parameter | Tag | Attribute | Description |
| --- | --- | --- | --- |
| BundleHtmlBody | `body` | `background` | Page background image\. |
| BundleHtmlScript | `script` | `src` | Script file\. |
| BundleHtmlLink | `link` | `href` | Additional file \(style, icon\)\. If `rel`=`stylesheet`, the tag will be replace with `style` tag\. |
| BundleHtmlImg | `img` | `src` | Image file\. |
| BundleHtmlAudio | `audio` | `src` | Audio file\. |
| BundleHtmlVideo | `video` | `src` | Video file\. |
| BundleHtmlSource | `source` | `src` or `srcset` | Alternative file for `picture`, `audio`, `video` tag, uses the `type` attribute to determine file type\. |
| BundleHtmlTrack | `track` | `src` | Additional file for `audio` or `video` tag, uses the `type` attribute to determine file type\. |
| BundleHtmlIframe | `iframe` | `src` | Media or document inside frame\. |
| BundleHtmlEmbed | `embed` | `src` | Media or document inside frame, uses the `type` attribute to determine file type\. |
| BundleHtmlObject | `object` | `data` | Media or document inside frame\. |

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
* **Worker** \- JavaScript Worker object, which commonly relates to other JavaScript file\.

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

## JavaScript Worker

The Worker object usually is constructed by providing other JavaScript file\. Bundler creates the fake JavaScript function containing the contents of provided file and creates the blob from the function\.

There can be bundled every **Worker** token occurence, which is the preceded by **new** token and followef by bracket open and directly provided file name\.

In such cases, the **Worker** can be bundled:

```
x = new Worker("file1.js");
y = new Worker('file2.js');
```

In the examples, the Worked can not be bundled:

```
const jsfile = "file1.js";
x = new Worker(jsfile);
y = Worker("file2.js");
```

The **SingleHtmlAppBundler** does not analyze the JavaScript semantics and the flow control, but creating the **Worker** object without the **new** token is not correct\.

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

By default, the minification will not be applied, but you can turn on minification using parameters described above\.




