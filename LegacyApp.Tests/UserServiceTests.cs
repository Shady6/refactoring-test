using System;
using FluentAssertions;
using LegacyApp.DataAccess;
using LegacyApp.DataAccess.Repositories;
using LegacyApp.Factory;
using LegacyApp.Models;
using LegacyApp.Services;
using LegacyApp.Validators;
using NSubstitute;
using Xunit;

namespace LegacyApp.Tests;

public class UserServiceTests
{
    private readonly IClientRepository _clientRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UserService _sut;
    private readonly IUserCreditService _userCreditService;
    private readonly IUserDataAccess _userDataAccess;

    private readonly User _validUser;

    public UserServiceTests()
    {
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _clientRepository = Substitute.For<IClientRepository>();
        _userCreditService = Substitute.For<IUserCreditService>();
        _userDataAccess = Substitute.For<IUserDataAccess>();

        _dateTimeProvider
            .GetCurrentDate()
            .Returns(new DateTime(2022, 01, 01));

        _validUser = new User
        {
            Firstname = "John",
            Surname = "Doe",
            EmailAddress = "John.Doe",
            DateOfBirth = _dateTimeProvider.GetCurrentDate().AddYears(-25),
            Client = new Client
            {
                Id = 1
            }
        };

        _sut = new UserService(
            _clientRepository,
            _userDataAccess,
            new UserDataValidator(
                _dateTimeProvider),
            new ClientCreditCreditProviderFactory(
                _userCreditService),
            new CreditValidator());
    }

    [Theory]
    [InlineData("", "asd")]
    [InlineData(null, "asd")]
    [InlineData("asd", "")]
    [InlineData("asd", null)]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void AddUser_ReturnsFalse_WhenFirstNameOrSurnameInvalid(
        string firstName,
        string surname)
    {
        _sut.AddUser(
                firstName,
                surname,
                _validUser.EmailAddress,
                _validUser.DateOfBirth,
                _validUser.Client.Id)
            .Should()
            .Be(false);
    }

    [Fact]
    public void AddUser_ReturnsFalse_WhenEmailInvalid()
    {
        _sut.AddUser(
                _validUser.Firstname,
                _validUser.Surname,
                "@asd",
                _validUser.DateOfBirth,
                _validUser.Client.Id)
            .Should()
            .Be(false);
    }

    [Fact]
    public void AddUser_ReturnsFalse_WhenAgeInvalid()
    {
        _sut.AddUser(
                _validUser.Firstname,
                _validUser.Surname,
                _validUser.EmailAddress,
                _dateTimeProvider.GetCurrentDate().AddYears(-20),
                _validUser.Client.Id)
            .Should()
            .Be(false);
    }

    [Fact]
    public void AddUser_SetsHasCreditLimitToFalse_WhenClientIsVeryImportant()
    {
        _validUser.Client.Name = "VeryImportantClient";
        _clientRepository
            .GetById(_validUser.Client.Id)
            .Returns(_validUser.Client);

        SutAddValidUser().Should().Be(true);

        _userDataAccess
            .Received(1)
            .AddUser(Arg.Is<User>(x =>
                _validUser.Client.Id == x.Client.Id
                && _validUser.HasCreditLimit == false));
    }

    [Theory]
    [InlineData("ImportantClient")]
    [InlineData("FooClient")]
    public void AddUser_ReturnsFalse_WhenClientCreditLimitTooLow(string clientName)
    {
        var importantUserCreditLimit = 200;
        _validUser.Client.Name = clientName;
        _clientRepository
            .GetById(_validUser.Client.Id)
            .Returns(_validUser.Client);
        _userCreditService.GetCreditLimit(
                _validUser.Firstname,
                _validUser.Surname,
                _validUser.DateOfBirth)
            .Returns(importantUserCreditLimit);

        SutAddValidUser().Should().Be(false);
    }

    [Fact]
    public void AddUser_DoublesCreditLimit_WhenClientImportant()
    {
        var importantUserCreditLimit = 500;
        _validUser.Client.Name = "ImportantClient";
        _clientRepository
            .GetById(_validUser.Client.Id)
            .Returns(_validUser.Client);
        _userCreditService.GetCreditLimit(
                _validUser.Firstname,
                _validUser.Surname,
                _validUser.DateOfBirth)
            .Returns(importantUserCreditLimit);

        SutAddValidUser().Should().Be(true);

        _userDataAccess
            .Received(1)
            .AddUser(Arg.Is<User>(x =>
                _validUser.Client.Id == x.Client.Id
                && x.HasCreditLimit == true
                && x.CreditLimit == 2 * importantUserCreditLimit));
    }

    [Fact]
    public void AddUser_ReturnsTrue_WhenClientNormalWithEnoughCreditLimit()
    {
        var importantUserCreditLimit = 500;
        _validUser.Client.Name = "FooBarskyClient";
        _clientRepository
            .GetById(_validUser.Client.Id)
            .Returns(_validUser.Client);
        _userCreditService.GetCreditLimit(
                _validUser.Firstname,
                _validUser.Surname,
                _validUser.DateOfBirth)
            .Returns(importantUserCreditLimit);

        SutAddValidUser().Should().Be(true);

        _userDataAccess
            .Received(1)
            .AddUser(Arg.Is<User>(x =>
                _validUser.Client.Id == x.Client.Id
                && x.HasCreditLimit == true
                && x.CreditLimit == importantUserCreditLimit));
    }

    private bool SutAddValidUser() =>
        _sut.AddUser(
            _validUser.Firstname,
            _validUser.Surname,
            _validUser.EmailAddress,
            _validUser.DateOfBirth,
            _validUser.Client.Id);
}