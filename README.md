# NumSharp

NumPy port in C# .NET Standard 2.0

[![Join the chat at https://gitter.im/publiclab/publiclab](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/numsharp/Lobby)
![NumSharp](https://ci.appveyor.com/api/projects/status/bmaauxd9rx5lsq9i?svg=true)

Is it difficult to translate python machine learning code into C#? Because too many functions can’t be found in the corresponding code in the .Net SDK. NumSharp is the C# version of NumPy, which is as consistent as possible with the NumPy programming interface, including function names and parameter locations. By introducing the NumSharp tool library, you can easily convert from python code to C# code. Here is a comparison code between NumSharp and NumPy (left is python, right is C#):

![comparision](docs/_static/screenshots/python-csharp-comparision.png)

NumSharp has implemented the arange, array, max, min, reshape, normalize, unique interfaces. More and more interfaces will be added to the library gradually. If you want to use .NET to get started with machine learning, NumSharp will be your best tool library.

## Implemented APIs
* NdArray
  * ARange
  * ArgMax
  * Array
  * AsMatrix
  * Convolve
  * Delete
  * Divide
  * Dot
  * HStack
  * Max
  * Mean
  * Min
  * Minus
  * Normalize
  * Random
  * ReShape
  * Std
  * Sum
  * Unique
  * Zeros
  
* NdArrayRandom
  * Permutation
  * Shuffle

* Matrix

## Install NumSharp in NuGet
```
Install-Package NumSharp -Version 0.1.0
```

# How to make docs
```
$ pip install sphinx
$ pip install recommonmark
$ cd docs
$ make html
```
Online [documents](https://numsharp.readthedocs.io)

NumSharp is referenced by:
* [Bigtree.MachineLearning](https://github.com/Oceania2018/Bigtree.MachineLearning)

Welcome to fork and pull request to add more APIs, and make reference list longer.