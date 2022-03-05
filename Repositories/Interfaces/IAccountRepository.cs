using LibApp.Dtos;

namespace LibApp.Repositories.Interfaces {
    public interface IAccountRepository {
        void RegisterUser(RegisterUserDTO userDto);
        string GenerateJwt(LoginUserDTO loginDto);
    }
}
