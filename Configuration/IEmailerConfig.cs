using System;
using System.Collections.Generic;
namespace NetEmailer.Configuration
{
    public interface IEmailerConfig
    {
        Dictionary<string, EmailerSection> EmailerSections { get; }
        EmailerSection this[string name] { get; }
    }
}
