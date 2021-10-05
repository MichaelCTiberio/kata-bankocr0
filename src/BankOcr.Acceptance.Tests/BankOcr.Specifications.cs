using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

using static System.Environment;

namespace BankOcr.Tests.Specifications
{
    public class BankOcrSpecifications : IClassFixture<BankOcrRunner>
    {
        private readonly BankOcrRunner runner;
        public BankOcrSpecifications(BankOcrRunner runner) => this.runner = runner;

        [Fact]
        public void ShouldErrorIfNoFileSpecified()
        {
            (int exitCode, var output) = runner.Run();

            Assert.Equal(1, exitCode);
            var expected = new List<string> { "ERROR: No file name given." };
            var actual = output.ToList();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldErrorIfBogusFileSpecified()
        {
            string filename = @"bogus\file.txt";

            (int exitCode, var output) = runner.Run(filename);

            Assert.Equal(1, exitCode);
            var expected = new List<string> { "ERROR: Could not open file." };
            var actual = output.ToList();
            Assert.Equal(expected, actual);
        }
    }

    public sealed class BankOcrRunner
    {
        public string BankOcrFullPath { get; }

        public BankOcrRunner()
        {
            string path = GetEnvironmentVariable(EnvInstallDir) ?? @".\";
            BankOcrFullPath = Path.GetFullPath(Path.Join(path, BankOcrExe));

            if (!File.Exists(BankOcrFullPath))
                throw new FileNotFoundException($"Could not find {BankOcrFullPath}", BankOcrFullPath);
        }

        public (int exitCode, IEnumerable<string> stdout) Run(params string [] args)
        {
            using Process bankOcrProcess = new ();

            bankOcrProcess.StartInfo.FileName = BankOcrFullPath;
            bankOcrProcess.StartInfo.UseShellExecute = false;
            bankOcrProcess.StartInfo.RedirectStandardOutput = true;

            foreach (var arg in args)
                bankOcrProcess.StartInfo.ArgumentList.Add(arg);

            bool started = bankOcrProcess.Start();

            if (!started)
                throw new BankOcrProcessNoStartException($"Could not start {BankOcrFullPath}");

            bankOcrProcess.WaitForExit();

            return (bankOcrProcess.ExitCode, StringsFromStreamReader(bankOcrProcess.StandardOutput));

            IEnumerable<string> StringsFromStreamReader(StreamReader sr)
            {
                string? line = sr.ReadLine();
                while (line is not null)
                {
                    yield return line;
                    line = sr.ReadLine();
                }
            }
        }

        private const string BankOcrExe = "BankOcr.exe";
        private const string EnvInstallDir = "INSTALL_DIR";

        public class BankOcrProcessNoStartException : Exception
        {
            public BankOcrProcessNoStartException() : base() { }
            public BankOcrProcessNoStartException(string? message) : base(message) { }
            public BankOcrProcessNoStartException(string? message, Exception? innerException) :
                base(message, innerException) { }
        }
    }
}
