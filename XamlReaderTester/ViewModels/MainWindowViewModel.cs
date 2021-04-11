using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Xml;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using Parser;
using Parser.Nodes;

namespace XamlReaderTester.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            CommandFileOpen = new RelayCommand(ExecFileOpen);
        }

        private void ExecFileOpen()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xaml";
            dlg.Filter = "XAML Files (*.xaml)|*.xaml|Any files (*.*)|*.*";
            dlg.InitialDirectory = Properties.Settings.Default.LastOpenDirectory;

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                Properties.Settings.Default.LastOpenDirectory = Path.GetDirectoryName(dlg.FileName);
                Properties.Settings.Default.Save();
                // Open document 
                string fileName = dlg.FileName;

                XamlParser parser = new XamlParser(new TraceErrorHandler());
                XamlMainObjectNode mainObjectNode = parser.Parse(fileName);

                //Parse01(fileName);
                //Parse03(fileName);
            }
        }

        private void Parse02(string fileName)
        {
            //XmlReader test;
            //XmlReader.Create("abs");
            // Create the XmlNamespaceManager.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);

            // Create the XmlParserContext.
            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            // Create the reader.
            //XmlTextReader reader = new XmlTextReader(xmlFrag, XmlNodeType.Element, context);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.NameTable = nt;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(fileStream, XmlNodeType.Element, context))
                {
                    while (!reader.EOF)
                    {
                        reader.ReadStartElement();
                        if (reader.HasAttributes)
                        {
                            reader.MoveToFirstAttribute();
                            if (reader.HasValue)
                            {
                                reader.ReadAttributeValue();
                                if (!String.IsNullOrEmpty(reader.Value))
                                {
                                    Trace.WriteLine(
                                        string.Format(
                                            "{0}.{1} Name:{2}:{3} ({4}), node type {5}, value type {6}, value '{7}'",
                                            reader.LineNumber,
                                            reader.LinePosition,
                                            reader.Prefix,
                                            reader.Name,
                                            reader.AttributeCount,
                                            reader.NodeType,
                                            reader.ValueType,
                                            ConvertToPrintable(reader.Value)));
                                    //reader.Value = "\r\n    "
                                }
                            }
                            else
                            {
                                Trace.WriteLine(
                                    string.Format(
                                        "{0}.{1} Name:{2}:{3} ({4}), node type {5}",
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.Prefix,
                                        reader.Name,
                                        reader.AttributeCount,
                                        reader.NodeType));
                            }
                        }
                      

                        //while (reader.MoveToNextAttribute())
                        //{
                        //    if (reader.HasValue)
                        //    {
                        //        Trace.WriteLine(
                        //            string.Format(
                        //                "\t{0}.{1} Name:{2} , node type {3}, value type {4}, value {5}",
                        //                reader.LineNumber,
                        //                reader.LinePosition,
                        //                reader.Name,
                        //                reader.NodeType,
                        //                reader.ValueType,
                        //                ConvertToPrintable(reader.Value)));
                        //    }
                        //    else
                        //    {
                        //        Trace.WriteLine(
                        //            string.Format(
                        //                "\t{0}.{1} Name:{2}, node type {3}",
                        //                reader.LineNumber,
                        //                reader.LinePosition,
                        //                reader.Name,
                        //                reader.NodeType));
                        //    }
                        //}
                    }
                }
            }
        }
        private void Parse01(string fileName)
        {
            //XmlReader test;
            //XmlReader.Create("abs");
            // Create the XmlNamespaceManager.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);

            // Create the XmlParserContext.
            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            // Create the reader.
            //XmlTextReader reader = new XmlTextReader(xmlFrag, XmlNodeType.Element, context);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.NameTable = nt;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(fileStream, XmlNodeType.Element, context))
                {
                    while (!reader.EOF)
                    {
                        reader.Read();
                        if (reader.HasValue)
                        {
                            if (!String.IsNullOrEmpty(reader.Value))
                            {
                                Trace.WriteLine(
                                    string.Format(
                                        "{0}.{1} Name:{2}:{3} ({4}), node type {5}, value type {6}, value '{7}'",
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.Prefix,
                                        reader.Name,
                                        reader.AttributeCount,
                                        reader.NodeType,
                                        reader.ValueType,
                                        ConvertToPrintable(reader.Value)));
                                //reader.Value = "\r\n    "
                            }
                        }
                        else
                        {
                            Trace.WriteLine(
                                string.Format(
                                    "{0}.{1} Name:{2}:{3} ({4}), node type {5}",
                                    reader.LineNumber,
                                    reader.LinePosition,
                                    reader.Prefix,
                                    reader.Name,
                                    reader.AttributeCount,
                                    reader.NodeType));
                        }

                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.HasValue)
                            {
                                Trace.WriteLine(
                                    string.Format(
                                        "\t{0}.{1} Name:{2} , node type {3}, value type {4}, value {5}",
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.Name,
                                        reader.NodeType,
                                        reader.ValueType,
                                        ConvertToPrintable(reader.Value)));
                            }
                            else
                            {
                                Trace.WriteLine(
                                    string.Format(
                                        "\t{0}.{1} Name:{2}, node type {3}",
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.Name,
                                        reader.NodeType));
                            }
                        }
                    }
                }
            }
        }
        private void Parse03(string fileName)
        {
           
            // Create the XmlNamespaceManager.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);

            // Create the XmlParserContext.
            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            // Create the reader.
            //XmlTextReader reader = new XmlTextReader(xmlFrag, XmlNodeType.Element, context);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.NameTable = nt;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(fileStream, XmlNodeType.Element, context))
                {
                    while (!reader.EOF)
                    {
                        reader.Read();
                        if (reader.HasValue)
                        {
                            if (!String.IsNullOrEmpty(reader.Value))
                            {
                                Trace.WriteLine(
                                    string.Format(
                                        "{0}.{1} Name:{2}:{3} ({4}), node type {5}, value type {6}, value '{7}'",
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.Prefix,
                                        reader.Name,
                                        reader.AttributeCount,
                                        reader.NodeType,
                                        reader.ValueType,
                                        ConvertToPrintable(reader.Value)));
                                //reader.Value = "\r\n    "
                            }
                        }
                        else
                        {
                            Trace.WriteLine(
                                string.Format(
                                    "{0}.{1} Name:{2}:{3} ({4}), node type {5}",
                                    reader.LineNumber,
                                    reader.LinePosition,
                                    reader.Prefix,
                                    reader.Name,
                                    reader.AttributeCount,
                                    reader.NodeType));
                        }

                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.HasValue)
                            {
                                string attributeName = reader.Name;
                                int lineNumberTag = reader.LineNumber;
                                int linePositionTag = reader.LinePosition;
                                reader.ReadAttributeValue();
                                Trace.WriteLine(
                                    string.Format(
                                        "\t{0}.{1} Name:{2} , node type {3}, {4}.{5} value type {6}, value {7}",
                                        lineNumberTag,
                                        linePositionTag,
                                        attributeName,
                                        reader.NodeType,
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.ValueType,
                                        ConvertToPrintable(reader.Value)));
                            }
                            else
                            {
                                Trace.WriteLine(
                                    string.Format(
                                        "\t{0}.{1} Name:{2}, node type {3}",
                                        reader.LineNumber,
                                        reader.LinePosition,
                                        reader.Name,
                                        reader.NodeType));
                            }
                        }
                    }
                }
            }
        }

        private string ConvertToPrintable(string readerValue)
        {
            if (!String.IsNullOrEmpty(readerValue))
            {
                if (readerValue[0] == '\r')
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char ch in readerValue)
                    {
                        if (ch == '\n')
                        {
                            sb.Append("\\n");
                        }
                        else if (ch == '\r')
                        {
                            sb.Append("\\r");
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                    }

                    return sb.ToString();
                }
                return readerValue;
            }

            return readerValue;
        }

        public ICommand CommandFileOpen
        {
            get;
        }
    }
}
