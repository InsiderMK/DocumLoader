using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Linq;
using System.IO.Compression;

namespace DocumLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            ScanDirectory(args[0]);
        }

        private static void ScanDirectory(string dirPath)
        {
            DirectoryInfo info = new DirectoryInfo(dirPath);

            foreach (FileInfo file in info.GetFiles().OrderBy(p => p.LastWriteTime))
            {
                ExtractFile(file);
            }
        }

        private static void ExtractFile(FileInfo file)
        {
            using (FileStream archiveStream = file.OpenRead())
            using (GZipStream dataStream = new GZipStream(archiveStream, CompressionMode.Decompress))
            {
                ParseXml(dataStream);
            }
        }

        private static void ParseXml(Stream dataStream)
        {
            DocumParser parser = new DocumParser();
            CaseInfo caseInfo = parser.ParseFile(dataStream);

            //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(caseInfo))
            //{
            //    string name = descriptor.Name;
            //    object value = descriptor.GetValue(caseInfo);
            //    Console.WriteLine("{0}={1}", name, value);
            //}
        }
    }
}
