using System.Threading.Tasks;

namespace ProfilZaufany.SigningForm
{
    public interface ISigningForm
    {
        Task<SigningFormModel> BuildFormModel(SigningFormBuildingArguments buildingArguments);
    }
}