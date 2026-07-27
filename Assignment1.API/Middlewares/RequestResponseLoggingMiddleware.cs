using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Assignment1.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // 1. Log Request
        var requestBody = await ReadRequestBodyAsync(context.Request);
        var maskedRequestBody = MaskSensitiveData(requestBody);

        _logger.LogInformation(
            "HTTP Request: {Method} {Path}{QueryString} | Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            string.IsNullOrWhiteSpace(maskedRequestBody) ? "[Empty]" : maskedRequestBody
        );

        // 2. Intercept Response Stream
        var originalResponseBodyStream = context.Response.Body;
        using var responseBodyMemoryStream = new MemoryStream();
        context.Response.Body = responseBodyMemoryStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // 3. Log Response
            responseBodyMemoryStream.Position = 0;
            var responseBody = await new StreamReader(responseBodyMemoryStream, Encoding.UTF8).ReadToEndAsync();
            responseBodyMemoryStream.Position = 0;

            _logger.LogInformation(
                "HTTP Response: {StatusCode} | Duration: {ElapsedMs}ms | Body: {Body}",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                string.IsNullOrWhiteSpace(responseBody) ? "[Empty]" : responseBody
            );

            // Copy response stream back to original HTTP response body stream
            await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        if (request.Body.CanRead && request.ContentLength is > 0)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        return string.Empty;
    }

    private static string MaskSensitiveData(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return body;

        // Mask credit card numbers (keep last 4 digits)
        body = Regex.Replace(body, @"(""(?:card_number|cardNumber)""\s*:\s*"")\d{12}(\d{4}"")", "$1************$2");

        // Mask CVV
        body = Regex.Replace(body, @"(""(?:cvv)""\s*:\s*"")[^""]+("")", "$1***$2");

        return body;
    }
}
