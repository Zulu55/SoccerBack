using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Index("Team_Name_LeagueId_Index", IsUnique = true, Order = 1)]
        [Display(Name = "Team")]
        public string Name { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(3, ErrorMessage = "The length for field {0} must be {1} characters", MinimumLength = 3)]
        [Index("Team_Initials_LeagueId_Index", IsUnique = true, Order = 1)]
        public string Initials { get; set; }

        [Index("Team_Name_LeagueId_Index", IsUnique = true, Order = 2)]
        [Index("Team_Initials_LeagueId_Index", IsUnique = true, Order = 2)]
        [Display(Name = "League")]
        public int LeagueId { get; set; }

        [JsonIgnore]
        public virtual League League { get; set; }

        [JsonIgnore]
        public virtual ICollection<TournamentTeam> TournamentTeams { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Fans { get; set; }

        [JsonIgnore]
        public virtual ICollection<Match> Locals { get; set; }

        [JsonIgnore]
        public virtual ICollection<Match> Visitors { get; set; }
    }
}
