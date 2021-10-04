#!/bin/bash

nextVersion=$(dotnet dotnet-gitversion -nofetch -output json -showvariable NuGetVersionV2)
{
echo ""
echo "## v$nextVersion"
echo ""
git log $(git describe --tags --abbrev=0)..HEAD --pretty=format:"%s" -i -E --grep="^(feat|fix)" >> CHANGELOG.md
}  >> CHANGELOG.md


#https://blogs.sap.com/2018/06/22/generating-release-notes-from-git-commit-messages-using-basic-shell-commands-gitgrep/
