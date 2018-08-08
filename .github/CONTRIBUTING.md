# How to contribute

When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change. You may open a pull request for current issues without any form of communication beforehand.

Do keep in mind that pull request should be passing current checks in order to be accepted:

* Capable of compiling in both Linux & Windows environments (Currently using CI by Travis & AppVeyor respectively).
  **Pull Requests from outside this repo to the master branch will always fail since secure variables are not being decrypted**
* All unit tests should pass successfully
* Your changes should not reduce code coverage by more than 5% overall or bring the repo to below 70%

Please note we have a code of conduct, please follow it in all your interactions with the project.

## Coding convetions

To maintain a consistent coding style, every pull request should:

* Use StyleCop for consistent code styling
* Use an .editorConfig aware editor
* Provide full documentation on classes & methods affected
* Upon adding a new project, you should [add as link](https://tinyurl.com/yc5rbzhl) the file stylecop.json from the root of this repository and [enable it](https://tinyurl.com/yczleafl). Additionally, add the ruleset from the .config submodule and install StyleCop Analyzers. Make sure to close and reload your project afterwards to enforce provided rules.
