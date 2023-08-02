using Models.Domain.Certifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain.CertificationResults
{
    public class CertificationResult
    {
        public int Id { get; set; }

        public Certification Certification { get; set; }
        public bool IsPhysicalCompleted { get; set; }
        public bool IsBackgroundCheckCompleted { get; set; }
        public bool IsTestCompleted { get; set; }
        public int? TestInstanceId { get; set; }
        public decimal? Score { get; set; }
        public bool IsFitnessTestCompleted { get; set; }
        public bool IsClinicAttended { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public BaseUser User { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
