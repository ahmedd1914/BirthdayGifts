using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayGifts.Models
{
    public class VotingSession
    {
        [Key]   
        public int VotingSessionId { get; set; }

        [Required]
        [Display(Name = "Vote Session Creator")]
        public int VoteSessionCreatorId { get; set; }
        [Required]
        [Display(Name = "Birthday Person ID")]
        public int BirthdayPersonId { get; set; }
        [Required]
        [Display(Name = "Is Active")]
        public bool isActive { get; set; }
        [Required]
        [Display(Name = "Start At")]
        [DataType(DataType.DateTime)]
        public DateTime StartedAt { get; set; }
        [Display(Name = "End At")]
        [DataType(DataType.DateTime)]
        public DateTime? EndedAt { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }
    }
}
