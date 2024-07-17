using OMS.Data.Entites.System;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.Utilites
{
    public static class FunctionScanner
    {
        public static IEnumerable<tbFunctions> GetControllerAndActionNames()
        {
            var functions = new List<tbFunctions>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }

                var controllers = types
                    .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                            .Where(m => m.IsPublic && !m.IsDefined(typeof(NonActionAttribute)));

                    foreach (var action in actions)
                    {
                        functions.Add(new tbFunctions
                        {
                            FunctionName = action.Name,
                        });
                    }
                }
            }
            return functions;
        }
    }
}
