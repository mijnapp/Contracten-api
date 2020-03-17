using ContractApi.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ContractApi.Services
{
    public class ReferencedApiService
    {
        private const string ConfigurationSectionName = "Api:ReferencedApis";
        private readonly IConfiguration _configuration;

        public ReferencedApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IList<ReferencedApiModel> GetAllReferencedApi()
        {
            var apiList = new List<ReferencedApiModel>();
            var configSection = _configuration.GetSection(ConfigurationSectionName);

            foreach (var section in configSection.GetChildren())
            {
                var isActive = section.GetValue<bool>("IsActive");
                if (!isActive)
                {
                    continue;
                }

                var name = section.GetValue<string>("Name");
                var url = section.GetValue<string>("Url");
                var apiKey = section.GetValue<string>("ApiKey");

                apiList.Add(new ReferencedApiModel
                {
                    Name = name,
                    Url = url,
                    IsActive = isActive,
                    ApiKey = apiKey
                });
            }
            return apiList;
        }
    }
}
