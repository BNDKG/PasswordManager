using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassWordManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassWordManager.Tests
{
    [TestClass()]
    public class aes_logicTests
    {
        [TestMethod()]
        public void AesEncryptTest()
        {
            string aaa = aes_logic.AesEncrypt("111", "12345678876543211234567887654abc");
            string bbb = "oVLeERBcMsDw05xboR27Sw==";


            Assert.IsTrue( aaa==bbb);
            /*
            string aaa2 = aes_logic.AesEncrypt("1112", "12345678876543211234567887654abc");
            string bbb2 = "oVLeERBcMsDw05xboR27Sw==";


            Assert.IsTrue(aaa2 == bbb2);
            */
            //Assert.Fail();
        }

        [TestMethod()]
        public void AesDecryptTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SavePasswordTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SavePasswordPureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void _SavePasswordPureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadPasswordPureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void _LoadPasswordPureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadPasswordTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PasswordCreateTest()
        {
            Assert.Fail();
        }
    }
}