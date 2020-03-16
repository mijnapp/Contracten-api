﻿using ContractApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ContractApi.Api
{
    [ApiController]
    public class ContractController : ControllerBase
    {
        [Route("api/personen/{bsn}/contracten")]
        public IActionResult GetAllContracts()
        {
            var contracts = new List<Contract>
            {
                new Contract
                {
                    Identificatie = Guid.NewGuid().ToString(),
                    Titel = "Integraal - plan",
                    Status = "actief",
                    BeginDatum = DateTime.Now.AddMonths(-4),
                    EindDatum = DateTime.Now.AddMonths(4),
                    Organisatie = "Solviteers"
                },
                new Contract
                {
                    Identificatie = Guid.NewGuid().ToString(),
                    Titel = "Burger - plan",
                    Status = "actief",
                    BeginDatum = DateTime.Now.AddMonths(-4),
                    EindDatum = DateTime.Now.AddMonths(4),
                    Organisatie = "Solviteers"
                }
            };
            return new JsonResult(contracts);
        }
    }
}