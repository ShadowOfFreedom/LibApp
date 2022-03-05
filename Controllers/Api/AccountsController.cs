using LibApp.Dtos;
using LibApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibApp.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase {
        private readonly IAccountRepository _accountRepository;
        public AccountsController(IAccountRepository accountRepository) => 
            _accountRepository = accountRepository;

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDTO userDto) {
            _accountRepository.RegisterUser(userDto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginUserDTO loginDto) {
            var token = _accountRepository.GenerateJwt(loginDto);
            return Ok(token);
        }
    }
}
