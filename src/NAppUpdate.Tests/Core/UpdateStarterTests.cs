using NAppUpdateR;
using NAppUpdateR.Tasks;
using NAppUpdateR.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NAppUpdate.Tests.Core
{
    public class UpdateStarterTests
    {
        [Test]
        public void UpdaterDeploymentWorks()
        {
            var path = Path.Combine(Path.GetTempPath(), "NAppUpdate-Tests");

            NauIpc.ExtractUpdaterFromResource(path);

            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdateR.exe")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdateR.dll")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdateR.runtimeconfig.json")));

            // Cleanup test
            FileSystem.DeleteDirectory(path);
        }

        [Test]
        public void UpdaterDeploymentAndIPCWorks()
        {
            var dto = new NauIpc.NauDto
            {
                Configs = UpdateManager.Instance.Config,
                Tasks = new List<IUpdateTask>
                {
                    new FileUpdateTask {Description = "Task #1", ExecutionStatus = TaskExecutionStatus.RequiresAppRestart},
                },
                AppPath = Process.GetCurrentProcess().MainModule.FileName,
                WorkingDirectory = Environment.CurrentDirectory,
                RelaunchApplication = false,
            };

            var path = dto.Configs.TempFolder;

            if (Directory.Exists(path))
                FileSystem.DeleteDirectory(path);

            NauIpc.ExtractUpdaterFromResource(path);
            var info = new ProcessStartInfo
            {
                //UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Path.Combine(path, dto.Configs.UpdateExecutableName),
                Arguments = string.Format(@"""{0}"" -log", dto.Configs.UpdateProcessName),
            };

            var p = NauIpc.LaunchProcessAndSendDto(dto, info, dto.Configs.UpdateProcessName);
            Assert.IsNotNull(p);
            p.WaitForExit();

            Assert.IsTrue(Directory.Exists(path));
            Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdateR.exe")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdateR.dll")));
            Assert.IsTrue(File.Exists(Path.Combine(path, "NAppUpdateR.runtimeconfig.json")));

            // Cleanup test
            FileSystem.DeleteDirectory(path);
        }
    }
}
