using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WalletWise.Model.DTO.User;
using WalletWise.Model.User;
using WalletWise.Repository;
using WalletWiseApi.Services;

namespace WalletWiseApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration? _config;
        private readonly ILogger<UserController> _logger;
        private readonly UserRepository? _userRepository;
        private string? _connString;

        public UserController(IConfiguration? config,
                              ILogger<UserController> logger)
        {
            _config = config;
            _logger = logger;
            _connString = _config?.GetConnectionString(Constants.WALLET_WISE_CONN_STRING);

            if (string.IsNullOrWhiteSpace(_connString))
                throw new ArgumentNullException(nameof(_connString));
            
            _userRepository = new UserRepository(_connString);
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<UserRegisterRespDto>))]
        public async Task<ActionResult<UserRegisterRespDto>> Register(UserRegisterReqDto data)
        {
            if (data is null)
            {
                _logger.LogError($"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: data is null");
                return BadRequest();
            }

            if (data.Email is null || String.IsNullOrEmpty(data.Email) || data.Password is null || String.IsNullOrEmpty(data.Password))
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: invalid information";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new UserRegisterRespDto();

            try 
            {
                var tmp = await _userRepository.GetUserByUsername(data.Username);

                if (tmp != null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: There is another user with username: {data.Username}";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                User user = new User()
                {
                    Username = data.Username,
                    Email = data.Email,
                    PasswordSalt = PasswordHasher.GenerateSalt()
                };

                user.Password = PasswordHasher.ComputeHash(data.Password, user.PasswordSalt, Constants.PEPPER, Constants.ITERATION);

                var userId = await _userRepository.InsertAsync(user);

                if (userId <= 0)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: Something wrong during user registration";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsRegisterSuccess = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }
    }
}
