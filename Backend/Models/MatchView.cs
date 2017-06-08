using Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [NotMapped]
    public class MatchView : Match
    {
        [Display(Name = "Date")]
        public string DateString { get; set; }

        [Display(Name = "Time")]
        public string TimeString { get; set; }

        [Display(Name = "Local League")]
        public int LocalLeagueId { get; set; }

        [Display(Name = "Visitor League")]
        public int VisitorLeagueId { get; set; }
    }
}