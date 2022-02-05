using LegacyApp.Services;
using LegacyApp.Strategy;

namespace LegacyApp.Factory
{
    public static class ClientsNames
    {
        public const string VeryImportantClient = "VeryImportantClient";
        public const string ImportantClient = "ImportantClient";
    }

    public class ClientCreditCreditProviderFactory : IClientCreditProviderFactory
    {
        private readonly IUserCreditService _userCreditService;

        public ClientCreditCreditProviderFactory(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public IClientCreditProvider GetClientCreditProvider(string clientName) =>
            clientName switch
            {
                ClientsNames.VeryImportantClient => new VeryImportantClientCreditProvider(),
                ClientsNames.ImportantClient => new ImportantClientCreditProvider(_userCreditService),
                _ => new CasualClientCreditProvider(_userCreditService)
            };
    }

    public interface IClientCreditProviderFactory
    {
        IClientCreditProvider GetClientCreditProvider(string clientName);
    }
}