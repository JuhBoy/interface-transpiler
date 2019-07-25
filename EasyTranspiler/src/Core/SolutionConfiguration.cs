using System;
using System.Collections.Generic;
using System.Text;

namespace EasyTranspiler.src
{
    public enum LinkStrategy
    {
        Link,
        NoLink
    }

    public class SolutionConfiguration
    {
        public SolutionConfiguration() { }

        public LinkStrategy LinkStrategy { get; set; }
    }
}
