using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllTechnologyWpf.Models.SerialisibleModels
{
    public class XMLUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int? LiderId { get; set; }
    }
}
