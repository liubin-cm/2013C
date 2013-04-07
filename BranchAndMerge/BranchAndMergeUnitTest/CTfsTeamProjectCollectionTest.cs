using BranchAndMerge.lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.Generic;

namespace BranchAndMergeUnitTest
{
    
    
    /// <summary>
    ///This is a test class for CTfsTeamProjectCollectionTest and is intended
    ///to contain all CTfsTeamProjectCollectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTfsTeamProjectCollectionTest
    {

        private string uri; 
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            this.uri = "http://192.168.83.70:8080/tfs/system";
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CTfsTeamProjectCollection Constructor
        ///</summary>
        [TestMethod()]
        public void CTfsTeamProjectCollectionConstructorTest()
        {
            try
            {
                CTfsTeamProjectCollection target = new CTfsTeamProjectCollection("http://192.168.81.58:8080/tfs/TfsExp", "ll", "dfs", "cn2");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Microsoft.TeamFoundation.TeamFoundationServerUnauthorizedException", e.GetType().ToString());
            }
        }

        /// <summary>
        ///A test for tfsTeamProjectCollection
        ///</summary>
        [TestMethod()]
        public void tfsTeamProjectCollectionTest()
        {
            CTfsTeamProjectCollection target = new CTfsTeamProjectCollection(this.uri); 
            TfsTeamProjectCollection actual;
            actual = target.TPC;
            Assert.AreEqual(actual.Uri.AbsoluteUri, "http://192.168.83.70:8080/tfs/system");
        }

        /// <summary>
        ///A test for GetProjects
        ///</summary>
        [TestMethod()]
        public void GetProjectsTest()
        {
            CTfsTeamProjectCollection target = new CTfsTeamProjectCollection(this.uri); 
            int expected = 4; 
            List<TeamProject> actual;
            actual = target.GetProjects();
            foreach (var item in actual)
            {
                Console.WriteLine(item.Name);
            }
            Assert.AreEqual(expected, actual.Count);
        }

        /// <summary>
        ///A test for GetProject
        ///</summary>
        [TestMethod()]
        public void GetProjectTest()
        {
            CTfsTeamProjectCollection target = new CTfsTeamProjectCollection(this.uri); 
            string expected = "Release";
            string expected1 = "hello";
            TeamProject actual;
            actual = target.GetProject(expected);
            Assert.AreEqual(expected, actual.Name);
            Assert.IsNull(target.GetProject(expected1));
        }

        /// <summary>
        ///A test for GetSecurityGroupNames
        ///</summary>
        [TestMethod()]
        public void GetSecurityGroupNamesTest()
        {
            CTfsTeamProjectCollection target = new CTfsTeamProjectCollection(this.uri); 
            List<string> actual;
            actual = target.GetSecurityGroupNames();
            foreach (var item in actual)
            {
                Console.WriteLine(item.ToString());
            }
            Assert.IsTrue(actual.Contains(@"[System]\ReadAll"));
            Assert.IsFalse(actual.Contains("hehe"));
        }

        /// <summary>
        ///A test for GetMembersofGroup
        ///</summary>
        [TestMethod()]
        public void GetMembersofGroupTest()
        {
            CTfsTeamProjectCollection target = new CTfsTeamProjectCollection(this.uri); // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = target.GetMembersofGroup("TRDC");
            foreach (var item in actual)
            {
                Console.WriteLine(item.ToString());
            }
            Assert.IsTrue(actual.Contains("app_code"));
            Assert.IsFalse(actual.Contains("jihl"));
        }

        /// <summary>
        ///A test for AddMembersToGroup
        ///</summary>
        [TestMethod()]
        public void AddMembersToGroupTest()
        {
            CTfsTeamProjectCollection target = new CTfsTeamProjectCollection(this.uri); 
            string groupName = "ReadAll";
            string[] memberNames = new string[] { "lbin" };
            List<string> members = target.GetMembersofGroup(groupName);
            if (!members.Contains("vlb刘斌"))
            {
                target.AddMembersToGroup(groupName, memberNames);
            }
            Assert.IsTrue(target.GetMembersofGroup(groupName).Contains("vlb刘斌"));
            target.DeleteMembersToGroup(groupName, memberNames);
            Assert.IsFalse(target.GetMembersofGroup(groupName).Contains("vlb刘斌"));
        }
    }
}
