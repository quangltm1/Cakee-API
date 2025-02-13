namespace Cakee.Models
{
    public class DatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? CategoriesCollectionName { get; set; }
        public string? CakesCollectionName { get; set; }
        public string? UsersCollectionName { get; set; }
        public string? RefreshTokensCollectionName { get; set; }
        public string? CakeSizesCollectionName { get; set; }
        public string? BillsCollectionName { get; set; }
        public string? AcessorysCollectionName { get; set; }
        public string? ShoppingCartsCollectionName { get; set; }
        public string? StatisticalsCollectionName { get; set; }
    }
}
