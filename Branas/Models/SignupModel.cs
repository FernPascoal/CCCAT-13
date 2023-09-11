namespace Branas.Models
{
    public class SignupModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string CarPlate { get; set; }
        public bool IsPassenger { get; set; }
        public bool IsDriver { get; set; }

        public SignupModel(string name, string email, string cpf, bool isPassenger, bool isDriver, string carPlate)
        {
            Name = name;
            Email = email;
            CPF = cpf;
            IsPassenger = isPassenger;
            IsDriver = isDriver;
            CarPlate = carPlate;
        }
    }
}
