using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace neilhighley_fb.Models
{
    public class StatusModel
    {
        public double Id { get; set; }
        public string Message { get; set; }
        public string Updated { get; set; }

        public StatusModel(dynamic obj)
        {
            Id = double.Parse(obj.id.ToString());
            Message = obj.message;
            Updated = obj.updated_time;
        }
        public StatusModel() { }
    }
}