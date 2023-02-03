using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutNote.Models
{
    public class SummaryData
    {
        public string? InOut { get; set; }
        public string? Money { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public string? Bank { get; set; }

        public int returnInt(string money)
        {
            int value = int.Parse(money);

            return value;
        }
    }
}
