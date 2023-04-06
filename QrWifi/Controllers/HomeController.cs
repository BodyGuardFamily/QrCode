using Microsoft.AspNetCore.Mvc;
using QrWifi.Models;
using System.Diagnostics;
using System.Linq;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Shouldly;
using static QRCoder.PayloadGenerator;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


//library we used were shouldly and QRcode
namespace QrWifi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        //we needed to put everything in privacy because we were getting null values in index since there was no info getting grabbed
        public IActionResult Privacy(qrCodeModel model)
        {
            PayloadGenerator.WiFi.Authentication authmode;
            
            //to determine if hidessis is on or off
            bool hideSSid;
            string imgpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/robot.png");

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
            if(model.hiddenSSID == "true")
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

            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(generator, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

/*                using (Bitmap qrCodeImage = qrCode.GetGraphic(60, Color.DeepPink, Color.Black, false))*/                
                using (Bitmap qrCodeImage = qrCode.GetGraphic(60, Color.DeepPink, Color.Black, (Bitmap)Bitmap.FromFile(imgpath)))
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    ViewBag.QRcode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
              
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //hello test
    }
}