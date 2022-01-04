using System;
using System.Collections.Generic;
using System.Text;

namespace SBD_PROJEKT2
{
    internal class Organizer
    {
        public string filename = "organizer.dat";
        public int number_of_pages { get; set; }
        public int page_size { get; set; }
        public int idx_page_size { get; set; }
        public int read_counter { get; set; }
        public int write_counter { get; set; } 
    }
}
