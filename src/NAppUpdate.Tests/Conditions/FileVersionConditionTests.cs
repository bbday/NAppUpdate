using NAppUpdateR.Conditions;
using NUnit.Framework;
using System.IO;

namespace NAppUpdate.Tests.Conditions
{
    public class FileVersionConditionTests
    {
        [Test]
        public void ShouldAbortGracefullyOnUnversionedFiles()
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "foo");

            var cnd = new FileVersionCondition { ComparisonType = "is", LocalPath = tempFile, Version = "1.0.0.0" };
            Assert.IsTrue(cnd.IsMet(null));
        }
    }
}
