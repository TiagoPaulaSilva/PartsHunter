using System;
using System.Windows.Forms;
using PartsHunter.Repositories;
using PartsHunter.Repositories.Interfaces;
using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace PartsHunter
{
    public class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Container container = Container();
            Application.Run(container.GetInstance<Form1>());
        }

        private static Container Container()
        {
            // Create the container as usual.
            Container container = new Container();

            // Register your types, for instance:

            container.Register<IFirebaseRepositorie, FirebaseRepositorie>();
            container.Register<ILocalRepositorie, LocalRepositorie>();

            AutoRegisterWindowsForms(container);

            container.Verify();

            return container;
        }

        private static void AutoRegisterWindowsForms(Container container)
        {
            System.Collections.Generic.IEnumerable<Type> types = container.GetTypesToRegister<Form>(typeof(Program).Assembly);

            foreach (Type type in types)
            {
                Registration registration =
                    Lifestyle.Transient.CreateRegistration(type, container);

                registration.SuppressDiagnosticWarning(
                    DiagnosticType.DisposableTransientComponent,
                    "Forms should be disposed by app code; not by the container.");

                container.AddRegistration(type, registration);
            }
        }
    }
}
