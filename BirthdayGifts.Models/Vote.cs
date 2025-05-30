using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayGifts.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        [Required]
        [Display(Name = "Vote Session ID")]
        public int VoteSessionId { get; set; }
        [Required]
        [Display(Name = "Gift ID")]
        public int GiftId { get; set; }
        [Required]
        [Display(Name = "Voter ID")]
        public int VoterId { get; set; }

    }
}
