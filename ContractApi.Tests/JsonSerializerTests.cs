using ContractApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Linq;

namespace ContractApi.Tests
{
    [TestClass]
    public class JsonSerializerTests
    {
        [TestMethod]
        public void CheckJsonParser()
        {
            //Arrange
            var json = "[{\"identificatie\":\"555bf9ea-4f10-4bc8-a200-cd2c9b7ba56e\",\"bsn\":null,\"titel\":\"Integraal - plan\",\"status\":\"actief\",\"beginDatum\":\"2019-11-18T13:08:43.4235764+01:00\",\"eindDatum\":\"2020-07-18T13:08:43.4235928+02:00\",\"organisatie\":\"Solviteers\"},{\"identificatie\":\"663bbf66-e092-44d1-bdd2-7005f5d4d6cf\",\"bsn\":null,\"titel\":\"Burger - plan\",\"status\":\"actief\",\"beginDatum\":\"2019-11-18T13:08:43.4236165+01:00\",\"eindDatum\":\"2020-07-18T13:08:43.4236199+02:00\",\"organisatie\":\"Solviteers\"}]";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            //Act
            var result = (IList<Contract>)JsonSerializer.Deserialize(json, typeof(IList<Contract>), options);
            //Asser
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("555bf9ea-4f10-4bc8-a200-cd2c9b7ba56e", result.First().Identificatie);
            Assert.AreEqual("actief", result.First().Status);
            Assert.AreEqual(new DateTime(2019, 11, 18).Date, result.First().BeginDatum.Date);
        }
    }
}
