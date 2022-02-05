using System;
using LegacyApp.DataAccess;
using LegacyApp.DataAccess.Repositories;
using LegacyApp.Factory;
using LegacyApp.Models;
using LegacyApp.Services;
using LegacyApp.Validators;

namespace LegacyApp
{
    public class UserService
    {
        private readonly IClientCreditProviderFactory _clientCreditCreditProviderFactory;
        private readonly IClientRepository _clientRepository;
        private readonly ICreditValidator _creditValidator;
        private readonly IUserDataAccess _userDataAccess;
        private readonly IUserDataValidator _userDataValidator;

        public UserService(
            IClientRepository clientRepository,
            IUserDataAccess userDataAccess,
            IUserDataValidator userDataValidator,
            IClientCreditProviderFactory clientCreditCreditProviderFactory,
            ICreditValidator creditValidator)
        {
            _clientRepository = clientRepository;
            _userDataAccess = userDataAccess;
            _userDataValidator = userDataValidator;
            _clientCreditCreditProviderFactory = clientCreditCreditProviderFactory;
            _creditValidator = creditValidator;
        }

        public UserService() : this(
            new ClientRepository(),
            new UserDataAccessProxy(),
            new UserDataValidator(
                new DateTimeProvider()),
            new ClientCreditCreditProviderFactory(
                new UserCreditServiceClient()),
            new CreditValidator())
        {
        }

        public bool AddUser(
            string firstName,
            string surname,
            string email,
            DateTime dateOfBirth,
            int clientId)
        {
            if (!_userDataValidator.IsValid(
                    firstName,
                    surname,
                    email,
                    dateOfBirth))
                return false;

            var user = new User
            {
                Client = _clientRepository.GetById(clientId),
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firstName,
                Surname = surname
            };

            SetUserCreditLimit(firstName, surname, dateOfBirth, user);

            if (!_creditValidator.IsValid(user.HasCreditLimit, user.CreditLimit))
                return false;

            _userDataAccess.AddUser(user);
            return true;
        }

        private void SetUserCreditLimit(string firstName, string surname, DateTime dateOfBirth, User user)
        {
            (user.HasCreditLimit, user.CreditLimit) =
                _clientCreditCreditProviderFactory
                    .GetClientCreditProvider(user.Client.Name)
                    .GetCreditLimit(firstName, surname, dateOfBirth);
        }
    }
}