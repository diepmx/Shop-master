using System;
using System.Threading.Tasks;
using Shop.MoMo;
using System.Web.Mvc;
using Shop.Models;

public class PaymentController : Controller
{
    public async Task<ActionResult> MomoPayment()
    {
        var partnerCode = ConfigHelper.GetAppSetting("MomoPartnerCode");
        var accessKey = ConfigHelper.GetAppSetting("MomoAccessKey");
        var secretKey = ConfigHelper.GetAppSetting("MomoSecretKey");
        var requestUrl = ConfigHelper.GetAppSetting("MomoRequestUrl");
        var redirectUrl = ConfigHelper.GetAppSetting("MomoRedirectUrl");
        var ipnUrl = ConfigHelper.GetAppSetting("MomoIpnUrl");

        // Tạo thông tin giao dịch
        var requestId = Guid.NewGuid().ToString();
        var orderId = Guid.NewGuid().ToString();
        var rawData = $"accessKey={accessKey}&amount=100000&extraData=&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo=Thanh toan don hang&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=captureWallet";

        // Tạo chữ ký
        var signature = MomoHelper.GenerateSignature(rawData, secretKey);

        var momoRequest = new MomoRequest
        {
            PartnerCode = partnerCode,
            PartnerName = "Test Merchant",
            RequestId = requestId,
            Amount = "100000",
            OrderId = orderId,
            OrderInfo = "Thanh toan don hang",
            RedirectUrl = redirectUrl,
            IpnUrl = ipnUrl,
            RequestType = "captureWallet",
            ExtraData = "",
            Signature = signature
        };

        // Gửi yêu cầu đến API Momo
        var response = await MomoHelper.SendMomoRequestAsync(momoRequest, requestUrl);

        if (response != null && response.ResultCode == 0)
        {
            return Redirect(response.PayUrl);
        }

        return Content("Có lỗi xảy ra trong quá trình thanh toán!");
    }

    [HttpPost]
    public ActionResult PaymentNotify()
    {
        // Xử lý thông báo từ Momo
        return Content("OK");
    }

    public ActionResult PaymentSuccess()
    {
        return Content("Thanh toán thành công!");
    }
}
