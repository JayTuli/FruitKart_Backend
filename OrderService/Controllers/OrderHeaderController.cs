using Microsoft.AspNetCore.Mvc;
using OrderService.Model;
using OrderService.Model.DTO;
using OrderService.Repository;
using System.Net;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHeaderController: ControllerBase

    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApiResponse _response;

        public OrderHeaderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _response = new ApiResponse();
        }

        // GET api/orderheader?userId=xxx
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string userId = "")
        {
            _response.Result = await _orderRepository.GetAllOrdersAsync(userId);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // GET api/orderheader/{orderId}
        [HttpGet("{orderId:int}", Name = "GetOrder")]
        public async Task<ActionResult<ApiResponse>> GetOrder(int orderId)
        {
            if (orderId == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Invalid order Id");
                return BadRequest(_response);
            }

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Order not found");
                return NotFound(_response);
            }

            _response.Result = order;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // POST api/orderheader
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(_response);
                }

                var created = await _orderRepository.CreateOrderAsync(orderHeaderDTO);

                _response.Result = created;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetOrder", new { orderId = created.OrderHeaderId }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, _response);
            }
        }

        // PUT api/orderheader/{orderId}
        [HttpPut("{orderId:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrder(int orderId, [FromBody] OrderHeaderUpdateDTO orderHeaderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(_response);
                }

                if (orderId != orderHeaderDTO.OrderHeaderId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid Id");
                    return BadRequest(_response);
                }

                var updated = await _orderRepository.UpdateOrderAsync(orderId, orderHeaderDTO);
                if (updated is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Order not found");
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, _response);
            }
        }
    }
}