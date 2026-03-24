#!/usr/bin/env bash
set -euo pipefail

UNITY="/Applications/Unity/Hub/Editor/6000.3.11f1/Unity.app/Contents/MacOS/Unity"
PROJECT="/Users/oliverholmberg/dev/_personal/apoapsis2"
BUILD_DIR="$PROJECT/Builds/iOS"
IPHONE_UDID="E76CBB12-0DC6-4765-9653-4062DC1FB0C5"   # iPhone 15 Pro, iOS 17.2
IPAD_UDID="2DB55999-CF19-4FAC-95FF-F27161C901DD"     # iPad Pro 11-inch, iOS 17.2
SIMULATOR_UDID="${SIMULATOR_UDID:-$IPHONE_UDID}"
BUNDLE_ID="${BUNDLE_ID:-com.oliverholmberg.apoapsis2}"

usage() {
    echo "Usage: $0 [simulator|device] [--skip-unity] [--clean] [--ipad]"
    echo ""
    echo "  simulator    Build and deploy to iOS Simulator (default)"
    echo "  device       Build and deploy to connected iOS device"
    echo "  --skip-unity Skip Unity build, just recompile Xcode and deploy"
    echo "  --clean      Clean build directory before building"
    echo "  --ipad       Use iPad simulator instead of iPhone"
    echo ""
    echo "Environment variables:"
    echo "  APPLE_TEAM_ID    Apple Developer Team ID (required for device builds)"
    echo "  SIMULATOR_UDID   Override default simulator UDID"
    echo "  BUNDLE_ID        Override default bundle identifier"
    exit 1
}

TARGET="${1:-simulator}"
SKIP_UNITY=false
CLEAN=false

shift || true
for arg in "$@"; do
    case $arg in
        --skip-unity) SKIP_UNITY=true ;;
        --clean) CLEAN=true ;;
        --ipad) SIMULATOR_UDID="$IPAD_UDID" ;;
        -h|--help) usage ;;
        *) echo "Unknown option: $arg"; usage ;;
    esac
done

if [ "$TARGET" != "simulator" ] && [ "$TARGET" != "device" ]; then
    echo "Unknown target: $TARGET"
    usage
fi

if [ "$TARGET" = "device" ] && [ -z "${APPLE_TEAM_ID:-}" ]; then
    echo "Error: APPLE_TEAM_ID environment variable is required for device builds."
    echo "Find your Team ID at https://developer.apple.com/account under Membership Details."
    echo ""
    echo "Usage: APPLE_TEAM_ID=XXXXXXXXXX $0 device"
    exit 1
fi

if [ "$CLEAN" = true ]; then
    echo "🧹 Cleaning build directory..."
    rm -rf "$BUILD_DIR"
fi

# ─── Stage 1: Unity Build ───────────────────────────────────────────────────
if [ "$SKIP_UNITY" = false ]; then
    echo ""
    echo "═══ Stage 1: Unity Build ($TARGET) ═══"
    echo ""

    METHOD="BuildScript.BuildiOSSimulator"
    [ "$TARGET" = "device" ] && METHOD="BuildScript.BuildiOSDevice"

    "$UNITY" \
        -quit \
        -batchmode \
        -nographics \
        -projectPath "$PROJECT" \
        -executeMethod "$METHOD" \
        -logFile -

    echo ""
    echo "Unity build complete."
fi

# ─── Stage 2: Xcode Build ───────────────────────────────────────────────────
echo ""
echo "═══ Stage 2: Xcode Build ═══"
echo ""

cd "$BUILD_DIR"

if [ "$TARGET" = "simulator" ]; then
    xcodebuild \
        -project Unity-iPhone.xcodeproj \
        -scheme Unity-iPhone \
        -configuration Debug \
        -sdk iphonesimulator \
        -derivedDataPath ./DerivedData \
        ARCHS=x86_64 \
        ONLY_ACTIVE_ARCH=NO \
        CODE_SIGNING_ALLOWED=NO \
        -quiet

    APP_PATH="$(find ./DerivedData/Build/Products/Debug-iphonesimulator -name 'apoapsis2.app' -maxdepth 1 2>/dev/null || echo './DerivedData/Build/Products/Debug-iphonesimulator/apoapsis2.app')"

elif [ "$TARGET" = "device" ]; then
    xcodebuild \
        -project Unity-iPhone.xcodeproj \
        -scheme Unity-iPhone \
        -configuration Debug \
        -sdk iphoneos \
        -arch arm64 \
        -derivedDataPath ./DerivedData \
        -allowProvisioningUpdates \
        -quiet

    APP_PATH="./DerivedData/Build/Products/Debug-iphoneos/apoapsis2.app"
fi

echo "Xcode build complete: $APP_PATH"

# ─── Stage 3: Deploy ────────────────────────────────────────────────────────
echo ""
echo "═══ Stage 3: Deploy ═══"
echo ""

if [ "$TARGET" = "simulator" ]; then
    # Boot simulator (no-op if already booted)
    xcrun simctl boot "$SIMULATOR_UDID" 2>/dev/null || true
    open -a Simulator

    # Install and launch
    xcrun simctl install "$SIMULATOR_UDID" "$APP_PATH"
    xcrun simctl launch "$SIMULATOR_UDID" "$BUNDLE_ID"

    echo "Launched on simulator ($SIMULATOR_UDID)"

elif [ "$TARGET" = "device" ]; then
    # Find first connected device
    DEVICE_UDID="${DEVICE_UDID:-}"
    if [ -z "$DEVICE_UDID" ]; then
        echo "Searching for connected device..."
        DEVICE_UDID=$(xcrun devicectl list devices 2>/dev/null | grep -E "iPhone|iPad" | head -1 | awk '{print $NF}' || true)
    fi

    if [ -z "$DEVICE_UDID" ]; then
        echo "Error: No connected iOS device found."
        echo "Connect a device via USB and try again."
        echo ""
        echo "To specify a device manually:"
        echo "  DEVICE_UDID=<udid> $0 device --skip-unity"
        exit 1
    fi

    echo "Deploying to device: $DEVICE_UDID"
    xcrun devicectl device install app --device "$DEVICE_UDID" "$APP_PATH"
    xcrun devicectl device process launch --device "$DEVICE_UDID" "$BUNDLE_ID"

    echo "Launched on device ($DEVICE_UDID)"
fi

echo ""
echo "═══ Done ═══"
