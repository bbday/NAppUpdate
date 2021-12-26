using NAppUpdateR.Common;
using NAppUpdateR.Tasks;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace NAppUpdateR.Utils
{
    /// <summary>
    /// Starts the cold update process by extracting the updater app from the library's resources,
    /// passing it all the data it needs and terminating the current application
    /// </summary>
    public static class NauIpc
    {
        [Serializable]
        public class NauDto
        {
            public NauConfigurations Configs { get; set; }
            public IList<IUpdateTask> Tasks { get; set; }
            public List<Logger.LogItem> LogItems { get; set; }
            public string AppPath { get; set; }
            public string WorkingDirectory { get; set; }
            public bool RelaunchApplication { get; set; }
        }

        private const int PIPE_TIMEOUT = 15000;

        /// <summary>
        /// Launches the specifies process and sends the dto object to it using a named pipe
        /// </summary>
        /// <param name="dto">Dto object to send</param>
        /// <param name="processStartInfo">Process info for the process to start</param>
        /// <param name="syncProcessName">Name of the pipe to write to</param>
        /// <returns>The started process</returns>
        public static Process LaunchProcessAndSendDto(NauDto dto, ProcessStartInfo processStartInfo, string syncProcessName)
        {
            Process p;

            using (NamedPipeServerStream pipe = new NamedPipeServerStream(syncProcessName, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                p = Process.Start(processStartInfo);

                if (p == null)
                {
                    throw new ProcessStartFailedException("The process failed to start");
                }

                var asyncResult = pipe.BeginWaitForConnection(null, null);

                if (asyncResult.AsyncWaitHandle.WaitOne(PIPE_TIMEOUT))
                {
                    pipe.EndWaitForConnection(asyncResult);

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(pipe, dto);
                }
                else if (p.HasExited)
                {
                    Type exceptionType = Marshal.GetExceptionForHR(p.ExitCode).GetType();

                    throw new TimeoutException(string.Format("The NamedPipeServerStream timed out waiting for a named pipe connection, " +
                        "but the process has exited with exit code: {0} ({1})", p.ExitCode, exceptionType.FullName));
                }
                else
                {
                    throw new TimeoutException("The NamedPipeServerStream timed out waiting for a named pipe connection.");
                }
            }

            return p;
        }

        /// <summary>
        /// Reads the dto object from the named pipe
        /// </summary>
        /// <param name="syncProcessName">Name of the pipe to read from</param>
        /// <returns>The dto object read from the pipe</returns>
        public static NauDto ReadDto(string syncProcessName)
        {
            NauDto dto;

            using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", syncProcessName, PipeDirection.In, PipeOptions.Asynchronous))
            {
                pipe.Connect(PIPE_TIMEOUT);

                BinaryFormatter formatter = new BinaryFormatter();
                dto = formatter.Deserialize(pipe) as NauDto;
            }

            if (dto == null || dto.Configs == null)
            {
                throw new Exception("Failed to read the dto from the pipe stream");
            }

            return dto;
        }

        public static void ExtractUpdaterFromResource(string updaterPath)
        {
            if (!Directory.Exists(updaterPath))
                Directory.CreateDirectory(updaterPath);

            var assemblyLocation = Path.GetDirectoryName(typeof(NauIpc).Assembly.Location);
            File.Copy(Path.Combine(assemblyLocation, "NAppUpdateR.exe"), Path.Combine(updaterPath, "NAppUpdateR.exe"), true);
            File.Copy(Path.Combine(assemblyLocation, "NAppUpdateR.dll"), Path.Combine(updaterPath, "NAppUpdateR.dll"), true);
            File.Copy(Path.Combine(assemblyLocation, "NAppUpdateR.runtimeconfig.json"), Path.Combine(updaterPath, "NAppUpdateR.runtimeconfig.json"), true);

            // And also all other referenced DLLs (opt-in only)
            var assemblyPath = Path.GetDirectoryName(assemblyLocation) ?? string.Empty;
            if (UpdateManager.Instance.Config.DependenciesForColdUpdate == null) return;
            // TODO Maybe we can back this up with typeof(UpdateStarter).Assembly.GetReferencedAssemblies()

            foreach (var dep in UpdateManager.Instance.Config.DependenciesForColdUpdate)
            {
                string fullPath = Path.Combine(assemblyPath, dep);
                if (!File.Exists(fullPath)) continue;

                var dest = Path.Combine(updaterPath, dep);
                FileSystem.CreateDirectoryStructure(dest);
                File.Copy(fullPath, Path.Combine(updaterPath, dep), true);
            }
        }
    }
}
