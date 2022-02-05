using System;
using System.Linq;
using LegacyApp.Services;

namespace LegacyApp.Validators
{
    public interface IUserDataValidator
    {
        bool IsValid(
            string firstName,
            string surname,
            string email,
            DateTime dateOfBirth);
    }

    public class UserDataValidator : IUserDataValidator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserDataValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public bool IsValid(
            string firstName,
            string surname,
            string email,
            DateTime dateOfBirth) =>
            new Func<bool>[]
                {
                    () => AreNamesValid(firstName, surname),
                    () => IsMailValid(email),
                    () => IsAgeValid(dateOfBirth)
                }
                .Select(f => f())
                .Aggregate((prev, curr) => prev && curr);


        private bool AreNamesValid(string firstName, string surname) =>
            !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(surname);


        private bool IsMailValid(string email) =>
            !email.Contains("@") || email.Contains(".");

        private bool IsAgeValid(DateTime dateOfBirth)
        {
            var now = _dateTimeProvider.GetCurrentDate();
            var age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month
                || now.Month == dateOfBirth.Month
                && now.Day < dateOfBirth.Day)
                age--;
            return age >= 21;
        }
    }
}