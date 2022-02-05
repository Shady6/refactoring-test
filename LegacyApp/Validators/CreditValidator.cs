namespace LegacyApp.Validators
{
    public interface ICreditValidator
    {
        bool IsValid(bool hasCreditLimit, int creditLimit);
    }

    public class CreditValidator : ICreditValidator
    {
        public bool IsValid(bool hasCreditLimit, int creditLimit) =>
            !hasCreditLimit || creditLimit >= 500;
    }
}