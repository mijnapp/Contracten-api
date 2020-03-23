using System;

namespace ContractApi.Model
{
    public class Contract
    {
        public string Identificatie { get; set; }
        public string Bsn { get; set; }
        public string Titel { get; set; }
        public string Status { get; set; }
        public DateTime BeginDatum { get; set; }
        public DateTime? EindDatum { get; set; }
        public string Organisatie { get; set; }
    }
}
