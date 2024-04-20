namespace BankApplication.Managers.AccountManager
{
    public class CurrentAccount : IAccount
    {
        public double interestRate = 1.2;
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
