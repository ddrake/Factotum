# Factotum
Report generating application for FAC inspection teams at nuclear plants.

This application was originally designed and built in 2006 for .NET 2.0 using the C# language.  It uses a SQL Compact Edition database.  The schema and initial data are provided in the source code.

Factotum has been used to generate reports in about a half-dozen outages that I know of.  A number of features have been added over the years, and version 1.24 is fairly solid.  It runs on Windows XP, Vista and Windows 7.  I haven't tested it on Windows 8 or the upcoming Windows 10.

The original application code contained some complicated functionality which was designed to restrict use of the program to individuals approved by Westinghouse Nuclear Division.  Some of this code, including remote procedure calls to an external web service, has now been removed as of version 1.24.  As of version 1.24, any PC with Factotum installed is automatically a "system master" PC, with full capabilities to create both master data files and outage data files.  Most of the remaining obsolete anti-piracy related code has since been removed in commits made after the version 1.24 tag.

The old programming machine/environment I used for development of this project failed the day after I released version 1.24, and due to limitations of my current development environment (a Windows 7 virtual machine running on a cheap laptop with a Linux Mint host), I have converted this project from Visual Studio 2005 to Sharp Develop 5.1.  The original Setup project could not be automatically converted, so I'm working on creating a new one using Wix.  It's turning out to be a complicated task (and not particularly interesting) so it's unlikely that I'll finish it any time soon.

Going forwared, I think this project would benefit by converting to use SQL Express instead of the poorly supported SQL Compact Edition.  If I had sufficient motivation, I would plan to replace the home-made ORM with a modern, well-supported one like Entity Framework 6, and upgrade/re-write the code to take advantage of .NET 4.0 features.

This project would also benefit from documentation, which is sorely lacking.

At this point, I don't have plans to add any more features or upgrade/test for later versions of Windows.  Any interested parties should fork this repository and make all changes on their own forks.  

The current release, version 1.24 (5/8/2015) is available here on the 'releases' tab.
Here are links to some older versions in case they might be useful:

http://dowdrake.com/uploads/factotum/Factotum_v1.23_Setup.zip
http://dowdrake.com/uploads/factotum/Factotum_1.22_setup.zip
