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
    public ActionResult<PaymentResponse> Pay([FromBody] PaymentRequest request)
    {
        var response = _paymentService.Pay(request);
        return Ok(response);
    }
}
