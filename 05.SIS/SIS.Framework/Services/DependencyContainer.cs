namespace SIS.Framework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Contracts;

    public class DependencyContainer : IDependencyContainer
    {
        private const string InvalidTypeMessage = "Type {0} cannot be instantiated.";

        private readonly IDictionary<Type, Type> dependancyDictionary;

        public DependencyContainer()
        {
            this.dependancyDictionary = new Dictionary<Type, Type>();
        }

        private Type this[Type key] => this.dependancyDictionary.ContainsKey(key) ? this.dependancyDictionary[key] : null;

        public void RegisterDependency<TSource, TDestination>()
        {
            this.dependancyDictionary[typeof(TSource)] = typeof(TDestination);
        }

        public T CreateInstance<T>()
        {
            return (T)this.CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            Type instanceType = this[type] ?? type;

            if (instanceType.IsInterface || instanceType.IsAbstract)
            {
                throw new InvalidOperationException(string.Format(InvalidTypeMessage, instanceType.FullName));
            }

            ConstructorInfo constructor = instanceType.GetConstructors().OrderBy(c => c.GetParameters().Length).First();

            ParameterInfo[] constructorParameters = constructor.GetParameters();

            object[] constructorParameterObjects = new object[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                constructorParameterObjects[i] = this.CreateInstance(constructorParameters[i].ParameterType);
            }

            return constructor.Invoke(constructorParameterObjects);
        }
    }
}
