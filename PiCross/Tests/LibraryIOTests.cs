using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Facade.IO;

namespace PiCross.Tests
{
    [TestClass]
    public class LibraryIOTests : TestBase
    {
        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved1()
        {
            var library = CreateLibrary(
                CreatePuzzle( "." )
                );

            CheckIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved2()
        {
            var library = CreateLibrary(
                CreatePuzzle( "x" )
                );

            CheckIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved3()
        {
            var library = CreateLibrary(
                CreatePuzzle( ".." )
                );

            CheckIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved4()
        {
            var library = CreateLibrary(
                CreatePuzzle( "xx" )
                );

            CheckIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved5()
        {
            var library = CreateLibrary(
                CreatePuzzle( 
                "xx",
                "xx" )
                );

            CheckIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved6()
        {
            var library = CreateLibrary(
                CreatePuzzle(
                "x....x..x",
                "x.xxx.x..",
                ".x.xx.x..",
                "x...x..xx" )
                );

            CheckIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void MemoryStreamAssumptionCheck()
        {
            var str = "Hello world";
            using ( var stream = new MemoryStream() )
            {
                var writer = new StreamWriter( stream );
                writer.WriteLine( str );
                writer.Flush();

                stream.Seek( 0, SeekOrigin.Begin );

                var reader = new StreamReader( stream );
                var line = reader.ReadLine();

                Assert.AreEqual( str, line );
            }
        }

        private void CheckIO( ILibrary library )
        {
            var io = new LibraryIO( new TextFormat() );

            using ( var memoryStream = new MemoryStream() )
            {
                io.Save( library, memoryStream );
                memoryStream.Seek( 0, SeekOrigin.Begin );
                var loaded = io.Load( memoryStream );

                Assert.AreEqual( library.Entries.Count, loaded.Entries.Count );

                for ( var i = 0; i != library.Entries.Count; ++i )
                {
                    var expected = library.Entries[i];
                    var actual = loaded.Entries[i];

                    Assert.AreEqual( expected, actual, string.Format( "Puzzle #{0} not saved/loaded correctly", i ) );
                }
            }
        }
    }
}
