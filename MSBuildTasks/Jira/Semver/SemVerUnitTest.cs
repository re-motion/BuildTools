using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Remotion.BuildTools.MSBuildTasks.Jira.Semver
{
    [TestFixture]
    class SemVerUnitTest
    {
        private SemVerParser semVerParser;

        [SetUp]
        public void SetUp()
        {
            semVerParser = new SemVerParser();    
        }

        [Test]
        public void SemVer_WithourPre_ShouldParse()
        {
            string version = "1.2.3";
            SemVer semver = semVerParser.ParseVersion(version);

            Assert.AreEqual(1, semver.Major);
            Assert.AreEqual(2, semver.Minor);
            Assert.AreEqual(3, semver.Patch);

            Assert.IsNull(semver.Pre);
            Assert.IsNull(semver.PreVersion);
        }

        [Test]
        public void SemVer_WithPre_ShouldParse()
        {
            string version = "1.2.3-alpha.4";
            SemVer semver = semVerParser.ParseVersion(version);

            Assert.AreEqual(1, semver.Major);
            Assert.AreEqual(2, semver.Minor);
            Assert.AreEqual(3, semver.Patch);
            
            Assert.AreEqual(PreEnum.alpha, semver.Pre);
            Assert.AreEqual(4, semver.PreVersion);
        }

        [Test]
        public void SemVer_InvalidFormat_ShouldThrowArgumentException()
        {
            Assert.That(() => semVerParser.ParseVersion("TotalInvalidFormat"), 
                        Throws.ArgumentException
                              .With.Message.EqualTo("Version has an invalid format. Expected equivalent to '1.2.3' or '1.2.3-alpha.4'"));

            Assert.That(() => semVerParser.ParseVersion("1.2.3-invalid.4"),
                        Throws.ArgumentException
                              .With.Message.EqualTo("Version has an invalid format. Expected equivalent to '1.2.3' or '1.2.3-alpha.4'"));

            Assert.That(() => semVerParser.ParseVersion("1.2.3.4"),
                        Throws.ArgumentException
                              .With.Message.EqualTo("Version has an invalid format. Expected equivalent to '1.2.3' or '1.2.3-alpha.4'"));

            Assert.That(() => semVerParser.ParseVersion("1.2.3.alpha-4"),
                        Throws.ArgumentException
                              .With.Message.EqualTo("Version has an invalid format. Expected equivalent to '1.2.3' or '1.2.3-alpha.4'"));
        }


        [Test]
        public void SemVer_Ordering()
        {
            var semVer1 = semVerParser.ParseVersion("1.2.3");
            var semVer2 = semVerParser.ParseVersion("1.2.4");
            var semVer3 = semVerParser.ParseVersion("1.3.0");
            var semVer4 = semVerParser.ParseVersion("1.4.0");
            var semVer5 = semVerParser.ParseVersion("1.4.0-alpha.1");
            var semVer6 = semVerParser.ParseVersion("1.4.0-beta.1");
            var semVer7 = semVerParser.ParseVersion("1.4.0-beta.2");
            var semVer8 = semVerParser.ParseVersion("1.4.0-rc.1");
            var semVer9 = semVerParser.ParseVersion("2.0.0");

            var semVerList = new List<SemVer>() {semVer9, semVer8, semVer7, semVer6, semVer5, semVer4, semVer3, semVer2, semVer1};

            var orderedList = semVerList.OrderBy(x => x).ToList();

            Assert.AreEqual(semVer1, orderedList[0]);
            Assert.AreEqual(semVer2, orderedList[1]);
            Assert.AreEqual(semVer3, orderedList[2]);
            Assert.AreEqual(semVer4, orderedList[3]);
            Assert.AreEqual(semVer5, orderedList[4]);
            Assert.AreEqual(semVer6, orderedList[5]);
            Assert.AreEqual(semVer7, orderedList[6]);
            Assert.AreEqual(semVer8, orderedList[7]);
            Assert.AreEqual(semVer9, orderedList[8]);
        }
    }
}
