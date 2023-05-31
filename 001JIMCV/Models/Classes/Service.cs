﻿using _001JIMCV.Models.Classes.Enum;
using System.Collections.Generic;

namespace _001JIMCV.Models.Classes
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Type { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public float Price { get; set; }
        public string State { get; set; }

        public int IdUser { get; set; }
        public virtual User UserProvider { get; set; }
    }
}
