using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Gateway.Dto.User
{
    public struct DeviceLinkRequest
    {
        public string Email { get; set; }
        public string DeviceId { get; set; }
        public string MacAddress { get; set; }
    }
}
