using System.Collections.Generic;

namespace neilhighley_fb.Models
{
    public class FacebookUserModel
    {
        public string Name { get; set; }
        public List<FriendModel> Friends { get; set; }
 
    }

    public class FriendModel
    {
        public string Name { get; set; }
        public string Profile { get; set; }
    }
}