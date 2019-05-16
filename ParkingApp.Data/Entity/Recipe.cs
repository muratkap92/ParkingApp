using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingApp.Data.Entity
{
    public class Recipe
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public int Type { get; set; }
        public int MinimumValue { get; set; }
        public int MaximumValue { get; set; }
        public int Cost { get; set; }
        public bool Status { get; set; }
        public DateTime InsertDate { get; set; }

    }
}
