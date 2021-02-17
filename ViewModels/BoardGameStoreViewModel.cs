using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class BoardGameStoreViewModel
    {
        public BoardGame BoardGame { get; set; }
        public UserBoardGameReview UserBoardGameReview { get; set; }
        public UserCart UserCart { get; set; }
        public IEnumerable<UserReviewGroup> UserReviewGroups { get; set; }
        public string UserId { get; set; }
        public bool UserHasReview { get; set; }
    }
}
