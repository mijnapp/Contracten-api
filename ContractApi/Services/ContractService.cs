using ContractApi.Model;
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

        public ContractService(ReferencedApiService referencedApiService)
        {
            _referencedApiService = referencedApiService;
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

            return responses.SelectMany(i => i).ToList(); //filter out any null values
        }
        public async Task<IList<Contract>> GetListOfContract(ReferencedApiModel api, string bsn)
        {
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
                    return await JsonSerializer.DeserializeAsync<IList<Contract>>(await response.Content.ReadAsStreamAsync());
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return null;
        }
    }
}
