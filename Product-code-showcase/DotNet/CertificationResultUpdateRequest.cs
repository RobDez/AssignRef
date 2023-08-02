using Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests.CertificationResults
{
    public class CertificationResultUpdateRequest : CertificationResultAddRequest, IModelIdentifier
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}
