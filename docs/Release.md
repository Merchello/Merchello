# Release Process

## Build

Delete the **Merchello** and **MerchelloProviders** directories from the Merchello.FastTrack.Ui project.

Verify that the minimum Umbraco version is correct around line 44 in the /build/Merchello.proj file.

      <MinUmbracoVersion>7.7.0</MinUmbracoVersion>

Run the build script **/build.cmd**

* Build number should be left empty
* Package version sets the Merchello version for release
* Package suffix should be left blank
* Simulate GitHub tag: true


Successful execution of the build script will produce the deploy artifacts in the /artifacts directory.

Log into the Merchello.FastTrack.Ui project.

Go to developer -> Packages -> Created packages.

Save -> publish -> download zip.  This is the FastTrack starter.

If you don't already have the package created, it's a manual process.  Check the /build/fastrack/[version] directory for a "hint" as to what needs to be added to the package.  Note: datatypes, templates and documenttypes are designated via ID which are database specific (so if your not using the SqlCe db the ids will be different).

## Test

Download latest version of Umbraco and install.

Install Merchello package

Install FastTrack package

Complete an order

Move the order through the back office work flow.

(if any of the above fail - fix issue and start the entire process over).

## Publish

Umbraco packages for Merchello and FastTrack to our.umbraco.org

NuGet packages to NuGet.org


## Cleanup

Merge Merchello-dev into master and create a version tag (trying to get away from creating a branch for each release).

