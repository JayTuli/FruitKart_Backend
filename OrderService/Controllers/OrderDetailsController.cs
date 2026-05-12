using Microsoft.AspNetCore.Mvc;
using System.Net;
using OrderService.Repository;
using OrderService.Model.DTO;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApiResponse _response;

        public OrderDetailsController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _response = new ApiResponse();
        }

        // PUT api/orderdetails/{orderDetailsId}
        [HttpPut("{orderDetailsId:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrder(
            int orderDetailsId,
            [FromBody] OrderDetailUpdateDTO orderDetailsDTO)
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

                if (orderDetailsId != orderDetailsDTO.OrderDetailId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid Id");
                    return BadRequest(_response);
                }

                var updated = await _orderRepository.UpdateOrderDetailRatingAsync(
                    orderDetailsId, orderDetailsDTO.Rating);

                if (updated is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Order detail not found");
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