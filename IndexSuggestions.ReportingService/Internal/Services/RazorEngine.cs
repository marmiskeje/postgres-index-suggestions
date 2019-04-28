using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.ReportingService
{
    internal class RazorEngine : IRazorEngine
    {
        private readonly IRazorEngineService engine = null;
        private bool isDisposed = false;
        public RazorEngine()
        {
            engine = RazorEngineService.Create();
        }

        public string Transform<T>(string template, T model)
        {
            var templateKey = template.GetHashCode().ToString();
            if (!engine.IsTemplateCached(templateKey, typeof(T)))
            {
                engine.AddTemplate(templateKey, template);
                engine.Compile(templateKey, typeof(T));
            }
            return engine.Run(templateKey, typeof(T), model);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    engine.Dispose();
                }
                isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RazorEngine()
        {
            Dispose(false);
        }
    }
}
