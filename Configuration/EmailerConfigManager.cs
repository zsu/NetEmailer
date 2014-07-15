#region License
// Copyright 2006 Zhicheng Su
//
//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Threading;

namespace NetEmailer.Configuration
{
    public static class EmailerConfigManager
    {
        #region Fields

        private static string _configFile = null;
        private static IEmailerConfig _config = null;
        private static readonly object _syncRoot = new object();

        #endregion

        #region Methods

        public static void Clear()
        {
            lock (_syncRoot)
            {
                _config = null;
                _configFile = null;
            }
        }

        public static IEmailerConfig GetConfig(string configFilePath)
        {
            if (_config != null && !string.IsNullOrEmpty(_configFile) && _configFile.Trim().ToLower() == configFilePath.Trim().ToLower())
                return _config;
            lock (_syncRoot)
            {
                if (_config == null || string.IsNullOrEmpty(_configFile) || _configFile.Trim().ToLower() != configFilePath.Trim().ToLower())
                {
                    _config = new EmailerConfig(configFilePath);
                    _configFile = configFilePath;
                }
            }
            return _config;
        }

        #endregion
    }


}
