namespace ProfilZaufany.SigningForm
{
    public interface ISigningForm
    {
        SigningFormModel BuildFormModel(SigningFormBuildingArguments buildingArguments);
    }
}