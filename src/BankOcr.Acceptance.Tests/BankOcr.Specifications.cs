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
            (int exitCode, IEnumerable<string> output) = runner.Run();

            Assert.Equal(1, exitCode);
            List<string> expected = new () { "ERROR: No file name given." };
            List<string> actual = output.ToList();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldErrorIfBogusFileSpecified()
        {
            string filename = @"bogus\file.txt";

            (int exitCode, IEnumerable<string> output) = runner.Run(filename);

            Assert.Equal(1, exitCode);
            List<string> expected = new () { "ERROR: Could not open file." };
            List<string> actual = output.ToList();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldSucceedWithEmptyFile()
        {
            string filename = Path.GetFullPath(Path.Join(@"TestResources", @"EmptyFile.txt"));

            (int exitCode, IEnumerable<string> output) = runner.Run(filename);

            Assert.Equal(0, exitCode);
            Assert.Empty(output);
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

            foreach (string arg in args)
                bankOcrProcess.StartInfo.ArgumentList.Add(arg);

            bool started = bankOcrProcess.Start();

            if (!started)
                throw new BankOcrProcessNoStartException($"Could not start {BankOcrFullPath}");

            bankOcrProcess.WaitForExit();

            return (bankOcrProcess.ExitCode, StringsFromStreamReader(bankOcrProcess.StandardOutput));

            IEnumerable<string> StringsFromStreamReader(StreamReader sr)
            {
                string? line;
                while ((line = sr.ReadLine()) is not null)
                    yield return line;
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
