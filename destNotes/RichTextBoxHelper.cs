using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace destNotes
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static void SetDocumentXaml(DependencyObject obj, string value) => 
            obj.SetValue(DocumentXamlProperty, value);

        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached(
                "DocumentXaml",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (obj, e) =>
                    {
                        var richTextBox = (RichTextBox)obj;

                        // Parse the XAML to a document (or use XamlReader.Parse())
                        var doc = new FlowDocument();
                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);

                        if (!File.Exists($"{e.NewValue}.xaml")) return;
                        var fStream = new FileStream($"{e.NewValue}.xaml", FileMode.OpenOrCreate);
                        range.Load(fStream, DataFormats.XamlPackage);
                        fStream.Close();

                        // Set the document
                        richTextBox.Document = doc;
                    }
                });
    }
}