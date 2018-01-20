using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class MainClass
    {
        public virtual long id { get; set; }
        public virtual long Firstclassid { get; set; }
        public virtual IList<FirstClass> samp { get; set; }
    }
    public class FirstClass
    {
        public virtual long Firstclassid { get; set; }
        public virtual long Secondclassid { get; set; }
        public virtual IList<SecondClass> samp { get; set; }
    }
    public class SecondClass
    {
        public virtual long Secondclassid { get; set; }
        public virtual string Name { get; set; }
    }
   
}
