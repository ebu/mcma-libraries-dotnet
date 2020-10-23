using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mcma
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        private static EnvironmentVariables _instance;
        
        public EnvironmentVariables(IDictionary<string, string> variables = null)
        {
            variables ??= System.Environment.GetEnvironmentVariables().Keys.OfType<string>()
                                .ToDictionary(k => k, System.Environment.GetEnvironmentVariable);
            
            Variables = new Dictionary<string, string>(variables, StringComparer.OrdinalIgnoreCase);
            Keys = new ReadOnlyCollection<string>(Variables.Keys.ToList());
        }
        
        private IDictionary<string, string> Variables { get; }

        public IReadOnlyCollection<string> Keys { get; }

        public static EnvironmentVariables Instance => _instance ??= new EnvironmentVariables();

        public string Get(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return Variables.ContainsKey(name) ? Variables[name] : throw new McmaException($"No environment variable found with name '{name}'.");
        }

        public string GetOptional(string name, string defaultValue = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            
            return Variables.ContainsKey(name) ? Variables[name] : defaultValue;
        }
    }
}