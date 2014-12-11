#Reports Package Instructions


###Steps to build the Reports package:


1) Build merchello src so that merchello.core.dll, merchello.examine.dll, and merchello.web.dll are current. You can do this by opening the solution file. You do not need to build the nuget package or package zip files.

2) Open the reports solution and add a local nuget location that points to the location you build the dlls in step 1.

3) Update the nuget package for merchello. 

4) Build the visual studio solution for the reports package. If there are no errors, go to step 5.

5) from a command line, in the reports/build directory, build the project. This will produce a zip file in the /build directory named something like "MerchelloSalesReports_1.0.0.zip. 

###Steps to install the Reports package:

1) You need to have installed Merchello first. 

2) Now install the reports package zip file in the Umbraco backend. 