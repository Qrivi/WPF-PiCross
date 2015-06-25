using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Dynamic;

namespace PiCross.Tests
{
    [TestClass]
    public class DynamicTests
    {
        [TestMethod]
        [TestCategory( "DynamicObjectGroup" )]
        public void GroupProperties1()
        {
            var dyn1 = new DDO() { { "x", 1 } };
            var group = DynamicObjectGroup<DDO, string>.FromMembers( dyn1 );
            var expected = new HashSet<string>() { "x" };
            var actual = group.Properties;

            AssertSameItems( expected, actual );
        }

        [TestMethod]
        [TestCategory( "DynamicObjectGroup" )]
        public void GroupProperties2()
        {
            var dyn1 = new DDO() { { "x", 1 } };
            var dyn2 = new DDO() { { "y", 1 } };
            var group = DynamicObjectGroup<DDO, string>.FromMembers( dyn1, dyn2 );
            var expected = new HashSet<string>() { "x", "y" };
            var actual = group.Properties;

            AssertSameItems( expected, actual );
        }

        [TestMethod]
        [TestCategory( "DynamicObjectGroup" )]
        public void GroupProperties3()
        {
            var dyn1 = new DDO() { { "x", 1 } };
            var dyn2 = new DDO() { { "y", 1 } };
            var dyn3 = new DDO() { { "x", 2 } };
            var group = DynamicObjectGroup<DDO, string>.FromMembers( dyn1, dyn2, dyn3 );
            var expected = new HashSet<string>() { "x", "y" };
            var actual = group.Properties;

            AssertSameItems( expected, actual );
        }

        [TestMethod]
        [TestCategory( "DynamicObjectGroup" )]
        public void GroupProperties4()
        {
            var dyn1 = new DDO() { { "x", 1 }, { "y", 2 } };
            var dyn2 = new DDO() { { "y", 1 } };
            var dyn3 = new DDO() { { "x", 2 } };
            var group = DynamicObjectGroup<DDO, string>.FromMembers( dyn1, dyn2, dyn3 );
            var expected = new HashSet<string>() { "x", "y" };
            var actual = group.Properties;

            AssertSameItems( expected, actual );
        }

        [TestMethod]
        [TestCategory( "DynamicObjectGroup" )]
        public void PropertyValues1()
        {
            var dyn1 = new DDO() { { "x", 1 } };
            var dyn2 = new DDO() { { "y", 1 } };
            var dyn3 = new DDO() { { "x", 2 } };
            var group = DynamicObjectGroup<DDO, string>.FromMembers( dyn1, dyn2, dyn3 );
            var expected = new HashSet<object>() { 1 };
            var actual = group.PropertyValues( "y" );

            AssertSameItems( expected, actual );
        }

        [TestMethod]
        [TestCategory( "DynamicObjectGroup" )]
        public void PropertyValues2()
        {
            var dyn1 = new DDO() { { "x", 1 } };
            var dyn2 = new DDO() { { "y", 1 } };
            var dyn3 = new DDO() { { "x", 2 } };
            var group = DynamicObjectGroup<DDO, string>.FromMembers( dyn1, dyn2, dyn3 );
            var expected = new HashSet<object>() { 1, 2 };
            var actual = group.PropertyValues( "x" );

            AssertSameItems( expected, actual );
        }

        private void AssertSameItems<T>( ISet<T> expected, ISet<T> actual )
        {
            Assert.AreEqual( expected.Count, actual.Count );

            foreach ( var x in expected )
            {
                Assert.IsTrue( actual.Contains( x ), string.Format( "{0} should be in set", x ) );
            }
        }

        private class DDO : DictionaryDynamicObject<string> { }
    }
}
