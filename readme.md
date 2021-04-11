# XAML parser
## Overview ##

Microsoft has *XmlReader* and *XamlXmlReader classes*. You can use it for XAML parsing.
But if you need to know exact attribute position in file or read comments, there is no way for this tasks as normally nobody need it.
But as I need to manupulate XAML files, I really need such a futures. That why I decided to write this parser.
It is native C# parser.

At this time this is only working prototype. No downloads.


## Description

It is possible to parse the XAML file or string with the XAML text. As a result you will have a tree with nodes.
It is possible to define own error handler.
Every node will have tag name (include namespaces), attributes list, comments and possible value as text.
For all parts exists start/end line and character position.

## System requirements

Actual version used .Net Framework 4.7.2


## How to use

For parsing you have one class *XamlParser* and can use *ExceptionErrorHandler* or implement own handler like this one:

```C#
public class TraceErrorHandler : IErrorHandler
{
        public void Error(string text, SymbolScanner scanner)
        {
            Trace.WriteLine(
                string.Format("Parse error:{0} before line {1} position {2} symbol '{3}'",
                    text,
                    scanner.LineNumber,
                    scanner.CharPosition,
                    scanner.NextSymbol));
        }
}
```

Then simply use *XamlParser*:
```C#
XamlParser parser = new XamlParser(new ExceptionErrorHandler());
XamlMainObjectNode mainObjectNode = parser.Parse(fileName);
```
For each node possible to check *Name*, *Attributes*, *Children* and occuped area (*LineNumberStart*, *LineNumberEnd*, *LinePositionStart*, *LinePositionEnd*)

## Current Limitation

- It is not allowed to have comments outside the main tag.
- Delimiters for strings is *"* only.
- No escape symbol \ allowed.
- No fully support of [XAML Grammar](XamlParser/Syntax.md)

## Repository description

There is a lot more that library source code.
Directories:
- TestFiles - contains XAML test collection
- UnitTestProjectXamlReader - contains Unit test for XAML parser project
- WpfParser - first XAML parser implementation based on Microsoft classes. In some caseses I use it as reference project
- XamlParser - Xaml parser library
- XamlReaderTester - simple WPF application for testing real XAML files. Output into Trace window.

## Additional links

[XAML language specification from Microsoft](https://download.microsoft.com/download/0/A/6/0A6F7755-9AF5-448B-907D-13985ACCF53E/%5BMS-XAML%5D.pdf)

[EBNF for XML 1.0](http://jelks.nu/XML/xmlebnf.html)

[XML 1.1 EBNF](https://www.liquid-technologies.com/XML/EBNF1.1.aspx)
