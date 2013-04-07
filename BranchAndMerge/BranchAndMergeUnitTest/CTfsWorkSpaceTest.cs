using BranchAndMerge.lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace BranchAndMergeUnitTest
{
    
    
    /// <summary>
    ///This is a test class for CTfsWorkSpaceTest and is intended
    ///to contain all CTfsWorkSpaceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTfsWorkSpaceTest
    {
        private static CTfsTeamProjectCollection ctpc;
        private static CTfsVersionControl cvc;
        private static CTfsWorkSpace cws;
        private static string localPath;
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ctpc = new CTfsTeamProjectCollection("http://192.168.81.58:8080/tfs/TfsExp");
            CTfsVersionControl cvc = new CTfsVersionControl(ctpc);
            localPath = @"d:\merge\TfsExp";
            
            try
            {
                cws = cvc.GetWorkSpace(localPath);
            }
            catch
            {
                cvc.CreateWorkSpace("TfsExp", localPath);
                cws = cvc.CWorkSpace;
            }
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod(), ExpectedException(typeof(Microsoft.TeamFoundation.VersionControl.Client.ItemNotMappedException))]
        public void GetTest()
        {
            cws.Get(new string[] { "d:\\dsfaeq" }, VersionSpec.Latest);
        }

        [TestMethod]
        public void GetTest1()
        {
            cws.Get(new string[] { "$/Sample/mainline/UAT" }, VersionSpec.Latest);
        }

        /// <summary>
        ///A test for CheckIn
        ///</summary>
        [TestMethod()]
        public void CheckInTest()
        {
            string comment = "Test Merge";
            Assert.AreEqual(cws.Merge(@"$/scm/MainLine/Code", @"d:\system_test\scm\Branches\ProjectName02"), 0);
            cws.CheckIn(@"d:\system_test\scm\Branches\ProjectName02", comment);
        }

        /// <summary>
        ///A test for Merge
        ///</summary>
        [TestMethod()]
        public void MergeTest()
        {
            string sourcePath = "$/Balance/Release/Release_093_20130324";
            cws.Get(new string[] { "D:\\merge\\TfsExp\\Balance\\MainLine\\code" }, VersionSpec.Latest);
            int expected = 0;
            int actual;
            actual = cws.Merge(sourcePath, "D:\\merge\\TfsExp\\Balance\\MainLine\\code");
            Assert.AreNotEqual(expected, actual);
        }


        ///// <summary>
        /////A test for Merge
        /////</summary>
        [TestMethod()]
        public void MergeTest1()
        {
            string sourcePath = "$/Balance/Release/Release_080_20121205";
            cws.Get(new string[] { "D:\\merge\\TfsExp\\Balance\\MainLine\\code" }, VersionSpec.Latest);
            int expected = 0;
            int actual;
            actual = cws.Merge(sourcePath, "D:\\merge\\TfsExp\\Balance\\MainLine\\code");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FolderDiff
        ///</summary>
        [TestMethod()]
        public void FolderDiffTest()
        {
            string sourcePath = "$/Balance/Release/Release_093_20130324";
            cws.Get(new string[] { "D:\\merge\\TfsExp\\Balance\\MainLine\\code" }, VersionSpec.Latest);

            string targetPath = "D:\\merge\\TfsExp\\Balance\\MainLine\\code";
            string expected = string.Empty; 
            string actual;

            actual = cws.FolderDiff(sourcePath, targetPath);
            bool isDiff = actual.Contains("1 folders, 2 files, 0 source, 1 target, 0 different, 0 with errors");
            Assert.IsTrue(isDiff);

        }

        /// <summary>
        ///A test for FolderDiff
        ///</summary>
        [TestMethod()]
        public void FolderDiffTest1()
        {
            string sourcePath = "$/Balance/Release/Release_080_20121205";
            cws.Get(new string[] { "D:\\merge\\TfsExp\\Balance\\MainLine\\code" }, VersionSpec.Latest);

            string targetPath = "D:\\merge\\TfsExp\\Balance\\MainLine\\code";
            string expected = string.Empty;
            string actual;

            actual = cws.FolderDiff(sourcePath, targetPath);
            bool isDiff = actual.Contains("0 source, 0 target, 0 different, 0 with errors");
            Assert.IsTrue(isDiff);
        }
    }
}
