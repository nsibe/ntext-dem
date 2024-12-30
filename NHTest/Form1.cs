using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NHTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // HTML dosyalarının bulunduğu dizin
            string directoryPath = textBox1.Text;
            string outputXmlFilePath = "output.xml";

            // XML yazıcı oluştur
            using (XmlWriter writer = XmlWriter.Create(outputXmlFilePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Rows");

                // Dizin içindeki tüm .HTM dosyalarını listele
                var htmlFiles = Directory.GetFiles(directoryPath, "*.HTM");

                foreach (var htmlFile in htmlFiles)
                {
                    // Her bir HTML dosyasını işleme
                    ProcessHtmlFile(htmlFile, writer);
                }

                writer.WriteEndElement(); // Rows
                writer.WriteEndDocument();
            }

            Console.WriteLine("XML dosyası başarıyla oluşturuldu.");
        }

        HtmlNode FindSegmentTable(HtmlAgilityPack.HtmlDocument doc)
        {
            // Tüm tabloları al
            var tables = doc.DocumentNode.SelectNodes("//table");

            // Tabloyu kontrol et
            foreach (var table in tables)
            {
                var header = table.SelectSingleNode(".//tr/td/div[contains(text(), 'SEG')]");

                if (header != null)
                {
                    return table; // "SEG" başlığına sahip tabloyu döndür
                }
            }
            return null; // Eğer "SEG" başlıklı tablo bulunmazsa null döner
        }

        HtmlNode FindSubTable(HtmlAgilityPack.HtmlDocument doc)
        {
            // Tüm tabloları al
            var tables = doc.DocumentNode.SelectNodes("//table");

            // Tabloyu kontrol et
            foreach (var table in tables)
            {
                var header = table.SelectSingleNode(".//tr/td/div[contains(text(), 'DESIGNATOR')]");

                if (header != null)
                {
                    return table; // "DESIGNATOR" başlığına sahip tabloyu döndür
                }
            }
            return null; // Eğer "DESIGNATOR" başlıklı tablo bulunmazsa null döner
        }


        void ProcessHtmlFile(string htmlFilePath, XmlWriter writer)
        {
            // HTML dosyasını yükle
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load(htmlFilePath);
            richTextBox1.AppendText(htmlFilePath);
            richTextBox1.AppendText("   ");
            richTextBox1.AppendText(htmlDoc.DocumentNode.OuterHtml);
           

            // "SEG" başlığına sahip tabloyu bul
            var table = FindSegmentTable(htmlDoc);

            if (table != null)
            {
                // Bu dosyada bulunan tabloyu XML'e ekle
                WriteTableDataToXml(table, writer);
            }
        }

        void ProcessSubHtmlFile(string htmlFilePath, XmlWriter writer)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load(htmlFilePath);

            var table = FindSubTable(htmlDoc);

            
        }

        
        void WriteTableDataToXml(HtmlNode table, XmlWriter writer)
        {
            // Tablo satırlarını al (ilk satır başlık, diğerleri veri)
            var rows = table.SelectNodes(".//tr[@class='normal']");

            foreach (var row in rows)
            {
                writer.WriteStartElement("Row");

                // Hücrelerdeki verileri al
                var cells = row.SelectNodes(".//td");
                if (cells != null && cells.Count > 0)
                {
                    // Her hücreyi işleyip uygun XML öğesine ekle
                    writer.WriteAttributeString("SETID", cells[3].InnerText.Trim());
                    writer.WriteAttributeString("SEQ", cells[4].InnerText.Trim());
                    writer.WriteAttributeString("CLS", cells[5].InnerText.Trim());
                    writer.WriteAttributeString("SET_FORMAT_NAME", cells[6].InnerText.Trim());
                    writer.WriteAttributeString("REMARKS", cells[7].InnerText.Trim());

                    // Eğer SETID hücresinde bir <a> etiketi varsa, href değerini al
                    var linkNode = cells[3].SelectSingleNode(".//a");
                    var link = "";
                    if (linkNode != null)
                    {
                        writer.WriteAttributeString("HREF", linkNode.GetAttributeValue("href", ""));
                        link = linkNode.GetAttributeValue("href", "");
                    }

                    var fileName = link;
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(fileName);

                    var subTable = FindSubTable(htmlDoc);

                    var subRows = subTable.SelectNodes(".//tr[@class='normal']");

                    foreach (var subRow in subRows)
                    {
                        var subCells = subRow.SelectNodes(".//td");
                        if (subCells != null && subCells.Count > 0)
                        {
                            writer.WriteStartElement("Row");
                        }
                    }

                    writer.WriteEndElement(); // Row
                }

            }
        }
    }
}
