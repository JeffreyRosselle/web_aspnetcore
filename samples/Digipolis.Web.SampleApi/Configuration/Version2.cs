using System;
using Microsoft.OpenApi.Models;

namespace Digipolis.Web.SampleApi.Configuration
{
    /// <summary>
    /// Contains all information for V2 of the API
    /// </summary>
    public class Version2 : OpenApiInfo
    {
        public Version2()
        {
            this.Version = Versions.V2;
            this.Title = "API V2";
            this.Description = "Description for V2 of the API";
            this.Contact = new OpenApiContact { Email = "info@digipolis.be", Name = "Digipolis", Url = new Uri("https://www.digipolis.be") };
            this.TermsOfService = new Uri("https://www.digipolis.be/tos");
            this.License = new OpenApiLicense
            {
                Name = "My License",
                Url = new Uri("https://www.digipolis.be/licensing")
            };
        }
    }
}
