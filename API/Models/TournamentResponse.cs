using Domain;
using System.Collections.Generic;

namespace API.Models
{
    public class TournamentResponse
    {
        public int TournamentId { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public List<TournamentGroup> Groups { get; set; }

        public List<Date> Dates { get; set; }
    }
}