namespace BankApplication.Managers.AccountManager
{
    public class AccountFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public AccountFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IAccount CreateAccount(string accType)
        {
            return accType switch
            {
                "saving" => _serviceProvider.GetRequiredService<SavingAccount>(),
                "current" => _serviceProvider.GetRequiredService<CurrentAccount>(),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
