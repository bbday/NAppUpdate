using NAppUpdateR.Tasks;
using NUnit.Framework;

namespace NAppUpdate.Tests.Tasks
{
    public class BasicTaskTests
    {
        [Test]
        public void TestTaskDefaultCharacteristics()
        {
            var task = new FileUpdateTask(); // just a random task object
            Assert.IsTrue(task.ExecutionStatus == TaskExecutionStatus.Pending);
        }
    }
}
