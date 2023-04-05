using System.ComponentModel.DataAnnotations;

namespace QrWifi.Models
{
    public class qrCodeModel
    {
        public string ssid { get; set; }
        public string password { get; set; }
        //public string auth { get; set; }
        public string Payload { get; set; }
    }
}