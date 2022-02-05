using System;
using LegacyApp.Services;

namespace LegacyApp.Strategy
{
    public class CasualClientCreditProvider : IClientCreditProvider
    {
        private readonly IUserCreditService _userCreditService;

        public CasualClientCreditProvider(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public (bool hasCreditLimit, int creditLimit) GetCreditLimit(
            string firstName,
            string surname,
            DateTime dateOfBirth)
        {
            var creditLimit = _userCreditService.GetCreditLimit(
                firstName,
                surname,
                dateOfBirth);
            return (true, creditLimit);
        }
    }
}