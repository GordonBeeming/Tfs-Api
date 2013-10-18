namespace TfsApi.Utilities
{
    #region

    using System.Diagnostics;

    #endregion

    internal static class CmdUtility
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Executes the specified CMD lines.
        /// </summary>
        /// <param name="cmdLines">The CMD lines.</param>
        /// <param name="output">The output.</param>
        /// <param name="error">The error.</param>
        public static void Execute(string cmdLines, out string output, out string error)
        {
            var processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;

            Process process = Process.Start(processStartInfo);

            foreach (string line in cmdLines.Split('\n'))
            {
                process.StandardInput.WriteLine(line.Trim());
            }

            process.StandardInput.Close(); // line added to stop process from hanging on ReadToEnd()

            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
        }

        #endregion
    }
}