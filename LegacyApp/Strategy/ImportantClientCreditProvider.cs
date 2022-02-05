using System;
using LegacyApp.Services;

namespace LegacyApp.Strategy
{
    public class ImportantClientCreditProvider : IClientCreditProvider
    {
        private readonly IUserCreditService _userCreditService;

        public ImportantClientCreditProvider(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public (bool hasCreditLimit, int creditLimit) GetCreditLimit(
            string firstName,
            string surname,
            DateTime dateOfBirth)
        {
            var creditLimit = 2 * _userCreditService.GetCreditLimit(
                firstName,
                surname,
                dateOfBirth);
            return (true, creditLimit);
        }
    }
}