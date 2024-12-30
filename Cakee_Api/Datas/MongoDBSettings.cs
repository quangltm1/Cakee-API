namespace Cakee_Api.Datas
{
    public class MongoDBSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? CategoriesCollectionName { get; set; }
        public string? CakesCollectionName { get; set; }
        public string? UsersCollectionName { get; set; }
        public string? BillCollectionName { get; set; }
        public string? BillDetailCollectionName { get; set; }
        public string? ShoppingCartCollectionName { get; set; }
        public string? StatisticalCollectionName { get; set; }
        public string? CakeSizeCollectionName { get; set; }
    }
}
