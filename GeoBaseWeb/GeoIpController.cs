using Microsoft.AspNetCore.Mvc;
using System.Net;
using GeoIp.Data;
using GeoIp.Utils;
using GeoIp;
using Microsoft.Extensions.Options;

namespace GeoBaseWeb
{

    [ApiController]
    public class GeoIpController : ControllerBase
    {
        private readonly GeoIpDatabase dataBase;
        private readonly IOptions<GeoIpOptions> options;

        public GeoIpController(GeoIpDatabase dataBase, IOptions<GeoIpOptions> options)
        {
            this.dataBase = dataBase;
            this.options = options;
        }


        [HttpGet("search")]
        public FindResponse SearchLocations(string searchString)
        {
            if (IPAddress.TryParse(searchString, out IPAddress? ipAddress))
            {
                Location? location = dataBase.FindLocationByIpAddress(ipAddress.ToInteger());
                if (location.HasValue)
                {
                    return FindResponse.OK(location.Value.GetManaged());
                }
                else
                {
                    return FindResponse.NotFound();
                }
            }
            else
            {
                int i = 0;
                int count = options.Value.MaxRecordCount;

                FindResponse response = FindResponse.OK();
                dataBase.FindLocationsByCity(searchString, StringSearchMode.Starts, location =>
                {
                    response.Locations.Add(location.GetManaged());
                    return i++ < count;
                });

                return response.Locations.Any() ? response : FindResponse.NotFound();
            }
        }




        public class FindResponse
        {
            public enum ResultCode
            {
                OK,
                ERROR,
                NOT_FOUND
            }

            public ResultCode Code { get; set; }
            public string? Message { get; set; }

            public List<Location.Managed> Locations { get; } = new List<Location.Managed>();

            public static FindResponse OK(params Location.Managed[] locations)
            {
                FindResponse result = new FindResponse { Code = ResultCode.OK };
                result.Locations.AddRange(locations);
                return result;
            }

            public static FindResponse Error(string message)
            {
                return new FindResponse { Code = ResultCode.ERROR, Message = message };
            }


            public static FindResponse NotFound()
            {
                return new FindResponse { Code = ResultCode.NOT_FOUND, Message = "Not found" };
            }
        }
    }
}
