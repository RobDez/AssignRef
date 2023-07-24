using Sabio.Models;
using Sabio.Models.Domain.CertificationResults;
using Sabio.Models.Domain.Certifications;
using Sabio.Models.Requests.CertificationResults;
using Sabio.Models.Requests.Certifications;

namespace Sabio.Services.Interfaces
{
    public interface ICertificationResultsService
    {
        int Add(CertificationResultAddRequest model, int userId);
        void Update(CertificationResultUpdateRequest model, int userId);
        void Delete(int id, int userId);
        Paged<CertificationResult> Get(int id, int pageIndex, int pageSize);
        Paged<CertificationResult> Search(int pageIndex, int pageSize, string query);
    }
}
