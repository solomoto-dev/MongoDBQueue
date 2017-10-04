namespace MongoQueue.Core
{
    public interface IRegistrator
    {
        void Register<TAbst, TImpl>() where TImpl : class, TAbst;
        void Register<TImpl>() where TImpl : class;
        void RegisterSingleton<TAbst, TImpl>() where TImpl : class, TAbst;
        void RegisterInstance<T>(T instance) where T : class;
    }
}