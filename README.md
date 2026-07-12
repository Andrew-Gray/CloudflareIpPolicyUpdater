# Cloudflare Policy Updater

A small .NET console application that keeps a [Cloudflare Access reusable policy](https://developers.cloudflare.com/cloudflare-one/policies/access/) synchronized with the current public IP addresses of the machine on which it runs.

On each run, the application:

1. Retrieves the public IPv4 and IPv6 addresses from ipify.
2. Reads the local IPv4 and IPv6 addresses from a selected network adapter.
3. Compares the addresses with those saved during the previous run.
4. Sends a Gmail notification when any address changes.
5. Updates the Cloudflare reusable policy when a public address changes.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A Cloudflare account with an existing Access reusable policy
- A Cloudflare API token that can read and edit Access policies for the account
- A Gmail account with an [app password](https://support.google.com/accounts/answer/185833) if email notifications are required

The machine must also be able to reach the Cloudflare API, `api.ipify.org`, `api6.ipify.org`, and Gmail's SMTP service.

## Configuration

From the project directory, copy the example configuration:

```powershell
cd CloudflarePolicyUpdater
Copy-Item config.example.json config.json
```

Then edit `config.json`:

```json
{
  "Cloudflare": {
    "ApiToken": "<Your_API_Token>",
    "AccountId": "<Your_Account_Id>",
    "PolicyId": "<Your_Policy_Id>",
    "Country": "<Your_Country_Code_2_Char>",
    "Ipv6PrefixLength": 64
  },
  "Gmail": {
    "Email": "<Your_Gmail_Address>",
    "AppPassword": "<Your_Gmail_App_Password>"
  },
  "General": {
    "NetworkAdapterName": "<Your_Network_Adapter_Name>",
    "IpLogFile": "ip_log.txt"
  }
}
```

| Setting | Description |
| --- | --- |
| `Cloudflare.ApiToken` | Bearer token used to call the Cloudflare API. |
| `Cloudflare.AccountId` | Cloudflare account containing the reusable policy. |
| `Cloudflare.PolicyId` | ID of the reusable Access policy to update. |
| `Cloudflare.Country` | Optional two-letter country code added as a policy requirement. Use an empty string to omit it. |
| `Cloudflare.Ipv6PrefixLength` | Optional IPv6 network prefix length. For example, `64` turns the detected IPv6 address into its `/64` network. |
| `Gmail.Email` | Gmail address used as both sender and recipient. |
| `Gmail.AppPassword` | Gmail app password used for SMTP authentication. |
| `General.NetworkAdapterName` | Exact operating-system name of the adapter used to find local addresses. |
| `General.IpLogFile` | Path to the state file that stores the most recently detected addresses. Relative paths are resolved from the working directory. |

To find adapter names on Windows, run:

```powershell
Get-NetAdapter | Select-Object Name, Status
```

Keep `config.json` private: it contains credentials and should not be committed to source control.

## Run

The application expects `config.json` in its current working directory, so run it from the project directory:

```powershell
cd CloudflarePolicyUpdater
dotnet restore
dotnet run
```

The first run creates the configured IP log file, sends an address notification, and updates the Cloudflare policy. Later runs exit without external updates when no addresses have changed. A local-only address change sends an email but does not update Cloudflare.

For unattended operation, publish the application and invoke it periodically with Task Scheduler, cron, or another scheduler:

```powershell
dotnet publish -c Release -o publish
cd publish
./CloudflarePolicyUpdater.exe
```

Copy `config.json` into the publish directory before running the published executable. The project file also copies an existing `config.json` to the output directory during build and publish.

## Update the application version

Set `$Version` near the top of `Update-Version.ps1`, then run the script from the repository root to update both `FileVersion` and `AssemblyVersion`:

```powershell
.\Update-Version.ps1
```

The version must contain three or four numeric components. Use `-WhatIf` to preview the change without writing it, or pass `-ProjectPath` to target a different project file.

## Policy behavior

The updater preserves the existing policy's name and decision, but replaces its `include` rules with the detected public IPv4 address and, when available, the public IPv6 address or configured IPv6 network. If `Cloudflare.Country` is set, it also replaces the policy's `require` rules with that country requirement.

Because the policy is updated with `PUT`, test with a non-critical reusable policy first and confirm that the resulting rules match your intended access controls.

## License

This project is licensed under the [MIT License](LICENSE).
