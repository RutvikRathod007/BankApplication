namespace BankApplication.Managers.AccountManager
{
    public class SavingAccount : IAccount
    {
        public double interestRate = 3.4;
        public double CalculateInterest(double amount)
        {
            return interestRate / 100 * amount;
        }
        public double GetInterest()
        {
            return interestRate;
        }
    }
}
