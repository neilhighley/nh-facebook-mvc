using System.Collections.Generic;

namespace neilhighley_fb.Models
{
    public class FacebookUserModel
    {
        public string Name { get; set; }
        public List<FriendModel> Friends { get; set; }
        public string Email { get; set; }
        public List<StatusModel> Statuses { get; set; }
    }

    public class FriendModel
    {
        public string Name { get; set; }
        public string Profile { get; set; }
        public string Image { get; set; }
    }
}