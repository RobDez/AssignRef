using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.CertificationResults
{
    public class CertificationResultAddRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CertificationId { get; set; }

        [Required]
        public bool IsPhysicalCompleted { get; set; }

        [Required]
        public bool IsBackgroundCheckCompleted { get; set; }

        [Required]
        public bool IsTestCompleted { get; set; }

        //[Required]
        //[Range(1, int.MaxValue)]
        public decimal? Score { get; set; }

        [Required]
        public bool IsFitnessTestCompleted { get; set; }

        //[Required]
        //[Range(1, int.MaxValue)]
        [AllowNull]
        public int? TestInstanceId { get; set; }

        [Required]
        public bool IsClinicAttended { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
