# Barefoot Media Helper
A media processing utility / frontend used by Barefoot Invader team

Made for [Barefoot Invader (素足星侵略者)](https://space.bilibili.com/259213)

<p align="center">
  <img src="https://github.com/xlfdll/xlfdll.github.io/raw/master/images/projects/BarefootMediaHelper.png"
       alt="Barefoot Media Helper" width="1024">
</p>

### Features
* Media Download (YouTube / SoundCloud)
* Media Transcription (based on Whisper)
* Composition for BB
* Format Conversion (FLV => MP4)
* Subtitle Removal (both soft/embedded and hard-coded)
* Audio Extraction (from videos)

## System Requirements
* .NET Framework 4.8

[Runtime configuration](https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-configure-an-app-to-support-net-framework-4-or-4-5) may be needed for running on other versions of .NET Framework.

## Development Prerequisites
* Visual Studio 2017+

Before the build, generate-build-number.sh needs to be executed in a Git / Bash shell to generate build information code file (`BuildInfo.cs`).

## External Sources
User interface is based on [MahApps.Metro](https://github.com/MahApps/MahApps.Metro) library, which is licensed under [MIT](https://github.com/MahApps/MahApps.Metro/blob/develop/LICENSE) license.

Hard-coded subtitle removal algorithm and associated files are from:
https://forum.videohelp.com/threads/358181-How-to-remove-hard-coded-subtitles-HELP-PLEASE
