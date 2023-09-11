using Branas.Services;

namespace Branas.Test
{
    public class CpfValidatorTest
    {
        [Theory]
        [InlineData("95818705552")]
        [InlineData("01234567890")]
        [InlineData("565.486.780-60")]
        [InlineData("147.864.110-00")]
        public void Quando_CpfValido_Deve_Validar(string cpf)
        {
            CpfValidator cpfValidator = new();

            Assert.True(cpfValidator.Validate(cpf));
        }

        [Theory]
        [InlineData("958.187.055-00")]
        [InlineData("958.187.055")]
        public void Quando_CpfInvalido_NaoDeve_Validar(string cpf)
        {
            CpfValidator cpfValidator = new();

            Assert.False(cpfValidator.Validate(cpf));
        }
    }
}
