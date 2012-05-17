using FilterPipeLines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestLearnFromPure
{
    
    
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProgramTest
    {


        private TestContext testContextInstance;
        // ID is key
        //private Dictionary<string, string> methodsMap; // = new Dictionary<string, string>();
        //private Dictionary<string, string> clusterMap; // = new Dictionary<string, int>();
        //private string ifhfileID;

        public ProgramTest()
        {
            
        }
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
        ///A test for learnFromPure
        ///</summary>
        [TestMethod()]
        public void learnFromPureTest()
        {
            var methodsMap = new Dictionary<string, string>();
            var clusterMap = new Dictionary<string, int>();

            string ifhfileID = @"D:\Logs\ifhFileWithID.txt";
            Program.BuildMethodsMap(methodsMap, ifhfileID);
            Program.learnSameMethodIds(methodsMap, clusterMap);
            Program.splitIntoPureAndMix(methodsMap);

            Program.learnFromPure(methodsMap, clusterMap);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
