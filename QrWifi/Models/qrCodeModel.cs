using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace QrWifi.Models
{
    public class qrCodeModel
    {
        //to get and set ssid name
        public string ssid { get; set; } = default!;

        //to get and set the password
        public string password { get; set; } = default!;

        //to get and set the authenication 
        public string auth { get; set; } = default!;

        //to get and set if the ssid is hidden
        public string hiddenSSID { get; set; } = default!;

        //to get and set the different hex colors
        //this is set default to black
        public Color bgColor { get; set; } = default!;

        //this is set to default white
        public Color fgColor { get; set; } = Color.White;

        //this will set the default color to white
        public string GetFgColorHex()
        {
            return $"#{fgColor.R:X2}{fgColor.G:X2}{fgColor.B:X2}";
        }

        //to get and set the user image
        public IFormFile Imgpath { get; set; } = default!;
    }
}