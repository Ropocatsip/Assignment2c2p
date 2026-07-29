using Assignment1.Models;
using Assignment1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

[ApiController]
[Route("api/v1")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("pay")]
    public async Task<ActionResult<PaymentResponse>> Pay([FromBody] PaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _paymentService.PayAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

