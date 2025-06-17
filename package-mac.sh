#!/bin/bash
set -e

# This script packages the Renderer.app into a .dmg file for distribution.
# It should be run on a macOS machine.

# --- Configuration ---
APP_NAME="Renderer"
VOL_NAME="${APP_NAME}"
PUBLISH_DIR_X64="Renderer/bin/Release/net8.0/osx-x64/publish"
PUBLISH_DIR_ARM64="Renderer/bin/Release/net8.0/osx-arm64/publish"
DMG_PATH_X64="${PUBLISH_DIR_X64}/${APP_NAME}-x64.dmg"
DMG_PATH_ARM64="${PUBLISH_DIR_ARM64}/${APP_NAME}-arm64.dmg"

# --- Function to create DMG ---
create_dmg() {
    local publish_dir=$1
    local dmg_path=$2
    local app_path="${publish_dir}/${APP_NAME}.app"

    if [ ! -d "$app_path" ]; then
        echo "Error: ${app_path} not found. Please run the build script first."
        exit 1
    fi

    echo "Creating DMG for ${app_path}..."

    # Create a temporary directory
    mkdir -p "temp_dmg"
    cp -R "$app_path" "temp_dmg/"

    # Create the DMG file
    hdiutil create -volname "${VOL_NAME}" -srcfolder "temp_dmg" -ov -format UDZO "${dmg_path}"

    # Clean up
    rm -rf "temp_dmg"

    echo "Successfully created ${dmg_path}"
}

# --- Create DMGs for both architectures ---
create_dmg "$PUBLISH_DIR_X64" "$DMG_PATH_X64"
create_dmg "$PUBLISH_DIR_ARM64" "$DMG_PATH_ARM64"

echo "macOS packaging complete." 