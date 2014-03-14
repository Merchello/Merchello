# Merchello

A new high performance, open source eCommerce package for Umbraco being built with store owners and designers in mind.

## Building the Merchello Package

Building the Merchello Package can be accomplished by executing the \Build\Build.bat batch script.

Execute the \Build\Build.bat file - this should create a couple of new directories \Build\TEMP\Artifacts\

The Build.bat file should generate the following files in the \Build\TEMP\Artifacts\ folder.

* Merchello.AllBinaries.[VERSION].zip - Merchello binaries
* Merchello.Core.[VERSION].nupkg - Merchello Core NuGet package 
* Merchello_[VERSION].zip - The Merchello Umbraco Package 


## Installing and Building SASS

To install and build the [SASS](http://sass-lang.com/) scripts, you must have Ruby installed via the following steps:

* Download Ruby for Windows at [http://www.rubyinstaller.org/](http://www.rubyinstaller.org/)
* Set your Path in your Windows Environment to include Ruby:
	- From Windows Explorer, right click My Computer and click Properties
	- Select Advanced System Settings on the left (you must have administrative priviledges to do this)
	- In the System Properties window, click on the Advanced tab
	- Click the Environment Variables button on the bottom right
	- In this window, under System Variables, highlight the Path variable and click the Edit button
	- At the very end of the path add a semicolon ( ; ) and then add the path to your Ruby\bin directory (Note: this may have a version at the end such as Ruby200\bin. Check your directory structure to make sure it's entered correctly). It should look something like this: ;C:\Ruby200\bin
* Open your command prompt and install SASS by typing the following: gem install sass
* Double-check that SASS installed correctly by typing the following: sass -v

SASS must be used for the CSS files in the admin - if you make changes to the CSS files and not the SASS file (merchello.scss), then it will be overridden the next time the SASS file is updated.

SASS will compile automatically when you are editing it, but you must run the sass.bat file while making your edits. Every time you save, the CSS file will be compiled by the new version of your SASS file, and it will also show you any errors in your code as you work.