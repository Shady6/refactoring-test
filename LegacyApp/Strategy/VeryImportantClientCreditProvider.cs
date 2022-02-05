using System;

namespace LegacyApp.Strategy
{
    public class VeryImportantClientCreditProvider : IClientCreditProvider
    {
        public (bool hasCreditLimit, int creditLimit) GetCreditLimit(
            string firstName,
            string surname,
            DateTime dateOfBirth) =>
            (false, 0);
    }
}