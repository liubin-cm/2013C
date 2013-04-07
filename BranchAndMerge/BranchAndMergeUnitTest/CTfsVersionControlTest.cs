using BranchAndMerge.lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace BranchAndMergeUnitTest
{
    
    
    /// <summary>
    ///This is a test class for CTfsVersionControlTest and is intended
    ///to contain all CTfsVersionControlTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTfsVersionControlTest
    {


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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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
        ///A test for CreateBranch
        ///</summary>
        [TestMethod(),Ignore]
        public void CreateBranchTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.81.58:8080/tfs/TfsExp"); 
            CTfsVersionControl target = new CTfsVersionControl(ctpc); 
            string sourcePath = "$/Sample/mainline/UAT/"; 
            string targetPath = "$/Sample/branches/test1";
            VersionSpec version = VersionSpec.Latest; 
            int number = target.CreateBranch(sourcePath, targetPath, version);
            Assert.IsTrue(number > 0);
        }

        /// <summary>
        ///A test for GetWorkSpace
        ///</summary>
        [TestMethod(),Ignore]
        public void GetWorkSpaceTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System"); 
            CTfsVersionControl target = new CTfsVersionControl(ctpc); 
            string localPath = @"E:\System\scm\MainLine\Code"; 
            CTfsWorkSpace expected = null; 
            CTfsWorkSpace actual;
            actual = target.GetWorkSpace(localPath);
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsBranch
        ///</summary>
        [TestMethod()]
        public void IsBranchTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System");
            CTfsVersionControl target = new CTfsVersionControl(ctpc); 
            string serverPath = "$/scm/MainLine/Code/"; 
            string serverPath1 = @"$/Automation/Branch/3_Ctrip.AM.JOB_TaoBao";
            bool expected = true; 
            bool actual;
            actual = target.IsBranch(serverPath);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(target.IsBranch(serverPath1), false);
        }

        /// <summary>
        ///A test for SetInherit
        ///</summary>
        [TestMethod()]
        public void SetInheritTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System");
            CTfsVersionControl target = new CTfsVersionControl(ctpc);
            string filepath = "$/scm/MainLine/Code/"; 
            bool inherit = true; 
            target.SetInherit(filepath, inherit);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetInherit
        ///</summary>
        [TestMethod()]
        public void SetInheritTest1()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System");
            CTfsVersionControl target = new CTfsVersionControl(ctpc);
            string filepath = "$/scm/MainLine/Code/";
            bool inherit = false;
            target.SetInherit(filepath, inherit);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetFilePermission
        ///</summary>
        [TestMethod()]
        public void SetFilePermissionTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System");
            string[] permission = null; // TODO: Initialize to an appropriate value
            int flag = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual = false;
           // actual = target.SetFilePermission(filepath, userGroup, permission, flag);
            Assert.AreEqual(expected, actual);
            CTfsVersionControl target = new CTfsVersionControl(ctpc);
            string filepath = string.Empty; // TODO: Initialize to an appropriate value
            string userGroup = string.Empty; // TODO: Initialize to an appropriate value
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateLabel
        ///</summary>
        [TestMethod()]
        public void CreateLabelTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System");
            CTfsVersionControl target = new CTfsVersionControl(ctpc);
            string labelName = "testLabel";
            string[] itemSpecArray = { "$/scm/MainLine/Code/SetupWebsite", "$/scm/MainLine/Code/Permission_bat" };
            string labelComment = "testLabelComment";
            bool expected = true;
            bool actual;
            actual = target.CreateLabel(labelName, itemSpecArray, labelComment);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetItem
        ///</summary>
        [TestMethod()]
        public void GetItemTest()
        {
            CTfsTeamProjectCollection ctpc = new CTfsTeamProjectCollection("http://192.168.83.70:8080/tfs/System");
            CTfsVersionControl target = new CTfsVersionControl(ctpc);
            string serverPath = "$/scm/MainLine/Code/fdaseq"; 
            string expected = serverPath; 
            Item actual;
            try
            {
                actual = target.GetItem(serverPath);
                Assert.AreEqual(expected, actual.ServerItem);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.GetType());
            }
            
        }
    }
}
