using Models;
using Models.Domain.CertificationResults;
using Models.Domain.Certifications;
using Models.Requests.CertificationResults;
using Models.Requests.Certifications;

namespace Services.Interfaces
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
