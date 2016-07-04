======================
REMOTE PACKAGE MANAGER
======================

WHAT IT IS
----------

Remote Package Manager is a nice 'n' easy tool to help those who are struggling to
implement Unity's AssetBundles into their game!

It helps them with building, loading, version and dependency management, testing and
even uploading AssetBundles directly from the editor!


HOW IT WORKS
------------

In Unity 5, Remote Package Manager integrates on top of the new Asset Bundle workflow
providing extra features such as uploading them to your server and managing multiple platforms.

From "Window/Remote Package Manager" you can do almost all the bundle work.

There're two sections: "Build" and "Upload".

Buiild section:
Create, edit and build packages.

Upload section:
Upload selected packages to Amazon S3 or FTP.

In order to create a new remote package, simply select an asset in project view and assign an
Asset Bundle to it. This can be done using Unity5's workflow (where you can find more info about
it here http://docs.unity3d.com/Manual/BuildingAssetBundles5x.html) or using the RPM window.

When using the RPM editor, when an asset is selected, it shows up in "Build section" > "AssetBundle" > "Selected".
From there, you can change its package or create a new one.

For example, if you want to create a package called "mycube" with the "CubePrefab" inside it, first
select the prefab in the project view, then click the "Package" drop down menu and select "New...".
From there, you can new type the new package name ("mycube"). When you press Enter it'll create a new
package with that name and "CubePrefab" inside it.

Just check the examples out to see it in practice!


SETTING UP
----------

Place a GameObject with the RemotePackageManager script attached to it in your first scene
so you can start to operate the AssetBundles.

First you have to set in the script a "Base URL" from where your packages will be downloaded.
For instance, if you're going to download an "AudioPackage.unity3d" from your website in the
url: "http://www.mygame.com/AssetBundles/webplayer/AudioPackage.unity3d", your Base URL should be:
"http://www.mygame.com/AssetBundles/"

* Notice that the "webplayer" refers to which platform the package was build for. It is possible
to have the same package build for several platforms when they can't share the same Asset Bundle.
For instance, an Asset Bundle built for Android should not work on the iOS. Therefore, you should
build the same package multiple times.

In RPM, you can change this settings in the dropdown menu on the top right of the "Build" section.


Also, if you're already working with FTP or Amazon Web Service's S3 storage service, it's
also possible to automatically upload your AssetBundles there. More on that later! :D


DEFINING A PACKAGE'S ASSETS (AssetBundles)
--------------------------------------

A package's content is defined by selecting its contents (assets in the project view) and then
choosing their packages in the Remote Package Manager window.

Once you define which assets are going to be bundled together, we're able to build them!


DEFINING PACKAGE DEPENDECY (Package Hierarchy)
----------------------------------------------

With the new Unity5 workflow, it is *not* needed anymore to define dependencies between packages!


BUILDING PACKAGES (AssetBundles)
--------------------------------

The way to build your packages is very straightforward. There is this big button "Build Packages"
in the RPM window. Just push it and Unity will incrementally build your bundles. Also, you can force
it to rebuild all your packages with the "Force Rebuild".

After you build your packages, they will be automatically be exported to the folder
"RemotePackageManager/AssetBundles_RecentlyBuilt/".
This way, you can easily grab the folder structure and upload it anywhere.

Back to the "Window/Remote Package Manager" window:

"Build Target" (dropdown on the top right corner) defines in what platform the packages are going to be
used in. Generally, this just means choosing your game's current build platform. Simply open Unity's
Build Settings to check it out.

"Build Packages" will buid all packages incrementally.

"Force Rebuild" will force rebuild all packages.


UPLOADING TO AMAZON S3
----------------------

In the "Window/Remote Package Manager" "Upload" section:

Inside "AmazonS3Uploader" you will find fields to put your Amazon AWS's credentials so we can upload
there automatically for you.

Remember to select which packages you wish to upload to your server by checking them up.

* Don't Worry! *
RemotePackageManager will store your credentials within the EditorPrefs so it's saved locally
and there's no risk of commiting your credentials to the cloud!

* Note for WebPlayer Games *
Because of the way Unity WebPlayer's security works, it's not possible to upload to Amazon (even from
Editor) while the active BuildTarget is WebPlayer. So, when you hit the amazon upload button while in a
WebPlayer build, RemotePackageManager will switch to "Standalone Windows" BuildTarget before the upload
and then switch back to WebPlayer. If you don't want this to happen automatically, just manually switch
to another build target before upload!

AmazonS3Uploader Fields - Check your AWS S3 console at "https://console.aws.amazon.com/s3/home"

- AccessKey: You can find this in the Amazon AWS console top bar > "Your Name" menu > "Security Credentials" > "Access Keys"
- SecretKey: You can find this in the Amazon AWS console top bar > "Your Name" menu > "Security Credentials" > "Access Keys"
- Region:    The Amazon S3 region where your bucket is (usually it's just "s3")
- Bucket:    Your Amazon S3 bucket name where you're going to upload your AssetBundles


UPLOADING BY FTP
----------------

Inside "FtpUploader" you will find fields to put your FTP credentials so we can upload
there automatically for you.

Remember to select which packages you wish to upload to your server by checking them up.

* Don't Worry! *
RemotePackageManager will store your credentials within the EditorPrefs so it's saved locally
and there's no risk of commiting your credentials to the cloud!

* Note for WebPlayer Games *
Because of the way Unity WebPlayer's security works, it's not possible to upload to the cloud (even from
Editor) while the active BuildTarget is WebPlayer. So, when you hit the ftp upload button while in a
WebPlayer build, RemotePackageManager will switch to "Standalone Windows" BuildTarget before the upload
and then switch back to WebPlayer. If you don't want this to happen automatically, just manually switch
to another build target before upload!

FtpUploader Fields - More on FTP at "http://kb.iu.edu/data/aerg.html"

- Url:      The Url to where you're going to upload your files (usually, you'll want to upload to the "public_html" folder).
- Username: Your username used to login into your ftp account.
- Password: Your password used to login into your ftp account.


CONTACT INFO
------------

This asset was made by Matheus Lessa Rodrigues, a programmer @ BitCake Studio ("http://bitcakestudio.com")

If you have any questions, please feel free to contact him at "matheus@bitcakestudio.com"





*******************************
Thank you for supporting us! :)
*******************************
