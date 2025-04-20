using Jint;

namespace ELE.MockApi.Core.Service
{
    public class ScriptEvaluator
    {
        public ILogger _logger { get; private set; }
        public Engine Engine { get; private set; }

        public ScriptEvaluator(ILogger logger, Engine engine)
        {
            _logger = logger;
            Engine = engine;
        }


    }
}
