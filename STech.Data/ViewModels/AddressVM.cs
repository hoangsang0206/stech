using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class AddressVM
    {
        public class City
        {
            public string name { get; set; } = null!;
            public string type { get; set; } = null!;
            public string slug { get; set; } = null!;
            public string name_with_type { get; set; } = null!;
            public string code { get; set; } = null!;
        }

        public class District
        {
            public string name { get; set; } = null!;
            public string type { get; set; } = null!;
            public string slug { get; set; } = null!;
            public string name_with_type { get; set; } = null!;
            public string path { get; set; } = null!;
            public string path_with_type { get; set; } = null!;
            public string code { get; set; } = null!;
            public string parent_code { get; set; } = null!;
        }

        public class Ward
        {
            public string name { get; set; } = null!;
            public string type { get; set; } = null!;
            public string slug { get; set; } = null!;
            public string name_with_type { get; set; } = null!;
            public string path { get; set; } = null!;
            public string path_with_type { get; set; } = null!;
            public string code { get; set; } = null!;
            public string parent_code { get; set; } = null!;
        }
    }
}
