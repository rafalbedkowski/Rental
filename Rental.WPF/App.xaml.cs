using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Rental.Bll.DalContext;
using Rental.BLL.IDalContext;
using Rental.BLL.IRepository;
using Rental.BLL.Repository;
using Rental.WPF.Windows;
using Unity;
using Unity.Injection;

namespace Rental.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RegisterTypes(Container);

            var window = Container.Resolve<MainWindow>();
            window.Show();
        }

        private static readonly Lazy<IUnityContainer> _container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer Container => _container.Value;

        public static void RegisterTypes(IUnityContainer container)
        {
            var context = new RentalContext();
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<IToolRepository, ToolRepository>();
            container.RegisterType<ITransactionRepository, TransactionRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IRentalContext, RentalContext>();

            container.RegisterType<MainWindow>(new InjectionConstructor(container, new TransactionRepository(context)));
            container.RegisterType<CompanyWindow>(new InjectionConstructor(container, new CompanyRepository(context)));
            container.RegisterType<CompaniesWindow>(new InjectionConstructor(container, new CompanyRepository(context)));
            container.RegisterType<UserWindow>(new InjectionConstructor(container, new UserRepository(context)));
            container.RegisterType<UsersWindow>(new InjectionConstructor(container, new UserRepository(context)));
            container.RegisterType<ToolWindow>(new InjectionConstructor(container, new ToolRepository(context)));
            container.RegisterType<ToolsWindow>(new InjectionConstructor(container, new ToolRepository(context)));
            container.RegisterType<ToolsForTransactionWindow>(new InjectionConstructor(container, new ToolRepository(context)));
            container.RegisterType<TransactionWindow>(new InjectionConstructor(container, new TransactionRepository(context)));
            container.RegisterType<StatisticWindow>(new InjectionConstructor(container, new TransactionRepository(context)));
        }
    }
}
