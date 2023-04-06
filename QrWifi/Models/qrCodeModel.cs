using System.ComponentModel.DataAnnotations;

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

    }
}