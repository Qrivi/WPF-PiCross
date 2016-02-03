using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Facade.IO;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class PlayerDatabaseTests
    {
        [TestMethod]
        [TestCategory("PlayerDatabase")]
        public void IndexingYieldsSameObjectAsAdding()
        {
            var name = "mathy";
            var pdb = new PlayerDatabase();

            var profile = pdb.CreateNewProfile( name );

            Assert.AreSame( profile, pdb[name] );
        }

        [TestMethod]
        [TestCategory( "PlayerDatabase" )]
        public void AddingAddsToNameCollection()
        {
            var name = "mathy";
            var pdb = new PlayerDatabase();

            pdb.CreateNewProfile( name );

            pdb.PlayerNames.Contains( name );
        }

        [TestMethod]
        [TestCategory( "PlayerDatabase" )]
        public void NamesAreSorted()
        {            
            var pdb = new PlayerDatabase();
                        
            pdb.CreateNewProfile( "b" );
            pdb.CreateNewProfile( "a" );
            pdb.CreateNewProfile( "c" );

            Assert.AreEqual( 3, pdb.PlayerNames.Count );
            Assert.AreEqual( "a", pdb.PlayerNames[0] );
            Assert.AreEqual( "b", pdb.PlayerNames[1] );
            Assert.AreEqual( "c", pdb.PlayerNames[2] );
        }

        [TestMethod]
        [TestCategory( "PlayerDatabase" )]
        public void RemovingRemovesName()
        {
            var name = "mathy";
            var pdb = new PlayerDatabase();

            pdb.CreateNewProfile( name );

            Assert.IsTrue( pdb.PlayerNames.Contains( name ) );
            pdb.DeleteProfile( name );
            Assert.IsFalse( pdb.PlayerNames.Contains( name ) );
        }
    }
}
