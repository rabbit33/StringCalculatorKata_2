using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StringCalculatorTests
{
    [TestClass]
    public class UnitTest1
    {
        private StringCalculator calculator;

        [TestInitialize]
        public void Setup()
        {
            calculator = new StringCalculator();
        }

        [TestMethod]
        public void Add_EmptyString_ReturnZero()
        {
            int result = calculator.Add(string.Empty);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Add_OneNumber_ReturnsTheNumber()
        {
            int result = calculator.Add("1");

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Add_TwoNumbers_ReturnsSum()
        {
            int result = calculator.Add("1,2");

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void Add_MultipleNumbers_ReturnsSum()
        {
            int result = calculator.Add("1,2,3,4,5");

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Add_DelimiterIsNewLine_ReturnsSum()
        {
            int result = calculator.Add("1\n2,3");

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void Add_CustomDelimitersIsProvided_ReturnsSum()
        {
            int result = calculator.Add("//;\n1;2");

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_NegativeNumbers_ThrowsException()
        {
            calculator.Add("1,-2");
        }

        [TestMethod]
        public void Add_NegativeNumbers_MessageIsCorrect()
        {
            try
            {
                calculator.Add("1,-2");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Negative numbers: -2",e.Message);
            }
        }

        [TestMethod]
        public void Add_NumbersGreaterThan1000_ReturnsCorrectSum()
        {
            int result = calculator.Add("1\n2,3000");

            Assert.AreEqual(3, result);
        }
    }

    public class StringCalculator
    {
        public int Add(string numbers)
        {
            var stringSplitter = new StringSplitter(numbers);
            var intNumbers = stringSplitter.ExtractNumbers();

            ValidateNumbers(intNumbers);

            return intNumbers.Where(n => n <= 1000).Sum();
        }

        private static void ValidateNumbers(IEnumerable<int> intNumbers)
        {
            if (intNumbers.Any(n => n < 0))
            {
                throw new ArgumentException("Negative numbers: " + string.Join(",", intNumbers.Where(n => n < 0)));
            }
        }
    }

    public class StringSplitter
    {
        private readonly List<char> delimiters;
        private string numbers;

        public StringSplitter(string numbers)
        {
            this.numbers = numbers;
            delimiters = new List<char> { ',', '\n' };
        }

        public IEnumerable<int> ExtractNumbers()
        {
            if (numbers == string.Empty)
                return Enumerable.Empty<int>();

            var stringNumbers = SplitString();

            return stringNumbers.Select(int.Parse);
        }

        private IEnumerable<string> SplitString()
        {
            if (ContainsCustomDelimiter())
            {
                char customDelimiter = ExtractCustomDelimiter();
                delimiters.Add(customDelimiter);

                RemoveCustomDelimiterLine();
            }

            return numbers.Split(delimiters.ToArray());
        }

        private void RemoveCustomDelimiterLine()
        {
            numbers = numbers.Substring(4);
        }

        private char ExtractCustomDelimiter()
        {
            return numbers[2];
        }

        private bool ContainsCustomDelimiter()
        {
            return numbers.StartsWith("//");
        }
    }
}
