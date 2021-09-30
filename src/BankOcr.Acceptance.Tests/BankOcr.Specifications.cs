using System.IO;
using Xunit;
using System;

using static System.Environment;

namespace BankOcr.Tests.Specifications
{
    public class BankOcrSpecifications
    {
        [Fact]
        public void BankOcrExeShouldBeDeployed()
        {
            BankOcrRunner runner = new ();
        }
    }

    internal sealed class BankOcrRunner
    {
        public string BankOcrFullPath { get; init; }

        public BankOcrRunner()
        {
            string path = GetEnvironmentVariable(EnvInstallDir) ?? @".\";
            BankOcrFullPath = Path.GetFullPath(Path.Join(path, BankOcrExe));

            if (!File.Exists(BankOcrFullPath))
                throw new FileNotFoundException($"Could not find {BankOcrFullPath}", BankOcrFullPath);
        }

        private const string BankOcrExe = "BankOcr.exe";
        private const string EnvInstallDir = "INSTALL_DIR";
    }
}
