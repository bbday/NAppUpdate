using NAppUpdateR.FeedReaders;
using NAppUpdateR.Sources;
using NUnit.Framework;

namespace NAppUpdate.Tests.Sources
{
    /// <summary>
    /// Tests for different source file feed encodings: UTF-8 UTF-16LE UTF-16BE ANSI
    /// </summary>
	public class SourceEncodingTests
    {
        private void ReadUpdateFeed(IUpdateSource sc)
        {
            IUpdateFeedReader fr = new NauXmlFeedReader();

            var tasks = fr.Read(sc.GetUpdatesFeed());

            Assert.IsNotNull(tasks);
            Assert.IsTrue(tasks.Count == 3);
            Assert.IsTrue(tasks[0].Description.StartsWith("ÖÜÄ - some non 7-bit chars"));
        }

        [Test]
        public void ReadUTF8File()
        {
            ReadUpdateFeed(new UncSource(@"..\..\..\Sources\TestFeedXML.utf-8.xml", null));
        }

        [Test]
        public void ReadUTF8BOMFile()
        {
            ReadUpdateFeed(new UncSource(@"..\..\..\Sources\TestFeedXML.utf-8.bom.xml", null));
        }

        [Test]
        public void ReadUTF16BigEndianFile()
        {
            ReadUpdateFeed(new UncSource(@"..\..\..\Sources\TestFeedXML.utf-16.be.xml", null));
        }

        [Test]
        public void ReadUTF16LittleEndianFile()
        {
            ReadUpdateFeed(new UncSource(@"..\..\..\Sources\TestFeedXML.utf-16.le.xml", null));
        }
        /*
        [Test]
        public void ReadANSIFile()
        {
            ReadUpdateFeed(new UncSource(@"..\..\..\Sources\TestFeedXML.ansi.xml", null));
        }
        */
    }
}
