using Domain;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Backend.Models
{
    public class TeamView : Team
    {
        [Display(Name = "Logo")]
        public HttpPostedFileBase LogoFile { get; set; }
    }
}