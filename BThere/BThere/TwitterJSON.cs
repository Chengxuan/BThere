using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BThere
{
    public class Hashtag
    {
        public string text { get; set; }
        public List<int> indices { get; set; }
    }

    public class Entities
    {
        public List<Hashtag> hashtags { get; set; }
        public List<object> urls { get; set; }
        public List<object> user_mentions { get; set; }
    }

    public class Metadata
    {
        public string result_type { get; set; }
    }

    public class tResult
    {
        public string created_at { get; set; }
        public Entities entities { get; set; }
        public string from_user { get; set; }
        public int from_user_id { get; set; }
        public string from_user_id_str { get; set; }
        public string from_user_name { get; set; }
        public object geo { get; set; }
        public object id { get; set; }
        public string id_str { get; set; }
        public string iso_language_code { get; set; }
        public Metadata metadata { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string source { get; set; }
        public string text { get; set; }
        public object to_user { get; set; }
        public int to_user_id { get; set; }
        public string to_user_id_str { get; set; }
        public object to_user_name { get; set; }
    }

    public class RootObject1
    {
        public double completed_in { get; set; }
        public long max_id { get; set; }
        public string max_id_str { get; set; }
        public string next_page { get; set; }
        public int page { get; set; }
        public string query { get; set; }
        public string refresh_url { get; set; }
        public List<tResult> results { get; set; }
        public int results_per_page { get; set; }
        public int since_id { get; set; }
        public string since_id_str { get; set; }
    }


}

