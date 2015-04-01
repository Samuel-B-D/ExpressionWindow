using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace SimpleResourceDictionaryMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            string SolutionDir = args[0].Replace("\"", "");
            string ThemeColors = Path.Combine(SolutionDir, @"ExpressionWindow\Themes\Sources\Colors");
            string ThemeOutput = Path.Combine(SolutionDir, @"ExpressionWindow\Themes");
            Console.WriteLine("Solution dir : {0}", SolutionDir);
            Console.WriteLine("Theme colors path : {0}", ThemeColors);
            Console.WriteLine("Theme output path : {0}", ThemeOutput);
            foreach (string file in Directory.EnumerateFiles(ThemeColors))
            {
                Console.WriteLine();
                Console.WriteLine(".. Processing : {0}", file);
                string FileName = Path.GetFileNameWithoutExtension(file);
                XmlDocument Doc = new XmlDocument();
                XmlElement Root = Doc.CreateElement("ResourceDictionary", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                Root.SetAttribute("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
                Root.SetAttribute("xmlns:mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
                Root.SetAttribute("xmlns:d", "http://schemas.microsoft.com/expression/blend/2008");
                Root.SetAttribute("xmlns:local", "clr-namespace:ThemedWindows");
                Root.SetAttribute("xmlns:Controls", "clr-namespace:ThemedWindows.Controls;assembly=ExpressionWindow");
                Root.SetAttribute("xmlns:Effects", "clr-namespace:ThemedWindows.Effects;assembly=ExpressionWindow");

                Console.WriteLine(".. Loading base XAML");
                XmlDocument baseTheme = new XmlDocument();
                baseTheme.Load(Path.Combine(SolutionDir, @"ExpressionWindow\Themes\Sources\ExpressionDarkBase.xaml"));
                
                //Import topmost comment if there is one
                if (baseTheme.FirstChild.NodeType == XmlNodeType.Comment)
                    Doc.AppendChild(Doc.ImportNode(baseTheme.FirstChild, false));

                //Import the Colors
                XmlDocument colors = new XmlDocument();
                colors.Load(file);
                foreach (XmlNode node in colors.GetElementsByTagName("ResourceDictionary")[0].ChildNodes)
                {
                    Root.AppendChild(Doc.ImportNode(node, true));
                }

                bool Ignore = false;
                //Import content of Base Theme
                var Nodes = baseTheme.GetElementsByTagName("ResourceDictionary")[0].ChildNodes;
                foreach (XmlNode node in Nodes)
                {
                    if (node.NodeType == XmlNodeType.Comment && node.InnerText.Trim() == "[IGNORE]")
                        Ignore = true;
                    if (node.NodeType == XmlNodeType.Comment && node.InnerText.Trim() == "[ENDIGNORE]")
                        Ignore = false;
                    if (!Ignore)
                        Root.AppendChild(Doc.ImportNode(node, true));
                }

                Doc.AppendChild(Root);

                Console.WriteLine(".. Creating output : {0}", ThemeOutput + "\\" + FileName+ "Colors.xaml");

                Doc.Save(XmlWriter.Create(ThemeOutput + "\\" + FileName+ "Colors.xaml", new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Auto, OmitXmlDeclaration = true, NewLineHandling = NewLineHandling.Entitize, NewLineOnAttributes = true, Indent = true }));
            }
            Console.WriteLine();
            Console.WriteLine("DONE!");
            Console.WriteLine();
        }
    }
}
