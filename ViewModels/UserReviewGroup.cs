using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class UserReviewGroup
    {
        public string Id { get; set; }
        public int BoardGameID { get; set; }
        public string Name { get; set; }
        public string UserReview { get; set; }
        public string UserAvatarFilePath { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }
    }
}
