﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NumSharp
{
    public static partial class np
    {
        public static NDArray ravel(matrix mx)
        {
            var nd = new NDArray(mx.dtype, mx.size);

            switch (mx.dtype.Name)
            {
                case "Double":
                    nd.ReplaceData(mx.Data<double>());
                    break;
                case "Int32":
                    nd.ReplaceData(mx.Data<int>());
                    break;
            }

            return nd;
        }
    }
}
