# Display Model Builders (Factories)

In most cases Merchello Display Model Builders should implement the two methods:

* createDefault()
* transform(jsonResult) // where transform is used to convert json (or json arrays) into local js Display Models.

Exceptions are Query based models that are typically instantiated in the Merchello back office js

