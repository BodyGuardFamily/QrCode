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
using System.Reflection;
using Microsoft.AspNetCore.Http;

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

        public IActionResult Index(qrCodeModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Privacy(qrCodeModel model)
        {
            Run(model);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //determine if the user picked wep or wpa
        public PayloadGenerator.WiFi.Authentication WepOrWPa(qrCodeModel model)
        {
            PayloadGenerator.WiFi.Authentication authmode;

            if (model.auth == "WEP")
            {
                return authmode = PayloadGenerator.WiFi.Authentication.WEP;
            }
            else
            {
                return authmode = PayloadGenerator.WiFi.Authentication.WPA;
            }
        }

        //determin if the ssid is hidden or not
        private bool isSSIDHIDDEN(qrCodeModel model)
        {
            //to determine if hidessis is on or off
            bool hideSSid;

            if (model.hiddenSSID == "true")
            {
                return hideSSid = true;
            }
            else
            {
                return hideSSid = false;
            }
        }

        //this function creates the qr code
        public QRCode createQR(qrCodeModel model, PayloadGenerator.WiFi.Authentication authmode, bool ssidHidden) 
        {
            WiFi generator = new PayloadGenerator.WiFi(model.ssid, model.password, authmode, ssidHidden);
            generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(generator, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            return qrCode;
        }

        //this function will run if the user picks a picture they want to use and this will display the qr code with the picture
        public void userPicture(qrCodeModel model, IFormFile formFile, QRCode qrCode)
        {
            // Use the uploaded image file
            using (Bitmap uploadedImage = new Bitmap(formFile.OpenReadStream()))
            {
                //this create the qr code with the colors and the picture.
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

        //if user doesnt pick a picture this is function will run the default picture robot.png with the qr code
        public void defaultPicture(qrCodeModel model, QRCode qrCode)
        {
            // Use the default image file
            var defaultImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "robot.png");

            //this create the qr code with the colors and the picture.
            using (Bitmap qrCodeImage = qrCode.GetGraphic(60, model.bgColor, model.fgColor, (Bitmap)Bitmap.FromFile(defaultImagePath)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    ViewBag.QRcode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        //this run function is meant to run all the funtions 
        public void Run(qrCodeModel model)
        {
            //determine if the user picked wep or wpa
            PayloadGenerator.WiFi.Authentication authmode = WepOrWPa(model);

            //to determine if the ssid is hidden or not
            bool ssidHIDDEN = isSSIDHIDDEN(model);

            QRCode qrCode = createQR(model, authmode, ssidHIDDEN);

            //determine if there is a picture 
            if (model.Imgpath != null && model.Imgpath.Length > 0 && model.Imgpath is IFormFile formFile && formFile.ContentType.StartsWith("image/"))
            {
                // Use the uploaded image file
                userPicture(model, formFile, qrCode);
            }
            else
            {
                // Use the default image file
                defaultPicture(model, qrCode);
            }
        }
    }
}


