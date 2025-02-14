﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NumSharp
{
    public partial class NDArray
    {
        /// <summary>
        /// Return the cumulative sum of the elements along a given axis.
        /// </summary>
        /// <param name="axis">Axis along which the cumulative sum is computed. The default (-1) is to compute the cumsum over the flattened array.</param>
        /// <param name="dtype">Type of the returned array and of the accumulator in which the elements are summed. If dtype is not specified, it defaults to the dtype of a, unless a has an integer dtype with a precision less than that of the default platform integer. In that case, the default platform integer is used.</param>
        /// <returns>A new array holding the result is returned unless out is specified, in which case a reference to out is returned. The result has the same size as a, and the same shape as a if axis is not None or a is a 1-d array.</returns>
        /// <remarks>https://docs.scipy.org/doc/numpy/reference/generated/numpy.cumsum.html</remarks>
        public NDArray cumsum(int axis = -1, Type dtype = null)
        {
            return np.cumsum(this, axis, dtype);
        }
    }

    public static partial class np
    {
        /// <summary>
        /// Return the cumulative sum of the elements along a given axis.
        /// </summary>
        /// <param name="arr">Input array.</param>
        /// <param name="axis">Axis along which the cumulative sum is computed. The default (-1) is to compute the cumsum over the flattened array.</param>
        /// <param name="dtype">Type of the returned array and of the accumulator in which the elements are summed. If dtype is not specified, it defaults to the dtype of a, unless a has an integer dtype with a precision less than that of the default platform integer. In that case, the default platform integer is used.</param>
        /// <returns>A new array holding the result is returned unless out is specified, in which case a reference to out is returned. The result has the same size as a, and the same shape as a if axis is not None or a is a 1-d array.</returns>
        /// <remarks>https://docs.scipy.org/doc/numpy/reference/generated/numpy.cumsum.html</remarks>
        public static NDArray cumsum(NDArray arr, int axis = -1, Type dtype = null)
        {
            if (axis > -1)
            {
                // TODO currently no support for multidimensional a
                throw new NotImplementedException();
            }
            NDArray cs = np.zeros(arr.shape[0]);
            cs[0] = arr[0];
            for (int i = 1; i < arr.shape[0]; i++)
            {
                cs[i] = cs[i - 1] + arr[i];
            }
            return cs;
        }
    }
}
