using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.Models
{
    public class InOutModel
    {
        public string? InOut { get; set; }
        public int Money { get; set; }
        public string? Kind { get; set; }
        public string? UseDate { get; set; }
        public string? Bank { get; set; }
        public string? Card { get; set; }
        public string? Use { get; set; }
    }
}
