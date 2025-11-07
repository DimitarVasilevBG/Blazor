namespace ClothyShop.Settings
{
    public class AppSettings
    {
        public MongoDBSettings MongoDB { get; set; }
        public RedisSettings Redis { get; set; }
    }


    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; }
    }
}

