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
        private readonly UserRecoveryCredentialRepository _userRecoveryCredentialRepository;
        private string? _connString;

        public UserController(IConfiguration? config,
                              ILogger<UserController> logger)
        {
            _config = config;
            _logger = logger;
            _connString = _config?.GetConnectionString(Constants.WALLET_WISE_CONN_STRING);

            if (string.IsNullOrWhiteSpace(_connString))
                throw new ArgumentNullException(nameof(_connString));
            
            _userRepository                   = new UserRepository(_connString);
            _userRecoveryCredentialRepository = new UserRecoveryCredentialRepository(_connString);
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<RegisterRespDto>))]
        public async Task<ActionResult<RegisterRespDto>> Register(RegisterReqDto data)
        {
            if (data is null)
            {
                _logger.LogError($"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: data is null");
                return BadRequest();
            }

            if (data.Username is null || String.IsNullOrEmpty(data.Username) || data.Password is null || String.IsNullOrEmpty(data.Password))
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: invalid user information";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (data.Question is null || String.IsNullOrEmpty(data.Question) || data.Response is null || String.IsNullOrEmpty(data.Response))
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: invalid user recover information";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _userRecoveryCredentialRepository is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new RegisterRespDto();

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
                    Username     = data.Username,
                    Email        = data.Username,
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

                var userQuestion = await _userRecoveryCredentialRepository.GetUserRecoveryCrdentialByUsername(userId);

                if (userQuestion != null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: There is a question for user {user.Username}";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                UserCredentialRecovery userCredentialRecovery = new UserCredentialRecovery()
                {
                    User         = new User() { Id = userId },
                    Question     = data.Question,
                    ResponseSalt = PasswordHasher.GenerateSalt()
                };

                var response = data.Response.ToLower();
                userCredentialRecovery.Response = PasswordHasher.ComputeHash(response, userCredentialRecovery.ResponseSalt, Constants.PEPPER, Constants.ITERATION);

                var userCredentialRecoveryId = await _userRecoveryCredentialRepository.InsertAsync(userCredentialRecovery);

                if (userCredentialRecoveryId <= 0)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Register)}, error: Something wrong during user recovery registration";
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

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<LoginRespDto>))]
        public async Task<ActionResult<LoginRespDto>> Login(LoginReqDto data)
        {
            if (data is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: UserRegister is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (data.Username is null || String.IsNullOrEmpty(data.Username) || data.Password is null || String.IsNullOrEmpty(data.Password))
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: credentials are not valid";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new LoginRespDto();

            try
            {
                var user = await _userRepository.GetUserByUsername(data.Username);

                if (user == null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var passwordHash = PasswordHasher.ComputeHash(data.Password, user.PasswordSalt, Constants.PEPPER, Constants.ITERATION);

                if (user.Password != passwordHash)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: Username or password isn't correct";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.Token = JwtTokenService.CreateToken(data.Username, Constants.SYMMETRIC_SECURITY_KEY);

                if (String.IsNullOrEmpty(result.Token))
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: Some error occured during token generation";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsAuth = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(Login)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("getQuestion")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<RestoreQuestionRespDto>))]
        public async Task<ActionResult<RestoreQuestionRespDto>> GetQuestion(string username)
        {
            if (username is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(GetQuestion)}, error: UserRegister is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _userRecoveryCredentialRepository is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(GetQuestion)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new RestoreQuestionRespDto();

            try
            {
                var user = await _userRepository.GetUserByUsername(username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(GetQuestion)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var userQuestion = await _userRecoveryCredentialRepository.GetUserRecoveryCrdentialByUsername(user.Id);

                if (userQuestion is null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(GetQuestion)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                result.Question = userQuestion.Question;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(GetQuestion)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(GetQuestion)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("resetPassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<ResetPasswordRespDto>))]
        public async Task<ActionResult<ResetPasswordRespDto>> ResetPassword(ResetPasswordReqDto data)
        {
            if (data is null)
            {
                _logger.LogError($"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: data is null");
                return BadRequest();
            }

            if (data.Username is null || String.IsNullOrEmpty(data.Username) || data.Response is null || String.IsNullOrEmpty(data.Response) || data.NewPassword is null || String.IsNullOrEmpty(data.NewPassword))
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: invalid user information";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _userRecoveryCredentialRepository is null)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new ResetPasswordRespDto();

            try
            {
                var user = await _userRepository.GetUserByUsername(data.Username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: No user found for username: {data.Username}";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                var userRecoverCredential = await _userRecoveryCredentialRepository.GetUserRecoveryCrdentialByUsername(user.Id);

                if (userRecoverCredential is null)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: No user recover credential found for username: {data.Username}";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                var response = data.Response.ToLower();
                var responseHash = PasswordHasher.ComputeHash(response, userRecoverCredential.ResponseSalt, Constants.PEPPER, Constants.ITERATION);

                if (userRecoverCredential.Response != responseHash)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: response ins't correct";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                user.PasswordSalt = PasswordHasher.GenerateSalt();
                user.Password = PasswordHasher.ComputeHash(data.NewPassword, user.PasswordSalt, Constants.PEPPER, Constants.ITERATION);

                var updateUser = await _userRepository.UpdateAsync(user);

                if (updateUser <= 0)
                {
                    var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: Something wrong during user update";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsResetSuccess = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(UserController)}, method: {nameof(ResetPassword)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

    }
}
