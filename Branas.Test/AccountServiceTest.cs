using Branas.Models;
using Branas.Services;

namespace Branas.Test
{
    public class AccountServiceTest
    {
        [Fact()]
        public async Task Quando_InputOk_Deve_CriarPassageiroAsync()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);

            AccountService accountService = new();
            var output = await accountService.SignupAsync(signup);
            var account = await accountService.GetAccount(output);

            Assert.NotNull(account);
            Assert.NotNull(account.AccountId);
            Assert.Equal(signup.Name, account.Name);
            Assert.Equal(signup.Email, account.Email);
            Assert.Equal(signup.CPF, account.CPF);
        }

        [Fact()]
        public async Task Quando_CpfInvalido_NaoDeve_CriarPassageiroAsync()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "95818705500", true, false, string.Empty);

            AccountService accountService = new();

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.SignupAsync(signup));
            Assert.Equal("Invalid cpf", ex.Message);
        }

        [Fact()]
        public async Task Quando_NomeInvalido_NaoDeve_CriarPassageiroAsync()
        {
            SignupModel signup = new("John", $"john.doe{new Random().Next()}@gmail.com", "95818705500", true, false, string.Empty);

            AccountService accountService = new();

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.SignupAsync(signup));
            Assert.Equal("Invalid name", ex.Message);
        }

        [Fact()]
        public async Task Quando_EmailInvalido_NaoDeve_CriarPassageiroAsync()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@", "95818705500", true, false, string.Empty);

            AccountService accountService = new();

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.SignupAsync(signup));
            Assert.Equal("Invalid email", ex.Message);
        }

        [Fact()]
        public async Task Quando_ContaExistente_NaoDeve_CriarPassageiroAsync()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);

            AccountService accountService = new();
            await accountService.SignupAsync(signup);

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.SignupAsync(signup));
            Assert.Equal("Account already exists", ex.Message);
        }

        [Fact()]
        public async Task Quando_InputOk_Deve_CriarMotoristaAsync()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", false, true, "IUV7890");

            AccountService accountService = new();
            var output = await accountService.SignupAsync(signup);
            var account = await accountService.GetAccount(output);

            Assert.NotNull(account);
            Assert.NotNull(account.AccountId);
            Assert.Equal(signup.Name, account.Name);
            Assert.Equal(signup.Email, account.Email);
            Assert.Equal(signup.CPF, account.CPF);
            Assert.Equal(signup.CarPlate, account.CarPlate);
            Assert.True(account.IsDriver);
            Assert.False(account.IsPassenger);
        }

        [Fact()]
        public async Task Quando_PlacaInvalida_NaoDeve_CriarMotoristaAsync()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", false, true, string.Empty);

            AccountService accountService = new();

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.SignupAsync(signup));
            Assert.Equal("Invalid plate", ex.Message);
        }
    }
}