# Factotum
Report generating application for FAC inspection teams at nuclear plants.

This application was originally designed and built in 2006 for .NET 2.0 using the C# language.  It uses a SQL Compact Edition database.  The schema and initial data are provided in the source code.

Factotum has been used to generate reports in about a half-dozen outages that I know of.  A number of features have been added over the years, and version 1.24 is fairly stable.  It runs on Windows XP, Vista and Windows 7.  I haven't tested it on Windows 8 or the upcoming Windows 10.

The original application code contained some complicated functionality, designed to restrict use of the program to individuals approved by Westinghouse Nuclear Division.  Some of this code, including remote procedure calls to an external web service, has now been removed as of version 1.24.  As of version 1.24, any PC with Factotum installed is automatically a "system master" PC, with full capabilities to create both master data files and outage data files.  If time permits, I plan to remove the rest of the now obsolete anti-piracy related code.

The programming machine/environment I used for development of this project has recently failed, and this project has been converted from Visual Studio 2005 to Sharp Develop 5.1.  The original Setup project could not be automatically converted, so I'm working on creating a new one using Wix.  

I think this project would benefit by converting to use SQL Express instead of the poorly supported SQL Compact Edition.  I would recommend replacing the home-made ORM with a modern, well-supported one like Entity Framework 6, and upgrading/re-writing the code to use at least .NET 4.0.  Unfortunately, .NET 4.5 and above will not work on Windows XP machines, and there may still be some of these at some nuclear plants.  The project would also benefit from documentation, which is sorely lacking.

At this point, I don't have plans to add any more features or upgrade/test for later versions of Windows.  Any interested parties should feel free to fork this repository and make any changes they find useful.
