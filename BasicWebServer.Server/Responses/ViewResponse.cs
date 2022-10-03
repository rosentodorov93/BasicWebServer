using BasicWebServer.Server.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class ViewResponse : ContentResponse
    {
        private const char PathSeparator = '/';
        public ViewResponse(string viewName, string contollerName, object model = null)
            : base("", ContentType.Html)
        {
            if (!viewName.Contains(PathSeparator))
            {
                viewName = contollerName + PathSeparator + viewName;
            }

            var viewPath = Path.GetFullPath($"./Views/" + viewName.TrimStart(PathSeparator) + ".cshtml");

            var viewContent = File.ReadAllText(viewPath);

            var (layoutPath, layoutExists) = FindLayout();

            if (layoutExists)
            {
                var layoutContent = File.ReadAllText(layoutPath);
                viewContent = layoutContent.Replace("{{RenderBody}}", viewContent);
            }

            if (model != null)
            {
                if (model is IEnumerable)
                {
                    viewContent = PopulateEnumerableModel(viewContent, model);
                }
                else
                {
                    viewContent = PopulateModel(viewContent, model);
                }
            }
            this.Body = viewContent;
        }

        private (string, bool) FindLayout()
        {
            string layoutPath = null;
            bool layoutExists = false;

            layoutPath = Path.GetFullPath("./Views/Layout.cshtml");

            if (File.Exists(layoutPath))
            {
                layoutExists = true;
            }
            else
            {
                layoutPath = Path.GetFullPath("./Views/Shared/_Layout.cshtml");

                if (File.Exists(layoutPath))
                {
                    layoutExists = true;
                }
            }

            return (layoutPath, layoutExists);
        }

        private string PopulateEnumerableModel(string viewContent, object model)
        {
            var result = new StringBuilder();

            var viewContentLines = viewContent.Split(Environment.NewLine).Select(l => l.Trim());

            var inLoop = false;
            StringBuilder loopContent = null;

            foreach (var line in viewContentLines)
            {
                if (line.StartsWith("{{foreach}}"))
                {
                    inLoop = true;
                    continue;
                }

                if (inLoop)
                {
                    if (line.StartsWith("{"))
                    {
                        loopContent = new StringBuilder();
                    }
                    else if (line.StartsWith("}"))
                    {
                        var loopTemplate = loopContent.ToString();

                        foreach (var item in (IEnumerable)model)
                        {
                            var loopResult = PopulateModel(loopTemplate, item);

                            result.AppendLine(loopResult);
                        }

                        inLoop = false;
                    }
                    else
                    {
                        loopContent.AppendLine(line);
                    }

                    continue;
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }

        private string PopulateModel(string viewContent, object model)
        {
            var data = model.GetType()
                .GetProperties()
                .Select(pr => new
                {
                    pr.Name,
                    Value = pr.GetValue(model)
                });
            foreach (var entry in data)
            {
                const string openingBrackets = "{{";
                const string closingBrackets = "}}";

                viewContent = viewContent.Replace($"{openingBrackets}{entry.Name}{closingBrackets}", entry.Value.ToString());
            }

            return viewContent;
        }
    }
}
