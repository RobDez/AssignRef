using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Games
{
    public class GameAddRequest
    {
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int VenueId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int SeasonId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int Week { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int HomeTeamId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int VisitingTeamId { get; set; }
        [Required]
        public bool IsNonConference { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int ConferenceId { get; set; }
    }
}
