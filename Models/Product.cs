namespace ReviewService.Models
{
       public class Product
        {
           
            public int Id { get; set; }

           
            public int CategoryId { get; set; }


           
            public double Price { get; set; }


            public string Description { get; set; }

           
            public string Address { get; set; }

            public DateTime CreatedAt { get; set; }


        
    }
}
