using System;

namespace LegacyApp.Strategy
{
    public interface IClientCreditProvider
    {
        (bool hasCreditLimit, int creditLimit) GetCreditLimit(
            string firstName,
            string surname,
            DateTime dateOfBirth);
    }
}