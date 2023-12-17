# Instructional Splitter
A tool to take instructional files that contain a whole DVD/volume (for example the downloads from BJJ Fanatics) and splits them into sections. This allows for quickly navigating to a specific section. The splitting is done using [FFMpeg](https://ffmpeg.org/) which must be installed.

# How it Works
The information about where the files to split and what sections to split them into are stored in the `parts.txt` file. The file has the following format:
1) A line that begin with `FP:`, then a tab character (`\t`), then the absolute path to the file that is to be split.
2) A number of lines that represents the sections the file is to be split to. The format is the name of the section, then a tab character (`\t`), the start time of the section (relative to the beginning of the file being split), and dash (`-`), then the end time of the section (relative to the beginning of the file being split).
    * The section part format was modeled off the tables in the Course Content of BJJ Fanatics. You should be able to just copy and paste those course content tables into `parts.txt`.

1 and 2 can be repeated for any number of files. Please see `parts.txt` for an example.

# File Names

## Input File Name Format
The input file should have the following format `Name.Of.Instructional-E0###.extension` where ### can be any number (usually DVD/volume number). The extension can be any extension that FFMpeg supports.

## Output File Name Format
The output files will be named `Name.Of.Instructional-E##-Name.of.Section.extension` where ## will be the section number based on the order it appears in `parts.txt`. This format should be compatible with media solutions like [Jellyfin](https://jellyfin.org/) or [Plex](https://www.plex.tv/) so the instructional can be views in a TV style library.

## Example
Using `parts.txt` as an example we would take the two provided input files (`/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E01.mp4` and `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E02.mp4`) and split them into 6 sections:
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E1-Multiplicity.of.Triangles.mp4`
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E2-The.2.Stage.Approach.to.Triangles.mp4`
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E3-Mechanics.mp4`
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E4-Front.Triangle.Mechanics.2.mp4`
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E5-Front.Triangle.Mechanics.3.mp4`
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E6-Managing.Transition.from.Trap.Triangle.to.Figure.Four.Triangle.mp4`
* `/path/to/file/Enter.the.System.by.John.Danaher.Triangles-E7-Managing.Transition.from.Trap.Triangle.to.Figure.Four.Triangle.2.mp4`


