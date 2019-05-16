using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Data.Entity
{
    public class District
    {
        public District()
        {

        }
        public int Id { get; set; }
       public int City_Id { get; set; }
        public string Name { get; set; }
     
 
    }
}
