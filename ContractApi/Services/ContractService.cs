using ContractApi.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContractApi.Services
{
    public class ContractService
    {
        private readonly ReferencedApiService _referencedApiService;
        private readonly ILogger<ContractService> _logger;

        public ContractService(ReferencedApiService referencedApiService, ILogger<ContractService> logger)
        {
            _referencedApiService = referencedApiService;
            _logger = logger;
        }

        public async Task<IList<Contract>> GetAllContractForBsnAsync(string bsn)
        {
            var tasks = new List<Task<IList<Contract>>>();
            foreach (var api in _referencedApiService.GetAllReferencedApi())
            {
                //create the search tasks to be executed
                tasks.Add(GetListOfContract(api, bsn));
            }

            // Await the completion of all the running tasks. 
            var responses = await Task.WhenAll(tasks); // returns IList<IList<Contract>>>>

            return responses.Where(i => i != null).SelectMany(i => i).ToList(); //filter out any null values
        }
        public async Task<IList<Contract>> GetListOfContract(ReferencedApiModel api, string bsn)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try { 
                using var client = new HttpClient
                {
                    BaseAddress = new Uri(api.Url)
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(api.ApiKey);

                //GET Method  
                using (var response = await client.GetAsync($"api/personen/{bsn}/contracten"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var contracts = await JsonSerializer.DeserializeAsync<IList<Contract>>(await response.Content.ReadAsStreamAsync(), options);
                        return contracts;
                    }
                    else
                    {
                        _logger.LogError($"Api {api.Name} returned statuscode: {response.StatusCode}");
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Api exception for {api.Name}");
                return null;
            }
        }
    }
}
