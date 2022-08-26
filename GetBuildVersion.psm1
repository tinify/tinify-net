# MADE.NET
#
# Copyright (c) MADE Apps
#
# MIT License (MIT)
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.
#
Function GetBuildVersion {
    Param (
        [string]$VersionString
    )

    # Process through regex
    $VersionString -match "(?<major>\d+)(\.(?<minor>\d+))?(\.(?<patch>\d+))?(\-(?<pre>[0-9A-Za-z\-\.]+))?(\+(?<build>\d+))?" | Out-Null

    if ($matches -eq $null) {
        return "1.0.0-build"
    }

    # Extract the build metadata
    $BuildRevision = [uint64]$matches['build']
    # Extract the pre-release tag
    $PreReleaseTag = [string]$matches['pre']
    # Extract the patch
    $Patch = [uint64]$matches['patch']
    # Extract the minor
    $Minor = [uint64]$matches['minor']
    # Extract the major
    $Major = [uint64]$matches['major']

    $Version = [string]$Major + '.' + [string]$Minor + '.' + [string]$Patch;
    if ($PreReleaseTag -ne [string]::Empty) {
        $Version = $Version + '-' + $PreReleaseTag
    }

    if ($BuildRevision -ne 0) {
        $Version = $Version + '.' + [string]$BuildRevision
    }

    return $Version
}