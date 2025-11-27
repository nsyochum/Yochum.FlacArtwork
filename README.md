This project tags artwork on *.FLAC files.

__TOC__

# Summary
This program will crawl through sub directories of the direcory provided in the command line argument and apply a `*.JPG` or `*.PNG` as the `Pictures` tag on any `.FLAC` files in the directory.  
  - If there is a directory with no `*.jpg|.png` files **or** no `*.flac` files, the directory will be skipped.  
  - If the `Pictures` tag is already set on a `*.flac` file, it will be skipped. 

# Args  
`<Directory>`  
  - The Directory to trawl for flac files  

`-f|--force`  
  - Force updating artwork even if the files already have artwork  

# Pre-reqs
- dotnet9 SDK  
  
# Caveats
Theoretically, this should work for *NIX operating systems since dotnet9 is fully cross-platform, but it has only been tested on Win11
