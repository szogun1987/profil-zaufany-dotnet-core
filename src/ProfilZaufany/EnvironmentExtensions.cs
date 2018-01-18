using System;

namespace ProfilZaufany
{
    internal static class EnvironmentExtensions
    {
        private static readonly Uri ProductionUri;

        private static readonly Uri TestUri;

        static EnvironmentExtensions()
        {
            ProductionUri = new Uri("https://pz.gov.pl");
            TestUri = new Uri("https://int.pz.gov.pl");
        }

        public static Uri GetServiceUri(this Environment environment, string serviceAddress)
        {
            switch (environment)
            {
                case Environment.Production:
                    return new Uri(ProductionUri, serviceAddress);
                case Environment.Test:
                    return new Uri(TestUri, serviceAddress);
                default:
                    throw new ArgumentOutOfRangeException(nameof(environment), environment, null);
            }
        }
    }
}