using System;

namespace ProfilZaufany
{
    internal static class EnvironmentExtensions
    {
        private static readonly Uri _productionUri;

        private static readonly Uri _testUri;

        static EnvironmentExtensions()
        {
            _productionUri = new Uri("https://pz.gov.pl");
            _testUri = new Uri("https://int.pz.gov.pl");
        }

        public static Uri ToUri(this Environment environment)
        {
            switch (environment)
            {
                case Environment.Production:
                    return _productionUri;
                case Environment.Test:
                    return _testUri;
                default:
                    throw new ArgumentOutOfRangeException(nameof(environment), environment, null);
            }
        }
    }
}