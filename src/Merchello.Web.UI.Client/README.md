## Requirements

1. Install [node](http://nodejs.org/) 
2. From your command prompt go to the /src/Merchello.Web.UI.Client directory
3. Install the Grunt client using 'npm install -g grunt-cli'
4. Install Ruby and make sure it is in your PATH
[ruby for windows](http://rubyinstaller.org/downloads/)
5. From the ruby command prompt: gem install sass (via Ruby)

// this helps
visual studio extension https://visualstudiogallery.msdn.microsoft.com/8e1b4368-4afb-467a-bc13-9650572db708

###build & build-dev
Once you have completed these steps, you need to make sure all the node dependencies are installed.

The build command for release:
npm build

The build command for dev:
npm build-dev

###npm install dependencies
If you run these build commands and you receive an error that a dependency is missing, you need to install it:
npm install xyz (whatever the dependency name is)

###Run Bazaar solution 
The build process above places the files in the /App_Plugins directory in the Bazaar project in the source tree. Test the client-side files you just built by opening and running the Bazaar solution. 