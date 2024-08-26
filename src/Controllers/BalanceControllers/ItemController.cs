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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IConfiguration? _config;
        private readonly ILogger<ItemController> _logger;
        private readonly UserRepository? _userRepository;
        private readonly ItemRepository? _itemRepository;
        private string? _connString;

        public ItemController(IConfiguration? config,
                              ILogger<ItemController> logger)
        {
            _config = config;
            _logger = logger;
            _connString = _config?.GetConnectionString(Constants.WALLET_WISE_CONN_STRING);

            if (string.IsNullOrWhiteSpace(_connString))
                throw new ArgumentNullException(nameof(_connString));

            _userRepository = new UserRepository(_connString);
            _itemRepository = new ItemRepository(_connString);
        }

        [HttpGet]
        [Route("getItems")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<GetItemsRespDto>))]
        public async Task<ActionResult<GetItemsRespDto>> GetItems(string username, long? categoryId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (username is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(GetItems)}, error: username is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_itemRepository is null || _userRepository is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(GetItems)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new GetItemsRespDto();
            result.Items = new List<Item>();

            try
            {
                var user = await _userRepository.GetUserByUsername(username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(GetItems)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var items = await _itemRepository.GetAllAsync(user.Id, categoryId, startDate, endDate);
                result.Items = items;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(GetItems)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(GetItems)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("insertItem")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<ItemRespDto>))]
        public async Task<ActionResult<ItemRespDto>> InsertItem(ItemReqDto data)
        {
            if (data is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: data is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (data.Username is null || string.IsNullOrEmpty(data.Username) || data.CategoryId <= 0)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: data are not valid";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _itemRepository is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new ItemRespDto();

            try
            {
                var user = await _userRepository.GetUserByUsername(data.Username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var item = new Item() 
                {
                    User        = new User() { Id = user.Id },
                    Category    = new Category() { Id = data.CategoryId },
                    Description = data.Description,
                    Amount      = data.Amount,   
                    Date        = data.Date
                };

                var index = await _itemRepository.InsertAsync(item);

                if (index <= 0)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: Something wrong during insert category";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsSuccess = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(InsertItem)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("updateItem")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<ItemRespDto>))]
        public async Task<ActionResult<ItemRespDto>> UpdateItem(ItemReqDto data)
        {
            if (data is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: data is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (data.Username is null || string.IsNullOrEmpty(data.Username) || data.CategoryId <= 0 || data.Id <= 0)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: data are not valid";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _itemRepository is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new ItemRespDto();

            try
            {
                var user = await _userRepository.GetUserByUsername(data.Username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var item = await _itemRepository.GetByIdAsync(data.Id);

                if (item is null)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: Item not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                item.Description = data.Description;
                item.Amount      = data.Amount;
                item.Date        = data.Date;

                var index = await _itemRepository.UpdateAsync(item);

                if (index <= 0)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: Something wrong during insert category";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsSuccess = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(UpdateItem)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("deleteItem")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult<ItemRespDto>))]
        public async Task<ActionResult<ItemRespDto>> DeleteItem(ItemReqDto data)
        {
            if (data is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: data is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (data.Username is null || string.IsNullOrEmpty(data.Username) || data.CategoryId <= 0)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: data are not valid";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (_userRepository is null || _itemRepository is null)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: repository is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = new ItemRespDto();

            try
            {
                var user = await _userRepository.GetUserByUsername(data.Username);

                if (user is null)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: User not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var item = await _itemRepository.GetByIdAsync(data.Id);

                if (item is null)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: Item not found";
                    _logger.LogError(msg);
                    return NotFound(msg);
                }

                var index = await _itemRepository.DeleteAsync(item);

                if (index <= 0)
                {
                    var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: Something wrong during insert category";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                result.IsSuccess = true;
            }
            catch (SqlException ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            catch (Exception ex)
            {
                var msg = $"Error class: {nameof(ItemController)}, method: {nameof(DeleteItem)}, error: {ex.Message}";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            return Ok(result);
        }
    }
}
