using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WalletWise.Model.BalanceModels;
using WalletWise.Model.DTO.Balance;
using WalletWise.Model.UserModels;
using WalletWise.Repository.BalanceRepository;
using WalletWise.Repository.UserRepository;

namespace WalletWiseApi.Controllers.BalanceControllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration? _config;
        private readonly ILogger<CategoryController> _logger;
        private readonly UserRepository? _userRepository;
        private readonly CategoryRepository _categoryRepository;
        private string? _connString;

        public CategoryController(IConfiguration? config,
                              ILogger<CategoryController> logger)
        {
            _config     = config;
            _logger     = logger;
            _connString = _config?.GetConnectionString(Constants.WALLET_WISE_CONN_STRING);

            if (string.IsNullOrWhiteSpace(_connString))
                throw new ArgumentNullException(nameof(_connString));

            _userRepository     = new UserRepository(_connString);
            _categoryRepository = new CategoryRepository(_connString);
        }

        [HttpGet]
        [Route("getCategories")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<GetCategoriesRespDto>))]
        public async Task<ActionResult<GetCategoriesRespDto>> GetCategories(string username, CategoryType? type = null, bool isDeleted = false)
        {
            if (username is null)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(GetCategories)}, error: username is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _categoryRepository is null)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(GetCategories)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new GetCategoriesRespDto();
            result.Categories = new List<Category>();

            try
            {
                var user = await _userRepository.GetUserByUsername(username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(GetCategories)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var categories = await _categoryRepository.GetAllAsync(user.Id, type, isDeleted);

                result.Categories = categories;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(GetCategories)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(GetCategories)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("insertCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<CategoryRespDto>))]
        public async Task<ActionResult<CategoryRespDto>> InsertCategories(CategoryReqDto data)
        {
            if (data is null)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: data is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if(data.Username         is null || string.IsNullOrEmpty(data.Username) || 
                data.Icon            is null || string.IsNullOrEmpty(data.Icon) || 
                data.ColorBackground is null || string.IsNullOrEmpty(data.ColorBackground) ||
                data.Description     is null || string.IsNullOrEmpty(data.Description))
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: data are not valid";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _categoryRepository is null)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new CategoryRespDto();

            try 
            {
                var user = await _userRepository.GetUserByUsername(data.Username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                Category category = new Category() 
                {
                    User            = new User() { Id = user.Id },
                    Description     = data.Description,
                    CategoryType    = (CategoryType)data.Type,
                    Icon            = data.Icon,
                    ColorBackground = data.ColorBackground,
                    IsDeleted       = data.IsDelete
                };

                var index = await _categoryRepository.InsertAsync(category);

                if (index <= 0)
                {
                    var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: Something wrong during insert category";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsSuccess = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(CategoryController)}, method: {nameof(InsertCategories)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }
    }
}
