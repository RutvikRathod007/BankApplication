namespace BankApplication.Managers.AccountManager
{
    public interface IAccount
    {
        double CalculateInterest(double amount);
        double GetInterest();
    }
}
