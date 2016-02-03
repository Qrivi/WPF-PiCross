using System;
using System.IO;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class ArchiverTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dummy = new DummyData();
            var archiver = new Archiver();
            var gameData = new GameData(dummy.Puzzles, dummy.Players);


            using ( var stream = File.Open(@"g:\test.zip", FileMode.Create) )
            {
                archiver.Write( gameData, stream );
            }
        }
    }
}
