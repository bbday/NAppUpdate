using NAppUpdateR.Utils;
using NUnit.Framework;

namespace NAppUpdate.Tests.Integration
{
    public class FileDownloaderTests
    {
        [Test]
        public void Should_be_able_to_download_a_small_file_from_the_internet()
        {
            var fileDownloader = new FileDownloader("http://www.google.co.uk/intl/en_uk/images/logo.gif");

            byte[] fileData = fileDownloader.Download();

            Assert.IsTrue(fileData.Length > 0);
        }
    }
}
