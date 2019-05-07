using HOPU.Implement;
using HOPU.Services;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;

namespace HOPU
{
    public class BootStrapper
    {
        /// <summary>
        /// 获取容器-注册依赖关系
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        /// <summary>
        /// 加载容器
        /// </summary>
        /// <returns></returns>
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            RegisterTypes(container);

            return container;
        }

        /// <summary>
        /// 实施依赖注入
        /// </summary>
        /// <param name="container"></param>
        private static void RegisterTypes(UnityContainer container)
        {
            //依赖关系可以选择代码形式，也可以用配置文件的形式
            //UnityConfigurationSection config = (UnityConfigurationSection)ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            //加载到容器
            //config.Configure(container, "MyContainer");
            container.RegisterType<IBTTable, ImpBTTable>();
            container.RegisterType<ICourse, ImpCourse>();
            container.RegisterType<ITopic, ImpTopic>();
            container.RegisterType<ITypeinfo, ImpTypeInfo>();
            container.RegisterType<IUniteTest, ImpUniteTest>();
            container.RegisterType<IUniteTestScore, ImpUniteTestScore>();
            container.RegisterType<IUniteTestInfo, ImpUniteTestInfo>();
        }
    }
}