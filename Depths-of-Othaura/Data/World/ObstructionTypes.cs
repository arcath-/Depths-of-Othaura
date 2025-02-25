using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.World
{
    public enum ObstructionType
    {
        /// <summary>
        /// Can walk through, can see through
        /// </summary>
        Open,

        /// <summary>
        /// Cannot walk through, can see through
        /// </summary>
        MovementBlocked,

        /// <summary>
        /// Can walk through, cannot see through
        /// </summary>
        VisionBlocked,

        /// <summary>
        /// Cannot walk through, Cannot see through
        /// </summary>
        FullyBlocked
    }
}
