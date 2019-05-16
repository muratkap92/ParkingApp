using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParkingApp.Data.Entity
{
    public class Subscription
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RecipeId { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsPaid { get; set; }
        public virtual Subscriber Subscriber { get; set; }
        public virtual Recipe Recipe { get; set; }

    }
}
