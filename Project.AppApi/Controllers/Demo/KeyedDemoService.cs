namespace Project.AppApi.Controllers.Demo
{
    /// <summary>
    /// Keyed DI 示范接口（.NET 8 特性：同一接口可按 key 注册不同实现）
    /// </summary>
    public interface IKeyedDemoService
    {
        string GetName();
    }

    /// <summary>
    /// Keyed DI 示范实现 A
    /// </summary>
    public class KeyedServiceA : IKeyedDemoService
    {
        public string GetName() => "Service A";
    }

    /// <summary>
    /// Keyed DI 示范实现 B
    /// </summary>
    public class KeyedServiceB : IKeyedDemoService
    {
        public string GetName() => "Service B";
    }
}
