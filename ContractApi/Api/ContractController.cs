using ContractApi.Filters;
using ContractApi.Model;
using ContractApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractApi.Api
{
    [ApiKeyAuthorize]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly ContractService _contractService;
        public ContractController(ContractService contractService)
        {
            _contractService = contractService;
        }

        [Route("api/personen/{bsn}/contracten")]
        [HttpGet]
        public async Task<IList<Contract>> GetAllContractsAsync(string bsn)
        {
            var contracts = await _contractService.GetAllContractForBsnAsync(bsn);
            return contracts;
        }
    }
}