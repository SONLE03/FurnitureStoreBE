using FurnitureStoreBE.Services.OrderService;
using Microsoft.AspNetCore.Mvc;
using FurnitureStoreBE.Configurations;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Web;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.ProductService.FurnitureTypeService;
using FurnitureStoreBE.Utils;
using System.Net;

namespace FurnitureStoreBE.Controllers.PaymentController
{
    [Route("api/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;

        public VNPayController(IOrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        [HttpPost("submit-order")]
        public string SubmitOrder([FromQuery] decimal amount, [FromQuery] string orderInfo, [FromQuery] string orderId)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();

            Console.WriteLine(clientIPAddress);
            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            var vnp_PayUrl = _configuration.GetValue<string>("VNPay:VNP_PayUrl");
            var vnp_Returnurl = _configuration.GetValue<string>("VNPay:VNP_Returnurl");
            var vnp_TmnCode = _configuration.GetValue<string>("VNPay:VNP_TmnCode");
            var vnp_HashSecret = _configuration.GetValue<string>("VNPay:VNP_HashSecret");
            var vnp_apiUrl = _configuration.GetValue<string>("VNPay:VNP_ApiUrl");
            Console.WriteLine(baseUrl + vnp_Returnurl);
            VNPayConfig pay = new VNPayConfig();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", vnp_TmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (amount * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", orderInfo); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", baseUrl + vnp_Returnurl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", orderId); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(vnp_PayUrl, vnp_HashSecret);
            return paymentUrl;
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> PaymentConfirm()
        {

            if (Request.QueryString.HasValue)
            {
                var vnp_HashSecret = _configuration.GetValue<string>("VNPay:VNP_HashSecret");
                var vnp_TmnCode = _configuration.GetValue<string>("VNPay:VNP_TmnCode");

                //lấy toàn bộ dữ liệu trả về
                var queryString = Request.QueryString.Value;
                var json = HttpUtility.ParseQueryString(queryString);

                Guid orderId = Guid.Parse(json["vnp_TxnRef"].ToString());
                string orderInfor = json["vnp_OrderInfo"].ToString(); //Thông tin giao dịch
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString(); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = json["vnp_SecureHash"].ToString(); //hash của dữ liệu trả về
                var pos = Request.QueryString.Value.IndexOf("&vnp_SecureHash");

                //return Ok(Request.QueryString.Value.Substring(1, pos-1) + "\n" + vnp_SecureHash + "\n"+ PayLib.HmacSHA512(hashSecret, Request.QueryString.Value.Substring(1, pos-1)));
                bool checkSignature = VNPayConfig.ValidateSignature(Request.QueryString.Value.Substring(1, pos - 1), vnp_SecureHash, vnp_HashSecret); //check chữ ký đúng hay không?
                if (checkSignature && vnp_TmnCode == json["vnp_TmnCode"].ToString())
                {
                    if (vnp_ResponseCode == "00")
                    {
                        return new SuccessfulResponse<object>(await _orderService.CreateOrderPaid(orderId), (int)HttpStatusCode.OK, "Payment successfully").GetResponse();
                    }
                    else
                    {
                        return BadRequest($"Payment failed {vnp_ResponseCode}");
                    }
                }
                else
                {
                    return BadRequest("đường dẫn nếu phản hồi ko hợp lệ");
                }
            }
           
            return BadRequest("Phản hồi không hợp lệ");
        }
       
    }
}
