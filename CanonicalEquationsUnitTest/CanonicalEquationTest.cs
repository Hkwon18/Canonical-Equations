using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Canonical_Equations;
using Canonical_Equations.Classes;

namespace CanonicalEquationsUnitTest
{
    [TestClass]
    public class CanonicalEquationTest
    {
        [TestMethod]
        public void Equation1()
        {
            string equation1 = "x ^ 2 + 3.5xy + y = y ^ 2 - xy + y";
            string answer1 = "x^2 - y^2 + 4.5xy = 0";
            Canonizer canonizer = new Canonizer();
            Assert.AreEqual(canonizer.ParseEquation(equation1), answer1);
        }

        [TestMethod]
        public void Equation2()
        {
            string equation2 = "x = 1";
            string answer2 = "x - 1 = 0";
            Canonizer canonizer = new Canonizer();
            Assert.AreEqual(canonizer.ParseEquation(equation2), answer2);
        }

        [TestMethod]
        public void Equation3()
        {
            string equation3 = "x - (y^2 - x) = 0";
            string answer3 = "2x - y^2 = 0";
            Canonizer canonizer = new Canonizer();
            Assert.AreEqual(canonizer.ParseEquation(equation3), answer3);
        }
        [TestMethod]
        public void Equation4()
        {
            string equation4 = "x - (0 - (0 - x)) = 0";
            string answer4 = "0 = 0";
            Canonizer canonizer = new Canonizer();
            Assert.AreEqual(canonizer.ParseEquation(equation4), answer4);
        }

        [TestMethod]
        public void VariableOrder()
        {
            string equation4 = "1 + x - y + xx - yy + xy - x^2 + y^2 = 0";
            string answer4 = "-x^2 + x + y^2 - y + xx + xy - yy + 1 = 0";
            Canonizer canonizer = new Canonizer();
            Assert.AreEqual(canonizer.ParseEquation(equation4), answer4);
        }
    }
}
