namespace MovieTime.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public string UserName { get; set; }
        public int MovieId { get; set; }
        public virtual Movie? Movie { get; set; }
        public double Rate { get; set; }
        public string Comment { get; set; }
        public virtual DateTime Created { get; set;} = DateTime.Now;
    }
}
