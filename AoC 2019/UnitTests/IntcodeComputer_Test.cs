using NUnit.Framework;

namespace Tests
{
    using Intcode_Computer;

    public class IntcodeComputer_Test
    {
        private const string OutOfRangeErrorMessage = @"ERROR: \w+ pointer value \w+ is larger than input length \w+!\r\nParameter name: \w+ pointer";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ComputeIntcode_WithValidStringInput_ReturnCorrectAnswer()
        {
            // Arrange
            string input = "1,1,1,4,99,5,6,0,99";
            string expected = "30,1,1,4,2,5,6,0,99";
            IntcodeComputer IC = new IntcodeComputer();

            // Act
            string output = IC.ComputeIntcode(input);

            // Assert
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void ComputeIntcode_WithValidIntInput_ReturnCorrectAnswer()
        {
            // Arrange
            int[] input = new int[] { 2, 4, 4, 5, 99, 0 };
            int[] expected = new int[] { 2, 4, 4, 5, 99, 9801 };
            IntcodeComputer IC = new IntcodeComputer();

            // Act
            int[] output = IC.ComputeIntcode(input);

            // Assert
            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void ComputeIntcode_WithInvalidIntInput_ShouldThrowArgumentOutOfRange()
        {
            // Arrange
            int[] input = new int[] { 2, 99, 4, 5, 99, 0 };
            IntcodeComputer IC = new IntcodeComputer();

            // Act
            try
            {
                int[] output = IC.ComputeIntcode(input);
            }
            catch(System.ArgumentOutOfRangeException e)
            {
                // Assert
                Assert.That(e.Message, Does.Match(OutOfRangeErrorMessage));
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }
    }
}