using System;
using Xunit.Sdk;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace BankOcr.Test
{
    internal static class SupportConstants
    {
        internal const string AssemblyName = "BankOcr.Test";
        internal const string NamespacePrefix = "BankOcr.Test" + ".";

        internal const string TestSuite = "TestSuite";
        internal const string AcceptanceTest = "AcceptanceTests";
        internal const string CommitTest = "CommitTests";
    }

    [TraitDiscoverer(CommitTestsDiscoverer.DiscovererTypeName, SupportConstants.AssemblyName)]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CommitTestsAttribute : Attribute, ITraitAttribute
    {
        public CommitTestsAttribute() { }
    }

    public class CommitTestsDiscoverer : ITraitDiscoverer
    {
        internal const string DiscovererTypeName = SupportConstants.NamespacePrefix + nameof(CommitTestsDiscoverer);

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>(SupportConstants.TestSuite, SupportConstants.CommitTest);
        }
    }

    [TraitDiscoverer(AcceptanceTestsDiscoverer.DiscovererTypeName, SupportConstants.AssemblyName)]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AcceptanceTestsAttribute : Attribute, ITraitAttribute
    {
        public AcceptanceTestsAttribute() { }
    }

    public class AcceptanceTestsDiscoverer : ITraitDiscoverer
    {
        internal const string DiscovererTypeName = SupportConstants.NamespacePrefix + nameof(AcceptanceTestsDiscoverer);

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>(SupportConstants.TestSuite, SupportConstants.AcceptanceTest);
        }
    }
}
