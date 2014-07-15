using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetEmailer.Configuration
{
    public interface IConfigurable
    {
        void Configure(string configFilePath);
    }
}
