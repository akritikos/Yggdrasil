# Yggdrasil

[![Build status](https://ci.appveyor.com/api/projects/status/yy6b8oq69lsejj11?svg=true)](https://ci.appveyor.com/project/akritikos/yggdrasil)
[![codecov](https://codecov.io/gh/akritikos/Yggdrasil/branch/master/graph/badge.svg)](https://codecov.io/gh/akritikos/Yggdrasil)
[![Coverage Status](https://coveralls.io/repos/github/akritikos/Yggdrasil/badge.svg?branch=master)](https://coveralls.io/github/akritikos/Yggdrasil?branch=master)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

[Yggdrasil](https://en.wikipedia.org/wiki/Yggdrasil) (pronounced [ˈyɡːˌdrasilː]) is an immense mythical tree that connects the nine worlds in Norse cosmology.
***

## Thesis Project

Kritikos.Yggdrasil is meant to be a modernization of the venerable [GaTree](http://www.gatree.com) that is written in C++. The main goal apart from updating the current implementation is to provide a library that can be easily used in projects requiring machine learning and, as a stretch goal, creating a cloud platform leveraging current technology.

Guidelines for pull requests:

* Use StyleCop for consistent code styling
* Provide full documentation on classes & methods
* Your PR should be able to be build cross-platform and pass all supplied unit tests (AppVeyor CI & XUnit)
* Upon adding a new project, you should [add as link](https://tinyurl.com/yc5rbzhl) the file stylecop.json from the .config submodule and [enable it](https://tinyurl.com/yczleafl). Additionally, add the ruleset from the .config submodule to conform to the same standards. Make sure to close and reload your project afterwards.

***
Subprojects consist of:

### Yggdrasil.Helheim

Contains unit tests and terror, steer clear to avoid dishonorable death.

### Yggdrasil.Nidhogg

Project meant for local debugging, add classes you need inside the Debug folder and implement IDebug interface in order to load them automatically using reflection.
