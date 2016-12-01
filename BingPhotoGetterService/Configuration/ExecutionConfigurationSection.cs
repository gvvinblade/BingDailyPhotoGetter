using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BingPhotoGetterService.Configuration
{

    public class ExecutionConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("executionInterval")]
        public int ExecutionInterval => (int) base["executionInterval"];

        [ConfigurationProperty("photosCount")]
        public int PhotosCount => (int) base["photosCount"];

        [ConfigurationProperty("path")]
        public string Path => (string) base["path"];
    }
}

