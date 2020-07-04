using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEC.FormControlsSample.Data
{
    public interface IDbRecord<TRecord>
    {
        public int ID { get; }

        /// <summary>
        /// Creates a deep copy of the object
        /// </summary>
        /// <returns></returns>
        public TRecord ShadowCopy();
    }
}
