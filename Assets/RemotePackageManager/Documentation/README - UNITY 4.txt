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

From "Window/Remote Package Manager" you can do almost all the bundle work.
There're two tabs: "Packages" and "Upload".

Packages tab:
Create, edit, build and organize package dependencies.

Upload tab:
Upload recently built packages to Amazon S3 or FTP.


In order to create a new remote package, go to "Packages" tab and type the new package name
into the search field. If the name is unique, there should appear a "Create Package" button.
Click it and there you go! It will be created inside the folder "RemotePackageManager/AssetBundles"
where all remote packages are located.

For example, if you want to create a package called "MyCube", type in "MyCube" in the search
field and click "Create Package". It will create a folder "MyCube" inside "RemotePackageManager/AssetBundles".

Inside it, you'll find "MyCube-settings.asset". Click it (open the Inspector) and there you'll
find more options regarding this new package.

Just check the examples out to see them in practice!


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

In RPM, you can change this settings in the dropdown menu on the top left of the "Packages" tab.


Also, if you're already working with FTP or Amazon Web Service's S3 storage service, it's
also possible to automatically upload your AssetBundles there. More on that later! :D


DEFINING A PACKAGE'S ASSETS (AssetBundles)
--------------------------------------

Once you create a new package, open it on the inspector.
There you will find the field: "Assets" which is a list.
Simply drag your desired assets to it and RemotePackageManager will do the rest for you.

Once you define which assets are going to be bundled together, we're able to build them!


DEFINING PACKAGE DEPENDECY (Package Hierarchy)
----------------------------------------------

In the "Window/Remote Package Manager" window in the "Packages" tab you will see the
package hierarchy. By drag'n'drop, you can reposition them to construct the build tree.

Let's say we got package A and package B. Package B should be dependent on package A.
It means that A and B need to share some asset dependencies (both of them depend on
the same texture for instance).

* More on package dependency: "http://docs.unity3d.com/Manual/managingassetdependencies.html"

So, we need RemotePackageManger to know that B depends on A. For that, we drag package B
inside package A. It will now show as A's child.

This way, the first time you try to download package B, it will first download package A
if it's not already downloaded! Everything handled automatically.


BUILDING PACKAGES (AssetBundles)
--------------------------------

There're several ways to build a package. The most straightforward way is to click the
"Build" button (upper left corner) in the package settings inspector window. It will
only build that package (and its dependencies if any).

After you build any package, it will be automatically be exported to the folder
"RemotePackageManager/AssetBundles_RecentlyBuilt/".
This is so that you can easily grab the folder structure and upload it anywhere.


Back to the "Window/Remote Package Manager" window in the "Packages" tab:

"Build Target" (dropdown on the top left corner) defines in what platform the packages are going to be
used in. Generally, this just means choosing your game's current build platform. Simply open Unity's
Build Settings to check them out.

"Build Selected" will buid all selected package settings.

"Rebuild All" will rebuild all packages.

"Clear Recently Built" will discard everything in the "RemotePackageManager/AssetBundles_RecentlyBuilt/"
temp folder.

"Show Outdated" will list all detected outdated packages. Also, it's possible to build all outdated
packages at once when it's selected.


UPLOADING TO AMAZON S3
----------------------

In the "Window/Remote Package Manager" "Upload" tab:

Inside "AmazonS3Uploader" you will find fields to put your Amazon AWS's credentials so we can upload
there automatically for you.

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

Additionally, you can check the button "Clear After Upload" (upper left corner) to automatically delete the
"RemotePackageManager/AssetBundles_RecentlyBuilt/" folder after the upload success!


UPLOADING BY FTP
----------------

Inside "FtpUploader" you will find fields to put your FTP credentials so we can upload
there automatically for you.

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

Additionally, you can check the button "Clear After Upload" (upper left corner) to automatically delete the
"RemotePackageManager/AssetBundles_RecentlyBuilt/" folder after the upload success!


CONTACT INFO
------------

This asset was made by Matheus Lessa Rodrigues, a programmer @ BitCake Studio ("http://bitcakestudio.com")

If you have any questions, please feel free to contact him at "matheus@bitcakestudio.com"





*******************************
Thank you for supporting us! :)
*******************************
