using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingApp.Data.Entity
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string License { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime InsertDate { get; set; }

        public virtual City City { get; set; }
        public virtual District District{ get; set; }

    }
}
