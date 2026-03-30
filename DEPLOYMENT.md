# Deployment

This project uses **GitHub Actions** and **Fastlane** to build the Unity iOS app and deliver it to App Store Connect (TestFlight).

## How It Works

1. **Trigger** — Push a version tag (`v1.0.0`) or use the manual "Run workflow" button in GitHub Actions
2. **Unity Build** — `game-ci/unity-builder` builds the Xcode project from Unity in batch mode
3. **Code Signing** — Fastlane `match` fetches the distribution certificate and provisioning profile from a private git repo
4. **Archive & Upload** — Fastlane builds the IPA and uploads it to TestFlight

## One-Time Setup

Complete these steps before the pipeline will work.

### 1. Create an App Store Connect API Key

1. Go to [App Store Connect > Users and Access > Integrations > Team Keys](https://appstoreconnect.apple.com/access/integrations/api)
2. Click **Generate API Key**, give it a name, and select the **App Manager** role (or higher)
3. Download the `.p8` file — you can only download it once
4. Note the **Key ID** and **Issuer ID** shown on the page

Base64-encode the `.p8` file for storage as a secret:

```bash
base64 -i AuthKey_XXXXXXXXXX.p8 | pbcopy
```

### 2. Export Your Unity License

The CI runner needs an activated Unity license to build. Use `game-ci/unity-activate` or activate manually:

1. On your local machine (with Unity installed), find your license file:
   - macOS: `~/Library/Application Support/Unity/Unity_lic.ulf`
   - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
2. Copy the entire contents of this `.ulf` file — that's your `UNITY_LICENSE` secret

Alternatively, follow the [game-ci activation docs](https://game.ci/docs/github/activation) for a dedicated CI license.

### 3. Create a Private Git Repo for Match

Fastlane `match` stores certificates and provisioning profiles in a private git repo.

1. Create a new **private** GitHub repo (e.g., `oliverholmberg/ios-certificates`)
2. Generate a [Personal Access Token](https://github.com/settings/tokens) with `repo` scope
3. Base64-encode the credentials for CI access:
   ```bash
   echo -n "your-github-username:ghp_yourtoken" | base64
   ```
   This is your `MATCH_GIT_BASIC_AUTHORIZATION` secret.

### 4. Initialize Match and Generate Certificates

Run this once locally to create the distribution certificate and provisioning profile:

```bash
# Install dependencies
bundle install

# Set required environment variables
export ASC_KEY_ID="your-key-id"
export ASC_ISSUER_ID="your-issuer-id"
export ASC_KEY_CONTENT="$(base64 -i path/to/AuthKey.p8)"
export MATCH_GIT_URL="https://github.com/oliverholmberg/ios-certificates.git"
export MATCH_PASSWORD="a-strong-encryption-passphrase"

# Generate and store the App Store distribution certificate + profile
bundle exec fastlane match appstore
```

You'll be prompted to confirm. Match will create the certificate in your Apple Developer account and commit the encrypted files to your private repo.

### 5. Create the App in App Store Connect

If you haven't already:

1. Go to [App Store Connect > Apps](https://appstoreconnect.apple.com/apps)
2. Click **+** > **New App**
3. Select **iOS**, enter the app name, and set the bundle ID to `com.oliverholmberg.apoapsis2`

### 6. Add GitHub Secrets

Go to your repo **Settings > Secrets and variables > Actions** and add:

| Secret | Description |
|---|---|
| `UNITY_LICENSE` | Contents of your `Unity_lic.ulf` file |
| `ASC_KEY_ID` | App Store Connect API Key ID |
| `ASC_ISSUER_ID` | App Store Connect API Issuer ID |
| `ASC_KEY_CONTENT` | Base64-encoded `.p8` key file |
| `APPLE_TEAM_ID` | Your Apple Developer Team ID ([find it here](https://developer.apple.com/account#MembershipDetailsCard)) |
| `MATCH_GIT_URL` | URL of your private match certificates repo |
| `MATCH_PASSWORD` | Encryption passphrase you chose during `match init` |
| `MATCH_GIT_BASIC_AUTHORIZATION` | Base64-encoded `username:token` for git access |

## Triggering a Release

**Via tag:**

```bash
git tag v1.0.0
git push origin v1.0.0
```

**Via GitHub UI:**

Go to Actions > "Build & Deploy iOS to App Store Connect" > Run workflow.

## Local Usage

You can also run the Fastlane lane locally (after a Unity iOS build):

```bash
# Build in Unity first (or use the existing build script)
./build-ios.sh device --skip-unity  # if you already have a Unity build

# Or run the full Fastlane deploy
export ASC_KEY_ID="..."
export ASC_ISSUER_ID="..."
export ASC_KEY_CONTENT="$(base64 -i path/to/AuthKey.p8)"
export APPLE_TEAM_ID="..."
export MATCH_GIT_URL="..."
export MATCH_PASSWORD="..."

bundle exec fastlane ios deploy
```

## Updating Certificates

If your distribution certificate expires or is revoked:

```bash
# Nuke old certs and generate new ones
bundle exec fastlane match nuke distribution
bundle exec fastlane match appstore
```

## File Overview

```
.github/workflows/build-and-deploy-ios.yml  # GitHub Actions workflow
fastlane/
  Appfile       # App identifier config
  Fastfile      # Deploy lane (match + build + upload)
  Matchfile     # Match storage config
Gemfile         # Ruby dependencies (fastlane)
```
