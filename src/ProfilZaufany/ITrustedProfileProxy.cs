using ProfilZaufany.SigningForm;

namespace ProfilZaufany
{
    public interface ITrustedProfileProxy
    {
        ISigningForm SigningForm { get; }
    }
}