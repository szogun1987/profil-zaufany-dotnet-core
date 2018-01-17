namespace ProfilZaufany.SigningForm
{
    internal class SigningForm : ISigningForm
    {
        private readonly TrustedProfileSettings _profileSettings;

        public SigningForm(TrustedProfileSettings profileSettings)
        {
            _profileSettings = profileSettings;
        }

        public SigningFormModel BuildFormModel(SigningFormBuildingArguments buildingArguments)
        {
            throw new System.NotImplementedException();
        }
    }
}