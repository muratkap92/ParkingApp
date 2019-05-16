using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParkingApp.Data.Entity
{
    public class Control
    {
        public int Id { get; set; }
        public string License { get; set; }
        public DateTime EnterDate { get; set; }
        
        public DateTime? ExitDate { get; set; }
        public int? Time { get; set; }
        public int? Cost { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
