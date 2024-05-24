using Domain.Dto;

namespace Domain.Interface
{
    public interface IUser
    {
        Task<RegistrationResponse> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<LoginResponse> LoginUserAsync(LoginDto loginDto);
    }
}
