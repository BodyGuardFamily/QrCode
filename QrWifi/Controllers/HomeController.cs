using Microsoft.AspNetCore.Mvc;
using QrWifi.Models;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using QRCoder;
using static QRCoder.PayloadGenerator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace QrWifi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var model = new qrCodeModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Privacy(qrCodeModel model)
        {
            PayloadGenerator.WiFi.Authentication authmode;

            //to determine if hidessis is on or off
            bool hideSSid;

            //determine if the user picked wep or wpa
            if (model.auth == "WEP")
            {
                authmode = PayloadGenerator.WiFi.Authentication.WEP;
            }
            else
            {
                authmode = PayloadGenerator.WiFi.Authentication.WPA;
            }

            //to determine if the ssid is hidden or not
            if (model.hiddenSSID == "true")
            {
                hideSSid = true;
            }
            else
            {
                hideSSid = false;
            }

            WiFi generator = new PayloadGenerator.WiFi(model.ssid, model.password, authmode, hideSSid);
            //WiFi generator = new PayloadGenerator.WiFi(ssid1, password1, authmode);
            generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(generator, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);


            //determine if there is a picture 
            if (model.Imgpath != null && model.Imgpath.Length > 0 && model.Imgpath is IFormFile formFile && formFile.ContentType.StartsWith("image/"))
            {
                // Use the uploaded image file
                using (Bitmap uploadedImage = new Bitmap(formFile.OpenReadStream()))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(60, model.bgColor, model.fgColor, uploadedImage))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            ViewBag.QRcode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            else
            {
                // Use the default image file
                var defaultImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "robot.png");
                using (Bitmap qrCodeImage = qrCode.GetGraphic(60, model.bgColor, model.fgColor, (Bitmap)Bitmap.FromFile(defaultImagePath)))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        ViewBag.QRcode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
