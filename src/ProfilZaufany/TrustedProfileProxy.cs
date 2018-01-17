using ProfilZaufany.SigningForm;

namespace ProfilZaufany
{
    public class TrustedProfileProxy : ITrustedProfileProxy
    {
        private readonly TrustedProfileSettings _profileSettings;

        public TrustedProfileProxy(TrustedProfileSettings profileSettings)
        {
            _profileSettings = profileSettings;

            SigningForm = new SigningForm.SigningForm(_profileSettings);
        }

        public ISigningForm SigningForm { get; }
    }
}