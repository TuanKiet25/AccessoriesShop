# PayOS Signature Verification Error - Diagnostic & Solutions

## Error: "Mã kiểm tra(signature) không hợp lệ" (Invalid Signature)

This error occurs when PayOS cannot validate the webhook signature. Here are the common causes and solutions:

---

## **Root Causes**

### 1. **Invalid Checksum Key** ⚠️ MOST COMMON
The ChecksumKey in appsettings.json doesn't match the one from PayOS Dashboard.

**Solution:**
```json
// appsettings.json
"PayOSSettings": {
    "ClientId": "YOUR_CLIENT_ID",
    "ApiKey": "YOUR_API_KEY",
    "ChecksumKey": "VERIFY_THIS_IN_PAYOS_DASHBOARD",  // ← Must be exactly correct
    "ReturnUrl": "https://yourfrontend.com/payment/success",
    "CancelUrl": "https://yourfrontend.com/payment/cancel"
}
```

### 2. **Request Body Already Consumed**
The webhook body might be read before PayOS signature verification occurs.

**Solution: Enable Request Body Buffering**

Update [Program.cs](d:\Projects\PRN232\AccessoriesShop.Web\Program.cs):
```csharp
var app = builder.Build();

// Add this BEFORE UseHttpsRedirection and other middleware
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 3. **Content-Type Header Missing**
PayOS webhook MUST have proper headers for signature verification.

**Expected Headers from PayOS:**
```
Content-Type: application/json
x-payos-signature: <signature_value>
```

### 4. **Wrong Webhook Endpoint URL**
Make sure webhook URL registered in PayOS Dashboard matches your actual endpoint.

**Verify:**
- PayOS Dashboard → Settings → Webhook URL should be: `https://yourdomain.com/api/payments/payos/webhook`
- Must be HTTPS (not HTTP)
- Must be publicly accessible

---

## **Implementation Fix**

### Option A: Enable Body Buffering (Recommended)

Add this middleware to [Program.cs](d:\Projects\PRN232\AccessoriesShop.Web\Program.cs) before other middleware:

```csharp
var app = builder.Build();

// Enable request body buffering for webhook signature verification
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// ... rest of middleware
```

### Option B: Use Raw Request Body

Modify [PaymentController.cs](d:\Projects\PRN232\AccessoriesShop.Web\Controllers\PaymentController.cs) webhook endpoint:

```csharp
[HttpPost("payos/webhook")]
[AllowAnonymous]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> PayOSWebhook()
{
    try
    {
        // Read raw body for signature verification
        var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        _logger.LogInformation("PayOS webhook received: {Body}", body);
        
        // Parse webhook from body
        var webhookBody = System.Text.Json.JsonSerializer.Deserialize<Webhook>(body);
        
        var result = await _payOSService.ProcessWebhookAsync(webhookBody);
        
        return Ok(IpnResponse.FromResult(result.IsSuccess, result.Message ?? PaymentStatus.Success));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Webhook processing error");
        return BadRequest("Invalid webhook");
    }
}
```

---

## **Verification Checklist**

✅ **1. Check PayOS Settings**
```csharp
// Test in Program.cs
var payosSettings = builder.Configuration.GetSection("PayOSSettings");
Console.WriteLine($"ClientId: {payosSettings["ClientId"]}");
Console.WriteLine($"ChecksumKey: {payosSettings["ChecksumKey"]}");
// These MUST match PayOS Dashboard
```

✅ **2. Verify Webhook URL**
- Go to PayOS Dashboard
- Check registered webhook URL matches `https://yourdomain.com/api/payments/payos/webhook`
- Copy exact ChecksumKey from dashboard

✅ **3. Check Network**
```
POST https://yourdomain.com/api/payments/payos/webhook
Headers:
  Content-Type: application/json
  x-payos-signature: <signature>
Body: {...}
```

✅ **4. Enable Logging to Debug**
Add detailed logging in PayOSService:

```csharp
public async Task<ServiceResult<ProcessPaymentResult>> ProcessWebhookAsync(Webhook webhookBody)
{
    try
    {
        _logger.LogInformation("Webhook Body: {@WebhookBody}", webhookBody);
        _logger.LogInformation("PayOS Signature Key: {ChecksumKey}", _settings.ChecksumKey);
        
        WebhookData verifiedData = await _payOS.Webhooks.VerifyAsync(webhookBody);
        // ...
    }
    catch (PayOS.Exceptions.ApiException ex)
    {
        _logger.LogError(ex, "PayOS Signature Error: {Message}", ex.Message);
        throw;
    }
}
```

---

## **Quick Fix (Immediate)**

1. **Verify ChecksumKey is correct:**
   - Login to PayOS Dashboard
   - Copy ChecksumKey exactly
   - Replace in appsettings.json

2. **Enable body buffering in Program.cs:**
   ```csharp
   app.Use(async (context, next) =>
   {
       context.Request.EnableBuffering();
       await next();
   });
   ```

3. **Restart the application**

---

## **If Still Not Working**

Check logs for exact error message:
- Look for: "Error processing PayOS webhook"
- Log level should be set to Debug to see detailed signatures
