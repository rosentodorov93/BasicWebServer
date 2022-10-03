using BasicWebServer.Server.Attributes;
using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Routing
{
    public static class RoutingTableExtensions
    {
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction)
            where TController : Controller
            => routingTable.MapGet(path, request => controllerFunction(CreateController<TController>(request)));

        public static IRoutingTable MapPost<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction)
            where TController : Controller
            => routingTable.MapPost(path, request => controllerFunction(CreateController<TController>(request)));
        public static IRoutingTable MapControllers(this IRoutingTable routingTable)
        {
            IEnumerable<MethodInfo> controllerActions = GetControllerActions();

            foreach (var controllerAction in controllerActions)
            {
                string controllerName = controllerAction.DeclaringType
                    .Name
                    .Replace(nameof(Controller), string.Empty);
                string actionName = controllerAction.Name;
                string path = $"/{controllerName}/{actionName}";

                var responseFunction = GetResponseFunction(controllerAction);

                Method httpMethod = Method.Get;
                var actionAttribute = controllerAction.GetCustomAttribute<HttpMethodAttribute>();

                if (actionAttribute != null)
                {
                    httpMethod = actionAttribute.HttpMethod;
                }

                routingTable.Map(httpMethod, path, responseFunction);

                MapDefaultRoutes(routingTable, httpMethod, controllerName, actionName, responseFunction);
            }

            return routingTable;
        }


        private static Func<Request, Response> GetResponseFunction(MethodInfo controllerAction)
        {
            return request =>
            {
                if (!UserIsAuthorized(controllerAction, request.Session))
                {
                    return new Response(StatusCode.Unauthorized);
                }

                var controllerInstance = CreateController(controllerAction.DeclaringType, request);

                var parameterValues = GetParameterValues(controllerAction, request);

                return (Response)controllerAction.Invoke(controllerInstance, parameterValues);
            };
        }

        private static bool UserIsAuthorized(MethodInfo controllerAction, Session session)
        {
            var isAuthorzationReqiuired = controllerAction.DeclaringType.GetCustomAttribute<AuthorizeAttribute>()
                ?? controllerAction.GetCustomAttribute<AuthorizeAttribute>();

            if (isAuthorzationReqiuired != null)
            {
                var userIsAuthorized = session.ContainsKey(Session.SessionUserKey)
                    && session[Session.SessionUserKey] != null;

                if (!userIsAuthorized)
                {
                    return false;
                }
            }

            return true;
        }

        private static object[] GetParameterValues(MethodInfo controllerAction, Request reqest)
        {
            var actionParameters = controllerAction
                .GetParameters()
                .Select(p => new
                {
                    p.Name,
                    p.ParameterType
                })
                .ToArray();

            var parameterValues = new object[actionParameters.Length];

            for (int i = 0; i < parameterValues.Length; i++)
            {
                var parameter = actionParameters[i];

                if (parameter.ParameterType.IsPrimitive 
                    || parameter.ParameterType == typeof(string))
                {
                    var parameterValue = reqest.GetValue(parameter.Name);
                    parameterValues[i] = Convert.ChangeType(parameterValue, parameter.ParameterType);
                }
                else
                {
                    var parameterInstance = Activator.CreateInstance(parameter.ParameterType);
                    var parameterProperties = parameter.ParameterType.GetProperties();

                    foreach (var property in parameterProperties)
                    {
                        var propertyValue = reqest.GetValue(property.Name);
                        property.SetValue(parameterInstance,
                            Convert.ChangeType(parameterValues, property.PropertyType));
                    }

                    parameterValues[i] = parameterInstance;
                }
            }

            return parameterValues;
        }
        private static IEnumerable<MethodInfo> GetControllerActions()
            => Assembly
            .GetEntryAssembly()
            .GetExportedTypes()
            .Where(t => t.IsAbstract == false)
            .Where(t => t.IsAssignableTo(typeof(Controller)))
            .Where(t => t.Name.EndsWith(nameof(Controller)))
            .SelectMany(t => t
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.ReturnType.IsAssignableTo(typeof(Response)))
            ).ToList();

        private static TController CreateController<TController>(Request request)
            => (TController)Activator.CreateInstance(typeof(TController), new[] { request });
        private static Controller CreateController(Type controllerType, Request request)
        {
            var controller = (Controller)Request.ServiceCollection.CreateInstance(controllerType);

            controllerType
                .GetProperty("Request", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(controller, request);

            return controller;
        }
        private static string GetValue(this Request request, string? name)
            => request.Query.GetValueOrDefault(name) ??
                request.Form.GetValueOrDefault(name);
        private static void MapDefaultRoutes(IRoutingTable routingTable, Method httpMethod, string controllerName, string actionName, Func<Request, Response> responseFunction)
        {
            const string defaultControllerName = "Home";
            const string defaultActionName = "Index";

            if (controllerName == defaultControllerName)
            {
                routingTable.Map(httpMethod, $"/{actionName}", responseFunction);

                if (actionName == defaultActionName)
                {
                    routingTable.Map(httpMethod, $"/", responseFunction);
                }
            }
        }
    }
}
